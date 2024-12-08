using System;
using KrIDE.Services;

namespace KrIDE.Tests;

public class TestDataGenerationTests
{
    [Test]
    public void TestGenerateTestData()
    {
        var mongoDbService = new MongoDbService();
        var generator = new TestDataGenerator(mongoDbService);
        
        try
        {
            generator.GenerateTestData();
            Assert.Pass("测试数据生成成功！");
        }
        catch (Exception ex)
        {
            Assert.Fail($"生成测试数据时出错：{ex.Message}");
        }
    }
}
