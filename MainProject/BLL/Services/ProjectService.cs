using BLL.Abstraction.Interfaces;
using Core;
using DAL.Abstraction.Interfaces;
using Task = Core.Task;

namespace BLL;

public class ProjectService : GenericService<Project>, IProjectService
{
    public ProjectService(IRepository<Project> repository) :
        base(repository)
    {
    }
    
    public void CreateProject(Project project)
    {
        try
        {
            Add(project);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create {project.Title}. Exception: {ex.Message}");
        }
    }

    public void UpdateTitle(Guid projectId, string newTitle)
    {
        try
        {
            var project = GetById(projectId);
            if (project is null)
            {
                throw new Exception("Task not found");
            }

            project.Title = newTitle;
            Update(projectId, project);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update {newTitle} in project by {projectId}. Exception: {ex.Message}");
        }
    }

    public void UpdateDescription(Guid projectId, string newDescription)
    {
        try
        {
            var project = GetById(projectId);
            if (project is null)
            {
                throw new Exception("Task not found");
            }

            project.Description = newDescription;
            Update(projectId, project);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update {newDescription} in project by {projectId}. Exception: {ex.Message}");
        }
    }

    public void UpdateTasks(Guid projectId, List<Task> newTasks)
    {
        try
        {
            var project = GetById(projectId);
            if (project is null)
            {
                throw new Exception("Task not found");
            }

            project.Tasks = newTasks;
            Update(projectId, project);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update {newTasks} in project by {projectId}. Exception: {ex.Message}");
        }
    }

    public void DeleteProject(Guid projectId)
    {
        try
        {
            Delete(projectId);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to delete project by {projectId}. Exception: {ex.Message}");
        }
    }

    public List<Project> GetAllProjects()
    {
        try
        {
            return GetAll();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get all projects. Exception: {ex.Message}");
        }
    }

    public List<Project> GetProjectsByTitle(string title)
    {
        try
        {
            return GetAll().Where(p => p.Title.Equals(title)).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get projects by {title}. Exception: {ex.Message}");
        }
    }

    public List<Project> GetProjectsByTask(Task task)
    {
        try
        {
            return GetAll().Where(p => p.Tasks.Contains(task)).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get projects by {task.Title}. Exception: {ex.Message}");
        }
    }
}