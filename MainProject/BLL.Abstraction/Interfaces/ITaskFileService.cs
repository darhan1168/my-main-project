using Core;
using Task = System.Threading.Tasks.Task;

namespace BLL.Abstraction.Interfaces;

public interface ITaskFileService : IGenericService<TaskFile>
{
    Task CreateFile(TaskFile file);
}