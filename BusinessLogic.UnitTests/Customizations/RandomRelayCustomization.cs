using AutoFixture.Kernel;

namespace BusinessLogic.UnitTests.Customizations;
public class RandomRelayCustomization : ISpecimenBuilder
{
    private readonly List<ISpecimenBuilder> _builders;

    public RandomRelayCustomization(params ISpecimenBuilder[] builders)
        : this(builders.AsEnumerable())
    {
    }

    public RandomRelayCustomization(IEnumerable<ISpecimenBuilder> builders)
    {
        if (builders is null)
        {
            throw new ArgumentNullException(nameof(builders));
        }

        _builders = builders.ToList();
    }

    public object Create(object request, ISpecimenContext context)
    {
        var builderIndex = Random.Shared.Next(_builders.Count);
        var builder = _builders[builderIndex];
        return builder.Create(request, context);
    }
}
