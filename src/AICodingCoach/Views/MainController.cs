using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AICodingCoach.Services;
using AICodingCoach.ViewModels;
using Ara3D.Utils;
using ICSharpCode.AvalonEdit;

namespace AICodingCoach.Views;

public class MainController
{
    public TextEditor TextEditor { get; }
    public ChatViewModel ChatViewModel { get; }
    public ChatService ChatService { get; }
    
    public INamedCommand CutCommand { get; }
    public INamedCommand CopyCommand { get; }
    public INamedCommand PasteCommand { get; }
    public INamedCommand DeleteCommand { get; }
    public INamedCommand ExplainDetailedCommand { get; }
    public INamedCommand ExplainSuccinctlyCommand { get; }
    public INamedCommand SelectAllCommand { get; }
    public INamedCommand UndoCommand { get; }
    public INamedCommand RedoCommand { get; }

    public INamedCommand[] Commands { get; }

    public MainController(TextEditor textEditor, ChatViewModel chatViewModel, ChatService chatService)
    {
        TextEditor = textEditor;
        ChatViewModel = chatViewModel;
        ChatService = chatService;

        Commands = new[]
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

        TextEditor.TextArea.SelectionChanged += TextArea_SelectionChanged;
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
        foreach (var cmd in Commands)
        {
            cmd.NotifyCanExecuteChanged();
        }
    }

    public bool IsTextSelected
        => !TextEditor.SelectedText.IsNullOrWhiteSpace();

    public void Cut()
        => TextEditor.Cut();

    public void Copy()
        => TextEditor.Copy();

    public void Paste()
        => TextEditor.Paste();

    public void Delete()
        => TextEditor.Delete();

    public void SelectAll()
        => TextEditor.SelectAll();
    
    public string SelectedText
        => TextEditor.SelectedText;

    public bool CanUndo
        => TextEditor.CanUndo;

    public bool CanRedo
        => TextEditor.CanRedo;

    public void Undo()
        => TextEditor.Undo();

    public void Redo()
        => TextEditor.Redo();

    public async Task ExplainSuccinctly()
    {
        var prompt = $"Explain the code `{SelectedText}` succinctly";
        ChatViewModel.AppendUserText(prompt);
        await ChatService.SendPromptAsync(prompt, (_, s) => ChatViewModel.AppendNonUserText(s));
    }

    public async Task ExplainDetailed()
    {
        var prompt = $"Explain the code `{SelectedText}`";
        ChatViewModel.AppendUserText(prompt);
        await ChatService.SendPromptAsync(prompt, (_, s) => ChatViewModel.AppendNonUserText(s));
    }
}