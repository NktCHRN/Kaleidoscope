using BusinessLogic.Abstractions;
using DataAccess.Persistence;

namespace BusinessLogic.Services;
public class FakeService : IFakeService
{
    private readonly ApplicationDbContext _context;

    public FakeService(ApplicationDbContext context)
    {
        _context = context;
    }
}
