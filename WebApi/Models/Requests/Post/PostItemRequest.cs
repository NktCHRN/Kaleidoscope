using System.Text.Json.Serialization;

namespace WebApi.Models.Requests.Post;

[JsonDerivedType(typeof(ImagePostItemRequest), typeDiscriminator: "image")]
[JsonDerivedType(typeof(TextPostItemRequest), typeDiscriminator: "text")]
public abstract record PostItemRequest
{
    public Guid? Id { get; set; }
}
