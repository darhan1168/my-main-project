using Core;
using Microsoft.AspNetCore.Http;
using Task = System.Threading.Tasks.Task;

namespace BLL.Abstraction.Interfaces;

public interface ITaskFileService : IGenericService<TaskFile>
{
    Task<TaskFile> CreateFile(IFormFile modelFile);
}