using BusinessLogic.Abstractions;
using System.Security.Cryptography;

namespace BusinessLogic.Providers;
public class HashedFileNameProvider : IHashedFileNameProvider
{
    public string GenerateName(BinaryData file, string initialFileName)
    {
        var extension = Path.GetExtension(initialFileName);
        using var stream = file.ToStream();
        var hash = SHA256.HashData(stream);
        return $"{Convert.ToBase64String(hash).Replace('/', '-')}{extension}";      // Slashed are currently forbidden
                                                                                    // or treated as separator between folder name and file name.
    }
}
