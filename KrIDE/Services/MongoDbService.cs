using MongoDB.Driver;
using KrIDE.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KrIDE.Services;

public class MongoDbService
{
    private readonly IMongoCollection<FileSystemNode> _filesCollection;

    public MongoDbService(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _filesCollection = database.GetCollection<FileSystemNode>("files");

        // 确保根目录存在
        EnsureRootDirectory();
    }

    public MongoDbService() : this("mongodb://localhost:27017", "tahe")
    {
    }

    private void EnsureRootDirectory()
    {
        var root = _filesCollection.Find(x => x.ParentId == null).FirstOrDefault();
        if (root == null)
        {
            root = new FileSystemNode
            {
                Id = ObjectId.GenerateNewId(),
                Name = "/",
                IsDirectory = true,
                ParentId = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _filesCollection.InsertOne(root);
        }
    }

    public List<FileSystemNode> GetRootNodes()
    {
        return GetChildren(null);
    }

    public List<FileSystemNode> GetChildren(ObjectId? parentId)
    {
        return _filesCollection.Find(x => x.ParentId == parentId)
            .ToList()
            .OrderByDescending(x => x.IsDirectory)
            .ThenBy(x => x.Name)
            .ToList();
    }

    public void CreateNode(FileSystemNode node)
    {
        node.CreatedAt = DateTime.UtcNow;
        node.UpdatedAt = DateTime.UtcNow;
        _filesCollection.InsertOne(node);
    }

    public void UpdateNode(ObjectId id, string content)
    {
        var update = Builders<FileSystemNode>.Update
            .Set(x => x.Content, content)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);
        _filesCollection.UpdateOne(x => x.Id == id, update);
    }

    public void DeleteNode(ObjectId id)
    {
        // 递归删除所有子节点
        var children = GetChildren(id);
        foreach (var child in children)
        {
            DeleteNode(child.Id);
        }
        _filesCollection.DeleteOne(x => x.Id == id);
    }
}
