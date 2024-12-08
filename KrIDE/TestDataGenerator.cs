using System;
using KrIDE.Models;
using KrIDE.Services;
using MongoDB.Bson;

namespace KrIDE;

public class TestDataGenerator
{
    private readonly MongoDbService _mongoDbService;

    public TestDataGenerator(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }

    public void GenerateTestData()
    {
        // 创建根目录
        var rootDir = new FileSystemNode
        {
            Name = "root",
            IsDirectory = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _mongoDbService.CreateNode(rootDir);

        // 创建子目录
        var srcDir = new FileSystemNode
        {
            Name = "src",
            IsDirectory = true,
            ParentId = rootDir.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _mongoDbService.CreateNode(srcDir);

        var testsDir = new FileSystemNode
        {
            Name = "tests",
            IsDirectory = true,
            ParentId = rootDir.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _mongoDbService.CreateNode(testsDir);

        // 在 src 目录下创建一些文件
        var mainFile = new FileSystemNode
        {
            Name = "main.cs",
            IsDirectory = false,
            ParentId = srcDir.Id,
            Content = "public class Program { public static void Main() { } }",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _mongoDbService.CreateNode(mainFile);

        var utilsFile = new FileSystemNode
        {
            Name = "utils.cs",
            IsDirectory = false,
            ParentId = srcDir.Id,
            Content = "public static class Utils { public static string FormatString(string input) { return input.Trim(); } }",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _mongoDbService.CreateNode(utilsFile);

        // 在 tests 目录下创建测试文件
        var mainTestFile = new FileSystemNode
        {
            Name = "main_test.cs",
            IsDirectory = false,
            ParentId = testsDir.Id,
            Content = "public class ProgramTests { public void TestMain() { Assert.Pass(); } }",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _mongoDbService.CreateNode(mainTestFile);
    }
}
