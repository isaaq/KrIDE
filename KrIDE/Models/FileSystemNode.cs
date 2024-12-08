using System;
using MongoDB.Bson;

namespace KrIDE.Models;

public class FileSystemNode
{
    public ObjectId Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDirectory { get; set; }
    public string? Content { get; set; }
    public ObjectId? ParentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
