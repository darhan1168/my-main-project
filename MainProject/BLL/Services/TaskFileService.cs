using BLL.Abstraction.Interfaces;
using Core;
using DAL.Abstraction.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace BLL;

public class TaskFileService : GenericService<TaskFile>, ITaskFileService
{
    public TaskFileService(IRepository<TaskFile> repository) : base(repository)
    {
    }

    public async Task CreateFile(TaskFile file)
    {
        try
        {
            await Add(file);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create {file.FileName}. Exception: {ex.Message}", ex);
        }
    }
}