using Ardalis.Specification.EntityFrameworkCore;
using DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;
public class GenericRepository<T> : RepositoryBase<T>, IRepository<T> where T : class
{
    public GenericRepository(DbContext dbContext) : base(dbContext)
    {
    }
}
