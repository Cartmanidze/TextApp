using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Text.Api.Infrastructure.Contexts;
using Text.Api.Infrastructure.Models;

namespace Text.Api.Infrastructure.Repositories
{
    public class TextFileRepository : ITextFileRepository, IDisposable
    {
        private readonly TextContext _textContext;

        public TextFileRepository(TextContext textContext)
        {
            _textContext = textContext;
        }

        public async Task InsertAsync(TextFile entity, CancellationToken token = default)
        {
            await _textContext.TextFiles.AddAsync(entity, token).ConfigureAwait(false);
            await _textContext.SaveChangesAsync(token).ConfigureAwait(false);
        }

        public ValueTask<TextFile> GetByIdAsync(object id, CancellationToken token = default)
        {
            return _textContext.TextFiles.FindAsync(new[] {id}, token);
        }

        public Task<TextFile[]> GetAllAsync(CancellationToken token = default)
        {
            return _textContext.TextFiles.ToArrayAsync(token);
        }

        public void Dispose()
        {
            _textContext?.Dispose();
        }
    }
}
