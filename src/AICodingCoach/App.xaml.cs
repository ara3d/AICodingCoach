﻿using System.Diagnostics;
using System.Windows;
using AICodingCoach.Controllers;
using AICodingCoach.Services;
using AICodingCoach.Views;
using Ara3D.Utils;

namespace AICodingCoach
{
    public partial class App : Application
    {
        public static App Instance => Current as App;
        public static WorkspaceWindow WorkspaceWindow => Current.MainWindow as WorkspaceWindow;
        public static DirectoryPath SourceFolder => PathUtil.GetCallerSourceFolder();
        public static AppService Service => Instance.WorkspaceController.AppService;

        public WorkspaceController WorkspaceController { get; private set; }

        /// <summary>
        /// Called from the main application window once it is created 
        /// </summary>
        public void Initialize()
        {
            WorkspaceController = new WorkspaceController(WorkspaceWindow);
        }
    }
}
