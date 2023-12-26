using AutoFixture;
using AutoFixture.Kernel;
using BusinessLogic.Dtos;
using DataAccess.Entities;

namespace BusinessLogic.UnitTests.Customizations;
public class RandomPostItemChildrenCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customizations.Add(
             new FilteringSpecimenBuilder(
                 new RandomRelayCustomization(
                     new TypeRelay(typeof(PostItem), typeof(ImagePostItem)),
                     new TypeRelay(typeof(PostItem), typeof(TextPostItem))),
                 new ExactTypeSpecification(typeof(PostItem))));

        fixture.Customizations.Add(
             new FilteringSpecimenBuilder(
                 new RandomRelayCustomization(
                     new TypeRelay(typeof(PostItemDto), typeof(ImagePostItemDto)),
                     new TypeRelay(typeof(PostItemDto), typeof(TextPostItemDto))),
                 new ExactTypeSpecification(typeof(PostItemDto))));
    }
}
