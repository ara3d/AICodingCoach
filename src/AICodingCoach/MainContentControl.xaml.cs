using RoslynPad.Roslyn;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.CodeAnalysis;
using RoslynPad.Editor;

namespace CodingCanvasWpfApp
{

    /// <summary>
    /// Interaction logic for MainContentControl.xaml
    /// https://stackoverflow.com/questions/47293326/is-it-possible-to-include-intellisense-for-the-object-global-of-globalstype-in-t
    /// 
    /// </summary>
    public partial class MainContentControl : UserControl
    {
        public readonly IRoslynHost Host;
        public readonly Project Project;
        public readonly Thread CompilationThread;
        public bool Dirty = false;
        public string Text;
        public DateTimeOffset WhenModified = DateTimeOffset.Now;
        
        // How many milliseconds since the last edit before compilation 
        public int CompilationOnIdleMSec = 500;

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

        public MainContentControl()
        {
            InitializeComponent();
            
            // TODO: set a timer for compilation. 

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

            Host = new RoslynHost(
                assembliesToRef, 
                RoslynHostReferences.NamespaceDefault.With(
                    typeNamespaceImports: new [] { typeof(object) },
                    imports: namespaces),
                null);

            /*

            Host = new CustomRoslynHost(
                additionalAssemblies: assembliesToRef,
                references: RoslynHostReferences.NamespaceDefault
                    .With(typeNamespaceImports: new[]
                    {*/

            Project = new Project("Test", assembliesToRef);

            CompilationThread = new Thread(CompilationThreadStart);
            CompilationThread.Start();
        }

        private async void EditControl_OnLoaded(object sender, RoutedEventArgs e)
        { 
            await CodeEditor.InitializeAsync(Host, 
                new ClassificationHighlightColors(),
                System.IO.Path.Combine(System.IO.Path.GetTempPath(), "CodingCanvas"), 
                string.Empty, 
                SourceCodeKind.Regular);
        }
        
        private void CodeEditor_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
        }

        public void SetCompilationMessages(IEnumerable<string> lines)
        {
            CompilationMessages.Text = string.Join(Environment.NewLine, lines);
        }

        public void ExecuteCode(Assembly asm)
        {
            Canvas.Clear();

            if (asm == null)
            {
                Debug.WriteLine("No assembly provided");
                return;
            }

            var type = asm.GetTypes().FirstOrDefault();
            if (type == null)
            {
                Debug.WriteLine("No types found");
                return;
            }

            var method = type.GetMethod("Draw", BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.DeclaredOnly);
            if (method == null)
            {
                Debug.WriteLine("No Draw method found");
                return;
            }

            if (method.GetParameters().Length != 1)
            {
                Debug.WriteLine($"Expected method to have one parameter not {method.GetParameters().Length}");
                return;
            }

            var pi = method.GetParameters()[0];
            if (pi.ParameterType != typeof(DrawingContext))
            {
                Debug.WriteLine($"Expected method to have one parameter of type Canvas not {pi.ParameterType}");
                return;
            }

            var obj = Activator.CreateInstance(type);

            // https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/using-drawingvisual-objects?view=netframeworkdesktop-4.8
            // https://github.com/microsoft/WPF-Samples/blob/main/Visual%20Layer/DrawingVisual/MyVisualHost.cs
            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen())
            {
                method.Invoke(obj, new object[] { dc });
            }
            
            Canvas.SetVisual(dv);
        }

        public void Recompile()
        {   
            Project.Recompile(Text);
            var msgs = Project.Diagnostics;
            var asm = Project.Assembly;
            CompilationMessages.Dispatcher.Invoke(
                () => SetCompilationMessages(msgs));
            Canvas.Dispatcher.Invoke(
                () => ExecuteCode(asm));
        }

        private void CodeEditor_OnTextChanged(object? sender, EventArgs e)
        {
            Dirty = true;
            Project.TokenSource.Cancel();
            Text = CodeEditor.Text;
            WhenModified = DateTimeOffset.Now;
        }
    }
}
