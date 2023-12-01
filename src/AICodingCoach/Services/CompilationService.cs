using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AICodingCoach.Views;

namespace AICodingCoach.Services
{
    public class CompilationService
    {
        public readonly Project Project;
        public readonly Thread CompilationThread;

        public bool Dirty { get; set; }
        public object? UserObject { get; set; }
        public MethodInfo? Method { get; set; }
        private string _text;

        public string Text
        {
            get => _text;
            set => UpdateText(value);
        }

        public bool IsValid => UserObject != null && Method != null;
        public readonly VisualHost Canvas;
        public readonly TextBox DiagnosticsControl;
        public DateTimeOffset WhenModified = DateTimeOffset.Now;

        // How many milliseconds since the last edit before compilation 
        public readonly int CompilationOnIdleMSec = 300;
        public readonly int FrameUpdateMSec = 100;

        public CompilationService(VisualHost canvas, TextBox diagnosticsControl)
        {
            Canvas = canvas;
            DiagnosticsControl = diagnosticsControl;

            var tmp = Assembly.Load(
                new AssemblyName("System.Runtime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));

            var tmp2 = Assembly.Load("PresentationCore");

            var assembliesToRef = new List<Assembly>
            {
                typeof(object).Assembly, //mscorlib
                typeof(Canvas).Assembly,
                typeof(Math).Assembly,
                //typeof(Ara3D.Math.Line2D).Assembly,
                typeof(DrawingContext).Assembly,
                typeof(Point).Assembly,
                typeof(StreamGeometry).Assembly,
                tmp,
                tmp2,
            };

            Project = new Project("Test", assembliesToRef);
            CompilationThread = new Thread(CompilationThreadStart);
            CompilationThread.Start();
        }

        public void UpdateVisual()
        {
            if (!IsValid)
                return;

            try
            {
                Canvas.Dispatcher.Invoke(
                    () =>
                    {
                        // https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/using-drawingvisual-objects?view=netframeworkdesktop-4.8
                        // https://github.com/microsoft/WPF-Samples/blob/main/Visual%20Layer/DrawingVisual/MyVisualHost.cs
                        var dv = new DrawingVisual();
                        using (var dc = dv.RenderOpen())
                        {
                             Method?.Invoke(UserObject, new object[] { dc });
                        }

                        Canvas.SetVisual(dv);
                    });
            }
            catch (Exception ex)
            {
                Method = null;
                OutputDiagnostics(ex.ToString());
            }
        }

        public void ExecuteCode(Assembly asm)
        {
            try
            {
                UserObject = null;
                Method = null;
                Canvas.Dispatcher.Invoke(() => Canvas.Clear());

                if (asm == null)
                    throw new Exception("No assembly provided");

                var type = asm.GetTypes().FirstOrDefault();
                if (type == null)
                    throw new Exception("No types found");

                Method = type.GetMethod("Draw", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                if (Method == null)
                    throw new Exception("No Draw method found");

                if (Method.GetParameters().Length != 1)
                    throw new Exception($"Expected method to have one parameter not {Method.GetParameters().Length}");

                var pi = Method.GetParameters()[0];
                if (pi.ParameterType != typeof(DrawingContext))
                    throw new Exception($"Expected method to have one parameter of type Canvas not {pi.ParameterType}");

                Canvas.Dispatcher.Invoke(() =>
                {
                    // We want the object's constructor to happen on the same thread of the canvas. 
                    // This prevents thread access errors. 
                    UserObject = Activator.CreateInstance(type);
                });
                if (UserObject == null)
                    throw new Exception($"Was not able to construct an instance of {type}");

                UpdateVisual();
            }
            catch (Exception ex)
            {
                OutputDiagnostics(ex.ToString());
            }
        }

        public void Recompile()
        {
            try
            {
                Project.Recompile(Text);
                var msgs = Project.Diagnostics;
                var asm = Project.Assembly;
                SetCompilationMessages(msgs);
                if (asm != null)
                    ExecuteCode(asm);
            }
            catch (Exception ex)
            {
                OutputDiagnostics($"Exception caught {ex}");
            }
        }

        public void CompilationThreadStart()
        {
            while (true)
            {
                var elapsed = DateTimeOffset.Now - WhenModified;
                if (Dirty && elapsed.TotalMilliseconds > CompilationOnIdleMSec)
                {
                    Dirty = false;
                    Recompile();
                }
                else
                {
                    UpdateVisual();
                }
                Thread.Sleep(FrameUpdateMSec);
            }
        }

        public void OutputDiagnostics(string s)
        {
            Debug.WriteLine(s);
            DiagnosticsControl.Dispatcher.Invoke(() => DiagnosticsControl.Text = s);
        }

        public void SetCompilationMessages(IEnumerable<string> lines)
        {
            OutputDiagnostics(string.Join(Environment.NewLine, lines));
        }

        public void UpdateText(string text)
        {
            if (_text == text)
                return;
            Dirty = true;
            Project.TokenSource.Cancel();
            _text = text;
            WhenModified = DateTimeOffset.Now;      
            // Recompilation happens when idle for a period of time (e.g., 500msec)
        }
    }
}
