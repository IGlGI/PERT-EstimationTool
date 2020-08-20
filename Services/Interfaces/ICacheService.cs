using System.Threading.Tasks;

namespace PertEstimationTool.Services.Interfaces
{
    public interface ICacheService
    {
        Task<T> Get<T>(string key);

        Task Add<T>(string key, T obj);

        Task Upadate<T>(string key, T obj);

        Task Remove(string key);

        Task Clear();
    }
}
