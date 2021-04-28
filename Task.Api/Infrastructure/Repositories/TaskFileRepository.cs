using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Task.Api.Infrastructure.Contexts;
using Task.Api.Infrastructure.Models;

namespace Task.Api.Infrastructure.Repositories
{
    public class TaskFileRepository : ITaskFileRepository, IDisposable
    {
        private readonly TaskContext _taskContext;

        public TaskFileRepository(TaskContext taskContext)
        {
            _taskContext = taskContext;
        }

        public async System.Threading.Tasks.Task InsertAsync(TaskFile entity, CancellationToken token = default)
        {
            await _taskContext.TextFiles.AddAsync(entity, token).ConfigureAwait(false);
            await _taskContext.SaveChangesAsync(token).ConfigureAwait(false);
        }

        public Task<TaskFile[]> GetByTaskIdAsync(object id, CancellationToken token = default)
        {
            return _taskContext.TextFiles.Where(t => t.TaskId == Guid.Parse(id.ToString())).ToArrayAsync(cancellationToken: token);
        }


        public void Dispose()
        {
            _taskContext?.Dispose();
        }
    }
}
