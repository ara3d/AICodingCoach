using System.Windows.Controls;
using AICodingCoach.Services;
using Ara3D.Utils;

namespace AICodingCoach.Views
{
    /// <summary>
    /// Interaction logic for MainContentControl.xaml
    /// </summary>
    public partial class MainContentControl : UserControl
    {
        public CompilationService CompilationService { get; }
        public MainController Controller { get; }

        public MainContentControl()
        {
            InitializeComponent();
            CompilationService = new CompilationService(Canvas, CompilerOutput);
            ChatControl.ChatHistory.ViewModel.OnCopyCode = OnCopyCode;
            Controller = new MainController(CodeEditor, ChatControl.ChatHistory.ViewModel, ChatControl.ChatService);
            CodeEditor.ContextMenu = Controller.CreateContextMenu();
        }
        
        private void OnCopyCode(string code)
        {
            CodeEditor.Text = code;
        }

        private void CodeEditor_OnTextChanged(object? sender, EventArgs e)
        {
            // TODO: it would be better to use binding 
            CompilationService.Text = CodeEditor.Text;
        }
    }
}
