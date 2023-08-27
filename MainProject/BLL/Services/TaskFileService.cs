using BLL.Abstraction.Interfaces;
using Core;
using DAL.Abstraction.Interfaces;
using Microsoft.AspNetCore.Http;
using Task = System.Threading.Tasks.Task;

namespace BLL;

public class TaskFileService : GenericService<TaskFile>, ITaskFileService
{
    public TaskFileService(IRepository<TaskFile> repository) : base(repository)
    {
    }

    public async Task<TaskFile> CreateFile(IFormFile modelFile)
    {
        try
        {
            if (modelFile == null && modelFile.Length <= 0)
            {
                throw new Exception("Model file not found");
            }
            
            var file = new TaskFile
            {
                FileName = modelFile.FileName,
                FileData = new byte[modelFile.Length],
                CreationDate = DateTime.Now
            };
            
            using (var stream = new MemoryStream())
            {
                await modelFile.CopyToAsync(stream);
                file.FileData = stream.ToArray();
            }
            
            await Add(file);

            return file;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create {modelFile.FileName}. Exception: {ex.Message}", ex);
        }
    }
}