using System;
using System.Runtime.Serialization;

/*
 * Exceptions that should never be thrown in production
 */
[Serializable]
internal class WTFException : Exception
{
    public WTFException()
    {
    }

    public WTFException(string message) : base(message)
    {
    }

    public WTFException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected WTFException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
