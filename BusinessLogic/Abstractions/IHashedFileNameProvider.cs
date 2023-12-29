namespace BusinessLogic.Abstractions;
public interface IHashedFileNameProvider
{
    string GenerateName(BinaryData file, string initialFileName);
}
