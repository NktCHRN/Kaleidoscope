using Ardalis.Specification.EntityFrameworkCore;
using DataAccess.Abstractions;
using DataAccess.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataAccess.Repositories;
public class GenericRepository<T> : RepositoryBase<T>, IRepository<T> where T : class
{
    private readonly ApplicationDbContext _dbContext;

    public GenericRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return _dbContext.Database.BeginTransactionAsync();
    }
}
