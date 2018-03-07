using System.Threading.Tasks;

namespace BetterConfig.ConfigService
{
    internal interface IConfigService
    {
        Task<Config> GetConfigAsync();

        Task RefreshConfigAsync();
    }
}