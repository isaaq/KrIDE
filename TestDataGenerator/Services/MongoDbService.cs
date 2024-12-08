using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading.Tasks;
using System.Collections.Generic;
using TestDataGenerator.Models;
using System;
using System.Linq;

namespace TestDataGenerator.Services;

public class MongoDbService
{
    private readonly IMongoCollection<FileSystemNode> _filesCollection;

    public MongoDbService()
    {
        var connectionString = "mongodb://localhost:27017";
        var databaseName = "kride";
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _filesCollection = database.GetCollection<FileSystemNode>("sys_files");
    }

    public async Task<List<FileSystemNode>> GetChildrenAsync(ObjectId? parentId)
    {
        var filter = Builders<FileSystemNode>.Filter.Eq(x => x.ParentId, parentId);
        return await _filesCollection.Find(filter).ToListAsync();
    }

    public async Task<FileSystemNode?> GetNodeByIdAsync(ObjectId id)
    {
        var filter = Builders<FileSystemNode>.Filter.Eq(x => x.Id, id);
        return await _filesCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<List<FileSystemNode>> GetFullTreeAsync(ObjectId? parentId = null)
    {
        var nodes = new List<FileSystemNode>();
        var children = await GetChildrenAsync(parentId);
        
        foreach (var child in children.OrderBy(x => !x.IsDirectory).ThenBy(x => x.Name))
        {
            nodes.Add(child);
            if (child.IsDirectory)
            {
                var subNodes = await GetFullTreeAsync(child.Id);
                nodes.AddRange(subNodes);
            }
        }
        
        return nodes;
    }

    public async Task<string> GetTreeDisplayAsync(ObjectId? parentId = null, string indent = "", bool isLast = true)
    {
        var result = "";
        var children = await GetChildrenAsync(parentId);
        children = children.OrderBy(x => !x.IsDirectory).ThenBy(x => x.Name).ToList();

        for (int i = 0; i < children.Count; i++)
        {
            var child = children[i];
            var isLastChild = i == children.Count - 1;
            var prefix = isLast ? "└── " : "├── ";
            var newIndent = indent + (isLast ? "    " : "│   ");

            result += indent + prefix + child.Name + "\n";

            if (child.IsDirectory)
            {
                result += await GetTreeDisplayAsync(child.Id, newIndent, isLastChild);
            }
        }

        return result;
    }

    public async Task CreateNodeAsync(FileSystemNode node)
    {
        await _filesCollection.InsertOneAsync(node);
        if (node.ParentId.HasValue)
        {
            var parent = await GetNodeByIdAsync(node.ParentId.Value);
            if (parent != null)
            {
                parent.Children.Add(node.Id);
                var update = Builders<FileSystemNode>.Update.Set(x => x.Children, parent.Children);
                await _filesCollection.UpdateOneAsync(x => x.Id == parent.Id, update);
            }
        }
    }

    public async Task ClearAllDataAsync()
    {
        await _filesCollection.DeleteManyAsync(Builders<FileSystemNode>.Filter.Empty);
    }
}
