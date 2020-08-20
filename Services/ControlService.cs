using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Unity;
using PertEstimationTool.Models;
using PertEstimationTool.Services.Interfaces;

namespace PertEstimationTool.Services
{
    public class ControlService : IControlService
    {
        private IUnityContainer _container;

        private ICacheService _cacheService;

        public ControlService(IUnityContainer container)
        {
            _container = container;
            _cacheService = container.Resolve<ICacheService>();
        }

        public async Task<T> Get<T>(string cacheKey)
        {
            if (string.IsNullOrEmpty(cacheKey))
                throw new Exception("The key cannot be null or empty");

            var item = await _cacheService.Get<T>(cacheKey);

            if (item == null)
            {
                item = _container.Resolve<T>(cacheKey);

                if (item == null)
                    throw new Exception("The requested data wasn't found.");

                await _cacheService.Upadate<T>(cacheKey, item);
            }

            return item;
        }

        public async Task<T> Reset<T>(string key) where T : class, new()
        {
            if (string.IsNullOrEmpty(key))
                throw new Exception("The key cannot be null or empty");

            var item = await Get<T>(key);

            if (item != null)
            {
                item = new T();

                await _cacheService.Remove(key);
                _container.RegisterInstance<T>(key, item);
                return item;
            }

            return null;
        }

        public async Task<Guid> GenerateId()
        {
            var guid = Guid.NewGuid();
            var tasksItems = await Get<ObservableCollection<TaskItem>>(nameof(ObservableCollection<TaskItem>));

            if (tasksItems != null && tasksItems.Any())
            {
                var found = new TaskItem();

                do
                {
                    found = tasksItems?.FirstOrDefault(x => x.Id == guid);
                    guid = Guid.NewGuid();

                } while (found != null);

                return guid;
            }

            return guid;
        }

    }
}
