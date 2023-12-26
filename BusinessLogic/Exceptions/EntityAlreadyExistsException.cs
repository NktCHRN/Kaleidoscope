namespace BusinessLogic.Exceptions;
[Serializable]
public class EntityAlreadyExistsException : Exception
{
    public EntityAlreadyExistsException(string message): base(message) { }

    public EntityAlreadyExistsException(string message, Exception inner) : base(message, inner) { }
}
