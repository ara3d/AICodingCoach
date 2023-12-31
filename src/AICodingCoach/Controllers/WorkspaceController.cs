﻿using System.Collections.ObjectModel;
using AICodingCoach.Models;
using AICodingCoach.Services;
using AICodingCoach.Utilities;
using AICodingCoach.ViewModels;
using AICodingCoach.Views;
using Ara3D.Domo;

namespace AICodingCoach.Controllers;

/// <summary>
/// Acts as a factory and coordinator for project controllers and hosts the main services
/// and view model. This is create by the main App class, after the main window is created. 
/// </summary>
public class WorkspaceController
{
    public ObservableCollection<ProjectViewModel> ProjectViewModels { get; } = new();
    public AppService AppService { get; }

    public WorkspaceWindow WorkspaceWindow { get; }
    public WorkspaceService WorkspaceService => AppService.WorkspaceService;

    public WorkspaceController(WorkspaceWindow workspaceWindow)
    {
        WorkspaceWindow = workspaceWindow ?? throw new Exception("Workspace workspace must be created");

        // Start the main app service
        AppService = new AppService();
        
        // Set the items source for the project tab control 
        WorkspaceWindow.ProjectTabControl.ItemsSource = ProjectViewModels;

        // Register for notifications regarding new projects.
        // When a project is added or removed, we update the view models. 
        WorkspaceService.Repository.RepositoryChanged += ProjectsRepositoryChanged;

        // Create an initial project 
        var project = WorkspaceService.NewProject("First");
        project.SetCode("// Hello world!");
    }

    private void ProjectsRepositoryChanged(object? sender, RepositoryChangeArgs e)
    {
        ProjectViewModels.SynchronizeObservableCollection(
            WorkspaceService.Repository, 
            model => new ProjectViewModel(new ProjectService(model)),
            vm => vm.Model.Id,
            vm => vm.Dispose());
    }

}