using System.Threading;
using System.Threading.Tasks;
using Task.Api.Infrastructure.Models;

namespace Task.Api.Infrastructure.Repositories
{
    public interface ITaskFileRepository
    {
        System.Threading.Tasks.Task InsertAsync(TaskFile entity, CancellationToken token = default);

        Task<TaskFile[]> GetByTaskIdAsync(object id, CancellationToken token = default);
    }
}
