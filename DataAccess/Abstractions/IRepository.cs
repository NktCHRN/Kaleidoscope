using Ardalis.Specification;

namespace DataAccess.Abstractions;
public interface IRepository<T> : IRepositoryBase<T> where T: class
{
}
