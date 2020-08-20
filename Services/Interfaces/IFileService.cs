using System.Threading.Tasks;

namespace PertEstimationTool.Services.Interfaces
{
    public interface IFileService<T> where T : class
    {
        Task Save(T data, string fileName, string fileExtension = null);
    }
}
