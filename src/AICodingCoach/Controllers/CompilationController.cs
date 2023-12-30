using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AICodingCoach.Utilities;
using AICodingCoach.Views;
using Ara3D.Utils;

namespace AICodingCoach.Services
{
    /// <summary>
    /// Compiles and runs projects.
    /// It does this on a separate thread, and has dependencies on specific UI controls
    /// for output.  
    /// </summary>
    public class CompilationController
    {
        public readonly ProjectCompilation ProjectCompilation;
        public readonly Thread CompilationThread;

        public bool Dirty { get; set; }
        public object? UserObject { get; set; }
        public MethodInfo? Method { get; set; }
        public Type DrawingType;
        private string _text = "";

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

        // How many milliseconds between frame draws
        public readonly int FrameUpdateMSec = 100;

        public CompilationController(VisualHost canvas, TextBox diagnosticsControl)
        {
            Canvas = canvas;
            DiagnosticsControl = diagnosticsControl;

            var tmp = Assembly.Load(
                new AssemblyName("System.Runtime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));

            var tmp2 = Assembly.Load("PresentationCore");

            // TODO: this would have been nice to be in a text file. 
            var assembliesToRef = new List<Assembly>
            {
                typeof(object).Assembly, //mscorlib
                typeof(Canvas).Assembly,
                typeof(Math).Assembly,
                typeof(DrawingContext).Assembly,
                typeof(Point).Assembly,
                typeof(StreamGeometry).Assembly,
                typeof(Stack<>).Assembly,
                tmp,
                tmp2,
            };

            ProjectCompilation = new ProjectCompilation("Test", assembliesToRef);
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
                {
                    OutputDiagnostics("No assembly created.");
                    return;
                }

                var classTypes = asm.GetTypes().Where(t => t.IsClass).ToList();
                if (classTypes.Count == 0)
                {
                    OutputDiagnostics("No classes found.");
                    return;
                }

                var constructibleTypes = classTypes.Where(t => t.HasDefaultConstructor()).ToList();
                if (constructibleTypes.Count == 0)
                {
                    OutputDiagnostics("No classes found with a default constructor.");
                    return;
                }


                foreach (var type in asm.GetTypes())
                {
                    var methods = type.GetMethods(
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.NonPublic);

                    foreach (var method in methods)
                    {
                        if (method.Name == "Draw" && method.GetParameters().Length == 1)
                        {
                            DrawingType = type;
                            Method = method;
                            break;
                        }
                    }
                }

                if (Method == null)
                {
                    OutputDiagnostics("No class with a `Draw` method taking a single argument found");
                    return;
                }

                Canvas.Dispatcher.Invoke(() =>
                {
                    // We want the object's constructor to happen on the same thread of the canvas. 
                    // This prevents thread access errors. 
                    UserObject = Activator.CreateInstance(DrawingType);
                });
                
                if (UserObject == null)
                {
                    OutputDiagnostics($"Was not able to construct an instance of {DrawingType}");
                    return;
                }

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
                ProjectCompilation.Recompile(Text);
                var msgs = ProjectCompilation.Diagnostics;
                var asm = ProjectCompilation.Assembly;
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
                    // Uncomment the next line for animations
                    //UpdateVisual();
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
            ProjectCompilation.TokenSource.Cancel();
            _text = text;
            WhenModified = DateTimeOffset.Now;      
            // Recompilation happens when idle for a period of time 
        }
    }
}
