using System.Threading;
using System.Threading.Tasks;
using Text.Api.Infrastructure.Models;

namespace Text.Api.Infrastructure.Repositories
{
    public interface ITextFileRepository
    {
        Task InsertAsync(TextFile entity, CancellationToken token = default);

        ValueTask<TextFile> GetByIdAsync(object id, CancellationToken token = default);

        Task<TextFile[]> GetAllAsync(CancellationToken token = default);
    }
}
