using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TestDataGenerator.Models;

public class FileSystemNode
{
    [BsonId]
    public ObjectId Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public bool IsDirectory { get; set; }
    
    public string? Content { get; set; }
    
    public ObjectId? ParentId { get; set; }
    
    public List<ObjectId> Children { get; set; } = new();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
