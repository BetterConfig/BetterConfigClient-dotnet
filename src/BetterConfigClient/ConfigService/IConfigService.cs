using System.Threading.Tasks;

namespace BetterConfig.ConfigService
{
    internal interface IConfigService
    {
        Task<ProjectConfig> GetConfigAsync();

        Task RefreshConfigAsync();
    }
}