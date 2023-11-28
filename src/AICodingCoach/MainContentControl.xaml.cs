using RoslynPad.Roslyn;
using System.Windows.Controls;
using System.Windows.Input;
using RoslynPad.Editor;
using static System.Net.Mime.MediaTypeNames;

namespace CodingCanvasWpfApp
{

    /// <summary>
    /// Interaction logic for MainContentControl.xaml
    /// </summary>
    public partial class MainContentControl : UserControl
    {
        public CompilationService CompilationService { get; }

        public MainContentControl()
        {
            InitializeComponent();
            CompilationService = new CompilationService(Canvas, CompilationMessages);
        }

        private void CodeEditor_OnTextChanged(object? sender, EventArgs e)
        {
            // TODO: it would be better to use binding 
            CompilationService.Text = CodeEditor.Text;
        }
    }
}
