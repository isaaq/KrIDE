using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using KrIDE.Models;

namespace KrIDE.Services;

public class ConfigurationService
{
    private static ConfigurationService? _instance;
    private readonly string _configPath;
    private MongoDbConfig? _mongoDbConfig;

    private ConfigurationService()
    {
        _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
    }

    public static ConfigurationService Instance
    {
        get
        {
            _instance ??= new ConfigurationService();
            return _instance;
        }
    }

    public MongoDbConfig GetMongoDbConfig()
    {
        if (_mongoDbConfig != null)
            return _mongoDbConfig;

        try
        {
            if (!File.Exists(_configPath))
            {
                _mongoDbConfig = new MongoDbConfig
                {
                    ConnectionString = "mongodb://localhost:27017",
                    DatabaseName = "KrIDE"
                };
                SaveConfig();
                return _mongoDbConfig;
            }

            var jsonString = File.ReadAllText(_configPath);
            var config = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonString);
            
            if (config != null && config.TryGetValue("MongoDB", out var mongoSection))
            {
                _mongoDbConfig = JsonSerializer.Deserialize<MongoDbConfig>(mongoSection.ToString());
            }
        }
        catch (Exception ex)
        {
            // 如果读取配置失败，使用默认值
            Console.WriteLine($"Error reading configuration: {ex.Message}");
            _mongoDbConfig = new MongoDbConfig
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "KrIDE"
            };
        }

        _mongoDbConfig ??= new MongoDbConfig
        {
            ConnectionString = "mongodb://localhost:27017",
            DatabaseName = "KrIDE"
        };

        return _mongoDbConfig;
    }

    private void SaveConfig()
    {
        try
        {
            var config = new Dictionary<string, object>
            {
                ["MongoDB"] = _mongoDbConfig!
            };

            var jsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(_configPath, jsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving configuration: {ex.Message}");
        }
    }
}
