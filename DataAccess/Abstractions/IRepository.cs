using Ardalis.Specification;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataAccess.Abstractions;
public interface IRepository<T> : IRepositoryBase<T> where T: class
{
    Task<IDbContextTransaction> BeginTransactionAsync();
}
