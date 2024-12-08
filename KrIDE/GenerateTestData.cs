using System;
using KrIDE.Services;

namespace KrIDE;

public class GenerateTestData
{
    public static void Generate()
    {
        var mongoDbService = new MongoDbService();
        var generator = new TestDataGenerator(mongoDbService);
        
        try
        {
            generator.GenerateTestData();
            Console.WriteLine("测试数据生成成功！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"生成测试数据时出错：{ex.Message}");
        }
    }
}
