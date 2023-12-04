﻿using System.ComponentModel;
using System.Windows.Input;
using AICodingCoach.Models;
using Ara3D.Domo;
using Ara3D.Utils;

namespace AICodingCoach.ViewModels
{
    public class ChatMessageViewModel : INotifyPropertyChanged, IDisposable
    {
        public IModel<MessageData> Model { get; }
        public ChatViewModel Parent { get; }
        public bool IsEmpty => Model.Value.Text.IsNullOrWhiteSpace();

        public ICommand CopyCodeCommand => Parent.ProjectViewModel.CopyCodeCommand;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ChatMessageViewModel(IModel<MessageData> model, ChatViewModel parent)
        {
            Model = model;
            Parent = parent;
        }

        public void Dispose()
        {
            PropertyChanged = null;
        }

        public string Text
        {
            get
            {
                var tmp = Model.Value.Text.Trim();
                if (tmp.StartsWith("```"))
                {
                    var index = tmp.IndexOf('\n');
                    tmp = tmp.Substring(index + 1);
                }

                var end = tmp.LastIndexOf("```", StringComparison.Ordinal);
                if (end > 0)
                {
                    tmp = tmp.Substring(0, end).Trim();
                }

                return tmp;
            }
            // ReSharper disable once ValueParameterNotUsed
            set
            { }
        }
    }
}