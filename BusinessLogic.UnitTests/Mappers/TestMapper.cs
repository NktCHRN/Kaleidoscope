using AutoMapper;
using System.Reflection;

namespace BusinessLogic.UnitTests.Mappers;
public static class TestMapper
{
    public readonly static IMapper Mapper = 
        new Mapper(
            new MapperConfiguration(
                cfg => cfg.AddMaps(Assembly.GetAssembly(typeof(IBusinessLogicMarker)))));
}
