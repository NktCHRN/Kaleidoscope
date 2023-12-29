using AutoFixture;

namespace BusinessLogic.UnitTests.Customizations;
public class DomainCustomization : CompositeCustomization
{
    public DomainCustomization() : base(
        new RandomPostItemChildrenCustomization(),
        new RecursionCustomization())
    {

    }
}
