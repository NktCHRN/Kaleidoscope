using System.Text.Json.Serialization;

namespace WebApi.Models.Responses.Post;

[JsonDerivedType(typeof(ImagePostItemResponse), typeDiscriminator:"image")]
[JsonDerivedType(typeof(TextPostItemResponse), typeDiscriminator:"text")]
public abstract record PostItemResponse
{
    public Guid Id { get; set; }
}
