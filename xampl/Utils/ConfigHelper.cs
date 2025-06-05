#nullable disable
namespace xampl.Utils
{
    public static class ConfigHelper
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
    }

}
