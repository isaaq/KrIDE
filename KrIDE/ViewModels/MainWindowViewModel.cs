using System;
using System.Collections.ObjectModel;
using System.Linq;
using KrIDE.Models;
using KrIDE.Services;
using MongoDB.Bson;
using ReactiveUI;

namespace KrIDE.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly MongoDbService _mongoDbService;
    private ObservableCollection<FileTreeItemViewModel> _rootItems;
    private ObservableCollection<FileTabViewModel> _openFiles;
    private FileTabViewModel? _selectedFile;
    private string _searchText = string.Empty;

    public MainWindowViewModel()
    {
        _mongoDbService = new MongoDbService("mongodb://localhost:27017", "tahe");
        _rootItems = new ObservableCollection<FileTreeItemViewModel>();
        _openFiles = new ObservableCollection<FileTabViewModel>();
        LoadRootItems();
    }

    private void LoadRootItems()
    {
        var rootNodes = _mongoDbService.GetRootNodes();
        RootItems.Clear();
        foreach (var node in rootNodes)
        {
            var viewModel = new FileTreeItemViewModel(node, _mongoDbService);
            viewModel.FileSelected += OnFileSelected;  
            RootItems.Add(viewModel);
        }
    }

    private void OnFileSelected(object? sender, FileTreeItemViewModel e)
    {
        // 检查文件是否已经打开
        var existingTab = OpenFiles.FirstOrDefault(x => x.Id == e.Id);
        if (existingTab != null)
        {
            SelectedFile = existingTab;
            return;
        }

        // 创建新标签页
        var fileTab = new FileTabViewModel(e.Name, e.Content ?? string.Empty, e.Id);
        OpenFiles.Add(fileTab);
        SelectedFile = fileTab;
    }

    public ObservableCollection<FileTreeItemViewModel> RootItems
    {
        get => _rootItems;
        private set => this.RaiseAndSetIfChanged(ref _rootItems, value);
    }

    public ObservableCollection<FileTabViewModel> OpenFiles
    {
        get => _openFiles;
        private set => this.RaiseAndSetIfChanged(ref _openFiles, value);
    }

    public string SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }

    public FileTabViewModel? SelectedFile
    {
        get => _selectedFile;
        set => this.RaiseAndSetIfChanged(ref _selectedFile, value);
    }

    public void CreateFile(string name, bool isDirectory, string? content = null)
    {
        var node = new FileSystemNode
        {
            Name = name,
            IsDirectory = isDirectory,
            Content = content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mongoDbService.CreateNode(node);
        LoadRootItems();
    }
}

public class FileTabViewModel : ViewModelBase
{
    private string _name;
    private string _content;
    private bool _isSelected;
    private ObjectId _id;

    public FileTabViewModel(string name, string content, ObjectId id)
    {
        _name = name;
        _content = content;
        _id = id;
    }

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public string Content
    {
        get => _content;
        set => this.RaiseAndSetIfChanged(ref _content, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => this.RaiseAndSetIfChanged(ref _isSelected, value);
    }

    public ObjectId Id => _id;
}