using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using TestDataGenerator.Models;
using TestDataGenerator.Services;

namespace TestDataGenerator;

public class Program
{
    private readonly MongoDbService _mongoDbService;
    
    public Program()
    {
        _mongoDbService = new MongoDbService();
    }
    
    public async Task GenerateTestData()
    {
        // 创建根目录
        var rootDir = new FileSystemNode
        {
            Name = "root",
            IsDirectory = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _mongoDbService.CreateNodeAsync(rootDir);
        
        // 创建项目相关目录
        var srcDir = new FileSystemNode
        {
            Name = "src",
            IsDirectory = true,
            ParentId = rootDir.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _mongoDbService.CreateNodeAsync(srcDir);
        
        var testsDir = new FileSystemNode
        {
            Name = "tests",
            IsDirectory = true,
            ParentId = rootDir.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _mongoDbService.CreateNodeAsync(testsDir);
        
        var docsDir = new FileSystemNode
        {
            Name = "docs",
            IsDirectory = true,
            ParentId = rootDir.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _mongoDbService.CreateNodeAsync(docsDir);
        
        // 在 src 目录下创建源代码文件
        var files = new[]
        {
            (Name: "Program.cs", Content: "using System;\n\npublic class Program\n{\n    public static void Main(string[] args)\n    {\n        Console.WriteLine(\"Hello World!\");\n    }\n}"),
            (Name: "Utils.cs", Content: "public static class Utils\n{\n    public static string FormatString(string input)\n    {\n        return input.Trim();\n    }\n}"),
            (Name: "Config.json", Content: "{\n    \"name\": \"MyApp\",\n    \"version\": \"1.0.0\"\n}"),
            (Name: "styles.css", Content: "body {\n    font-family: Arial, sans-serif;\n    margin: 0;\n    padding: 20px;\n}"),
            (Name: "index.html", Content: "<!DOCTYPE html>\n<html>\n<head>\n    <title>My App</title>\n</head>\n<body>\n    <h1>Welcome</h1>\n</body>\n</html>")
        };
        
        foreach (var file in files)
        {
            var fileNode = new FileSystemNode
            {
                Name = file.Name,
                IsDirectory = false,
                ParentId = srcDir.Id,
                Content = file.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _mongoDbService.CreateNodeAsync(fileNode);
        }
        
        // 在 tests 目录下创建测试文件
        var testFiles = new[]
        {
            (Name: "ProgramTests.cs", Content: "using NUnit.Framework;\n\n[TestFixture]\npublic class ProgramTests\n{\n    [Test]\n    public void TestMain()\n    {\n        Assert.Pass();\n    }\n}"),
            (Name: "UtilsTests.cs", Content: "using NUnit.Framework;\n\n[TestFixture]\npublic class UtilsTests\n{\n    [Test]\n    public void TestFormatString()\n    {\n        Assert.AreEqual(\"test\", Utils.FormatString(\" test \"));\n    }\n}"),
            (Name: "test.config", Content: "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n<configuration>\n  <appSettings>\n    <add key=\"testMode\" value=\"true\" />\n  </appSettings>\n</configuration>")
        };
        
        foreach (var file in testFiles)
        {
            var fileNode = new FileSystemNode
            {
                Name = file.Name,
                IsDirectory = false,
                ParentId = testsDir.Id,
                Content = file.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _mongoDbService.CreateNodeAsync(fileNode);
        }
        
        // 在 docs 目录下创建文档文件
        var docFiles = new[]
        {
            (Name: "README.md", Content: "# My Application\n\nThis is a sample application.\n\n## Getting Started\n\n1. Clone the repository\n2. Run `dotnet build`\n3. Run `dotnet run`"),
            (Name: "API.md", Content: "# API Documentation\n\n## Endpoints\n\n- GET /api/v1/users\n- POST /api/v1/users"),
            (Name: "architecture.drawio", Content: "<mxfile>\n  <diagram id=\"sample\" name=\"Sample\">\n    <mxGraphModel><root><mxCell id=\"0\"/></root></mxGraphModel>\n  </diagram>\n</mxfile>"),
            (Name: "requirements.txt", Content: "numpy==1.21.0\npandas==1.3.0\nscikit-learn==0.24.2")
        };
        
        foreach (var file in docFiles)
        {
            var fileNode = new FileSystemNode
            {
                Name = file.Name,
                IsDirectory = false,
                ParentId = docsDir.Id,
                Content = file.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _mongoDbService.CreateNodeAsync(fileNode);
        }
    }
    
    public static async Task Main(string[] args)
    {
        var program = new Program();
        try
        {
            // 清理现有数据
            await program._mongoDbService.ClearAllDataAsync();
            Console.WriteLine("清理现有数据...");
            
            // 生成新数据
            await program.GenerateTestData();
            Console.WriteLine("测试数据生成成功！");
            
            // 显示文件树
            Console.WriteLine("\n文件树结构：");
            var treeDisplay = await program._mongoDbService.GetTreeDisplayAsync(null, "", true);
            Console.WriteLine(treeDisplay);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"生成测试数据时出错：{ex.Message}");
            Environment.Exit(1);
        }
    }
}
