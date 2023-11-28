using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.CodeAnalysis;
using System.Collections.ObjectModel;

namespace CodingCanvasWpfApp
{
    public class CompilationService 
    {
        public readonly Project Project;
        public readonly Thread CompilationThread;
        public ObservableCollection<string> Diagnostics { get; }

        public bool Dirty = false;
        public object UserObject { get; set; }
        public MethodInfo Method { get; set; }
        private string _text;
        public string Text;
        public bool IsValid => UserObject != null && Method != null;
        public readonly VisualHost Canvas;
        public readonly TextBox DiagnosticsControl;
        public DateTimeOffset WhenModified = DateTimeOffset.Now;

        // How many milliseconds since the last edit before compilation 
        public int CompilationOnIdleMSec = 500;

        public CompilationService(VisualHost canvas, TextBox diagnosticsControl)
        {
            var tmp = Assembly.Load(
                new AssemblyName("System.Runtime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));

            var tmp2 = Assembly.Load("PresentationCore");

            var assembliesToRef = new List<Assembly>
            {
                typeof(object).Assembly, //mscorlib
                Assembly.Load("RoslynPad.Roslyn.Windows"),
                Assembly.Load("RoslynPad.Editor.Windows"),
                typeof(Canvas).Assembly,
                typeof(Math).Assembly,
                typeof(Ara3D.Math.Line2D).Assembly,
                typeof(DrawingContext).Assembly,
                typeof(Point).Assembly,
                typeof(StreamGeometry).Assembly,
                tmp,
                tmp2,
            };

            var namespaces = new List<string>
            {
                "System",
                "System.Collections.Generic",
                "System.IO",
                "System.Linq",
                "System.Diagnostics",
                "CodingCanvasWpfApp",
                "Ara3D.Math",
                "System.Windows",
                "System.Windows.Media",
                "System.Windows.Media.Geometry",
            };


            Project = new Project("Test", assembliesToRef);
            CompilationThread = new Thread(CompilationThreadStart);
            CompilationThread.Start();
        }

        public void UpdateVisual()
        {
            if (!IsValid)
                return;

            // https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/using-drawingvisual-objects?view=netframeworkdesktop-4.8
            // https://github.com/microsoft/WPF-Samples/blob/main/Visual%20Layer/DrawingVisual/MyVisualHost.cs
            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen())
            {
                Method.Invoke(UserObject, new object[] { dc });
            }

            Canvas.SetVisual(dv);
        }

        public void ExecuteCode(Assembly asm)
        {
            try
            {
                UserObject = null;
                Method = null;
                Canvas.Clear();

                if (asm == null)
                    throw new Exception("No assembly provided");

                var type = asm.GetTypes().FirstOrDefault();
                if (type == null)
                    throw new Exception("No types found");

                Method = type.GetMethod("Draw", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                if (Method == null)
                    throw new Exception("No Draw method found");

                if (Method.GetParameters().Length != 1)
                    throw new Exception($"Expected method to have one parameter not {method.GetParameters().Length}");

                var pi = Method.GetParameters()[0];
                if (pi.ParameterType != typeof(DrawingContext))
                    throw new Exception($"Expected method to have one parameter of type Canvas not {pi.ParameterType}");
               
                UserObject = Activator.CreateInstance(type);
                if (UserObject == null)
                    throw new Exception($"Was not able to construct an instance of {type}");

                IsValid = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                Diagnostics.Control
            }
        }

        public void Recompile()
        {
            Project.Recompile(Text);
            var msgs = Project.Diagnostics;
            var asm = Project.Assembly;
            DiagnosticsControl.Dispatcher.Invoke(
                () => SetCompilationMessages(msgs));
            Canvas.Dispatcher.Invoke(
                () => ExecuteCode(asm));
        }

        public void CompilationThreadStart()
        {
            while (true)
            {
                var elapsed = (DateTimeOffset.Now - WhenModified);
                if (Dirty && elapsed.TotalMilliseconds > CompilationOnIdleMSec)
                {
                    Dirty = false;
                    Recompile();
                }
                Thread.Sleep(CompilationOnIdleMSec / 2);
            }
        }

        public void SetCompilationMessages(IEnumerable<string> lines)
        {
            DiagnosticsControl.Text = string.Join(Environment.NewLine, lines);
        }


        public void UpdateText(string text)
        {

            Dirty = true;
            Project.TokenSource.Cancel();
            Text = text;
            WhenModified = DateTimeOffset.Now;
        }
    }
}
