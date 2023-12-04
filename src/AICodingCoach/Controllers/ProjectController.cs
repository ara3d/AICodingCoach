using System.Windows.Controls;
using AICodingCoach.Services;
using AICodingCoach.ViewModels;
using AICodingCoach.Views;
using Ara3D.Utils;
using ICSharpCode.AvalonEdit;

namespace AICodingCoach.Controllers
{
    /// <summary>
    /// The relationship between the chat, code, and canvas
    /// is very complex. Data binding is also not well supported by
    /// the Avalon Edit command. So instead of a View-ProjectModel
    /// I have opted to create a controller that is aware of the specific
    /// controls and wires them manually through events.
    /// The term "controller" is not well-defined. You can think of this
    /// as a kind of coordinator or manager class.
    /// The project controller is responsible for:
    /// </summary>
    public class ProjectController
    {
        public string Name => ProjectViewModel.Name;

        public TextEditor CodeEditor => ProjectControl.CodeEditor;
        public CompilationController CompilationController { get; }
        public ProjectViewModel ProjectViewModel { get; }
        public ProjectControl ProjectControl { get; }

        public INamedCommand CutCommand { get; }
        public INamedCommand CopyCommand { get; }
        public INamedCommand PasteCommand { get; }
        public INamedCommand DeleteCommand { get; }
        public INamedCommand ExplainDetailedCommand { get; }
        public INamedCommand ExplainSuccinctlyCommand { get; }
        public INamedCommand SelectAllCommand { get; }
        public INamedCommand UndoCommand { get; }
        public INamedCommand RedoCommand { get; }

        public INamedCommand[] EditorCommands { get; }

        public ProjectController(
            ProjectControl projectControl,
            ProjectViewModel projectViewModel)
        {
            ProjectViewModel = projectViewModel;
            ProjectControl = projectControl;

            CompilationController = new CompilationController(
                projectControl.VisualHost, projectControl.CompilerOutput);

            EditorCommands = new[]
            {
                CutCommand = new NamedCommand("Cut", Cut, () => IsTextSelected),
                CopyCommand = new NamedCommand("Copy", Copy, () => IsTextSelected),
                PasteCommand = new NamedCommand("Paste", Paste),
                DeleteCommand = new NamedCommand("Delete", Delete, () => IsTextSelected),
                ExplainSuccinctlyCommand = new NamedCommand("Explain succinctly", ExplainSuccinctly, () => IsTextSelected),
                ExplainDetailedCommand = new NamedCommand("Explain in detail", ExplainDetailed, () => IsTextSelected),
                SelectAllCommand = new NamedCommand("Select all", SelectAll),
                UndoCommand = new NamedCommand("Undo", Undo, () => CanUndo),
                RedoCommand = new NamedCommand("Redo", Redo, () => CanRedo),
            };

            CodeEditor.TextArea.SelectionChanged += TextArea_SelectionChanged;
            CodeEditor.TextChanged += TextEditor_TextChanged;
            CodeEditor.ContextMenu = CreateContextMenu();
        }

        private void TextEditor_TextChanged(object? sender, EventArgs e)
        {
            CompilationController.Text = CodeEditor.Text;
        }

        public static MenuItem FromCommand(INamedCommand command) =>
            new MenuItem
            {
                Command = command,
                Header = command.Name,
            };

        public ContextMenu CreateContextMenu()
        {
            var cm = new ContextMenu();
            cm.Items.Add(FromCommand(CutCommand));
            cm.Items.Add(FromCommand(CopyCommand));
            cm.Items.Add(FromCommand(PasteCommand));
            cm.Items.Add(new Separator());
            cm.Items.Add(FromCommand(UndoCommand));
            cm.Items.Add(FromCommand(RedoCommand));
            cm.Items.Add(new Separator());
            cm.Items.Add(FromCommand(ExplainDetailedCommand));
            cm.Items.Add(FromCommand(ExplainSuccinctlyCommand));
            return cm;
        }

        private void TextArea_SelectionChanged(object? sender, EventArgs e)
        {
            foreach (var cmd in EditorCommands)
            {
                cmd.NotifyCanExecuteChanged();
            }
        }

        public bool IsTextSelected
            => !CodeEditor.SelectedText.IsNullOrWhiteSpace();

        public void Cut()
            => CodeEditor.Cut();

        public void Copy()
            => CodeEditor.Copy();

        public void Paste()
            => CodeEditor.Paste();

        public void Delete()
            => CodeEditor.Delete();

        public void SelectAll()
            => CodeEditor.SelectAll();
    
        public string SelectedText
            => CodeEditor.SelectedText;

        public bool CanUndo
            => CodeEditor.CanUndo;

        public bool CanRedo
            => CodeEditor.CanRedo;

        public void Undo()
            => CodeEditor.Undo();

        public void Redo()
            => CodeEditor.Redo();

        public async Task ExplainSuccinctly()
        {
            var prompt = $"Explain the code `{SelectedText}` succinctly";
            await ProjectViewModel.SendPromptToChat(prompt);
        }

        public async Task ExplainDetailed()
        {
            var prompt = $"Explain the code `{SelectedText}`";
            await ProjectViewModel.SendPromptToChat(prompt);
        }
    }
}