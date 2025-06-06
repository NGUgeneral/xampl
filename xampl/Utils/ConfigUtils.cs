#nullable disable
using dotenv.net;

namespace xampl.Utils
{
    public static class ConfigUtils
    {
        public static IConfiguration Configuration { get; private set; }

        public static void Initialize(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static string GetSetting(string key)
        {
            return Configuration[key];
        }

        public static void LoadAndReplaceEnvironmentVariables(IConfigurationBuilder configBuilder)
        {
            DotEnv.Load();
            configBuilder
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);

            var tempConfig = configBuilder.Build();
            var configDictionary = new Dictionary<string, string>();
            foreach (var kvp in tempConfig.AsEnumerable())
            {
                if (kvp.Value != null && kvp.Value.StartsWith("${") && kvp.Value.EndsWith('}'))
                {
                    var envVarValue = Environment.GetEnvironmentVariable(kvp.Value[2..^1]);
                    if (!string.IsNullOrEmpty(envVarValue))
                    {
                        configDictionary[kvp.Key] = envVarValue;
                    }
                }
            }
            configBuilder.AddInMemoryCollection(configDictionary);
        }
    }
}
