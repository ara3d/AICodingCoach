﻿using System.Windows.Controls;

namespace CodingCanvasWpfApp
{
    /// <summary>
    /// Interaction logic for ChatMessagesControl.xaml
    /// </summary>
    public partial class ChatMessagesControl : UserControl
    {
        public ChatMessagesViewModel ViewModel { get; } = new();

        public ChatMessagesControl()
        {
            InitializeComponent();
        }
    }
}