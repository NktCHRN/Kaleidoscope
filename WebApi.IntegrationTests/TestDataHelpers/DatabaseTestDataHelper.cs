using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebApi.IntegrationTests.Serialization;

namespace WebApi.IntegrationTests.TestDataHelpers;
public class DatabaseTestDataHelper
{
    public IReadOnlyList<User> Users { get; }
    public IReadOnlyList<IdentityRole<Guid>> Roles { get; }
    public IReadOnlyList<IdentityUserRole<Guid>> UserRoles { get; }
    public IReadOnlyList<Blog> Blogs { get; }
    public IReadOnlyList<Comment> Comments { get; }
    public IReadOnlyList<PostItem> PostItems { get; }
    public IReadOnlyList<Post> Posts { get; }

    public DatabaseTestDataHelper()
    {
        var deserializeOptions = new JsonSerializerOptions
        {
            TypeInfoResolver = new PostItemTypeResolver(),
            Converters = { new JsonStringEnumConverter() }
        };

        Users = TestDataSerializer.Deserialize<User>("AspNetUsers.json", deserializeOptions);
        Roles = TestDataSerializer.Deserialize<IdentityRole<Guid>>("AspNetRoles.json", deserializeOptions);
        UserRoles = TestDataSerializer.Deserialize<IdentityUserRole<Guid>>("AspNetUserRoles.json", deserializeOptions);
        Blogs = TestDataSerializer.Deserialize<Blog>("Blogs.json", deserializeOptions);
        Comments = TestDataSerializer.Deserialize<Comment>("Comments.json", deserializeOptions);
        PostItems = TestDataSerializer.Deserialize<PostItem>("PostItems.json", deserializeOptions);
        Posts = TestDataSerializer.Deserialize<Post>("Posts.json", deserializeOptions);
    }

    public IEnumerable<object> GetAllEntities()
    {
        return Users.Concat<object>(Roles)
            .Concat(UserRoles)
            .Concat(Blogs)
            .Concat(Comments)
            .Concat(PostItems)
            .Concat(Posts);
    }
}
