using Ardalis.Specification.EntityFrameworkCore;
using DataAccess.Abstractions;
using DataAccess.Persistence;

namespace DataAccess.Repositories;
public class GenericRepository<T> : RepositoryBase<T>, IRepository<T> where T : class
{
    public GenericRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
