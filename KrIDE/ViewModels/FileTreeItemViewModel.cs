using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using KrIDE.Models;
using KrIDE.Services;
using MongoDB.Bson;
using ReactiveUI;

namespace KrIDE.ViewModels;

public class FileTreeItemViewModel : ReactiveObject
{
    private readonly FileSystemNode _node;
    private readonly MongoDbService _mongoDbService;
    private bool _isExpanded;
    private ObservableCollection<FileTreeItemViewModel>? _children;

    public event EventHandler<FileTreeItemViewModel>? FileSelected;

    public FileTreeItemViewModel(FileSystemNode node, MongoDbService mongoDbService)
    {
        _node = node;
        _mongoDbService = mongoDbService;
        _isExpanded = true;  // 设置默认展开
        
        if (IsDirectory)
        {
            _children = new ObservableCollection<FileTreeItemViewModel>();
            LoadChildren();
        }

        OpenCommand = ReactiveCommand.Create(ExecuteOpen);
    }

    public ICommand OpenCommand { get; }

    private void ExecuteOpen()
    {
        if (!IsDirectory)
        {
            FileSelected?.Invoke(this, this);
        }
    }

    public string Name => _node.Name;
    public bool IsDirectory => _node.IsDirectory;
    public ObjectId Id => _node.Id;
    public string? Content 
    { 
        get => _node.Content; 
        set => _node.Content = value; 
    }

    public bool IsExpanded
    {
        get => _isExpanded;
        set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
    }

    public ObservableCollection<FileTreeItemViewModel>? Children
    {
        get => _children;
        set => this.RaiseAndSetIfChanged(ref _children, value);
    }

    private void LoadChildren()
    {
        if (!IsDirectory) return;
        
        _children ??= new ObservableCollection<FileTreeItemViewModel>();
        var childNodes = _mongoDbService.GetChildren(_node.Id);
        _children.Clear();
        foreach (var node in childNodes)
        {
            var viewModel = new FileTreeItemViewModel(node, _mongoDbService);
            viewModel.FileSelected += (s, e) => FileSelected?.Invoke(s, e);
            _children.Add(viewModel);
        }
    }
}
