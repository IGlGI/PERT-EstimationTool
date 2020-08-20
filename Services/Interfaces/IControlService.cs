using PertEstimationTool.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PertEstimationTool.Services.Interfaces
{
    public interface IControlService
    {
        Task<T> Reset<T>(string key) where T : class, new();

        Task<T> Get<T>(string cacheKey);

        Task<Guid> GenerateId();
    }
}
