using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using DataAccess.Entities;

namespace WebApi.IntegrationTests.TestDataHelpers;
public class PostItemTypeResolver : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

        Type basePointType = typeof(PostItem);
        if (jsonTypeInfo.Type == basePointType)
        {
            jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "Discriminator",
                IgnoreUnrecognizedTypeDiscriminators = true,
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
                DerivedTypes =
                {
                    new JsonDerivedType(typeof(TextPostItem), "TextPostItem"),
                    new JsonDerivedType(typeof(ImagePostItem), "ImagePostItem")
                }
            };
        }

        return jsonTypeInfo;
    }
}
