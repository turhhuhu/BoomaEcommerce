using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace BoomaEcommerce.Data.EfCore
{
    public class TransactionContext : ITransactionContext
    {
        private readonly IDbContextTransaction _transaction;

        public TransactionContext(IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public Task CommitAsync()
        { 
            return _transaction.CommitAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            return _transaction.DisposeAsync();
        }
    }

    public interface ITransactionContext : IAsyncDisposable, IDisposable
    {
        public Task CommitAsync();
    }
}
