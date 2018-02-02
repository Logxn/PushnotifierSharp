using System;
using System.Runtime.Serialization;


namespace PushNotfierSharp
{

    [Serializable]
    public class WrongCredentialsException : Exception
    {
        // Constructors
        public WrongCredentialsException(string message)
            : base(message)
        { }

        // Ensure Exception is Serializable
        protected WrongCredentialsException(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        { }
    }

    [Serializable]
    public class NotLoggedInException : Exception
    {
        // Constructors
        public NotLoggedInException(string message)
            : base(message)
        { }

        // Ensure Exception is Serializable
        protected NotLoggedInException(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        { }
    }

    [Serializable]
    public class BadRequestException : Exception
    {
        // Constructors
        public BadRequestException(string message)
            : base(message)
        { }

        // Ensure Exception is Serializable
        protected BadRequestException(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        { }
    }

    [Serializable]
    public class DeviceNotFoundException : Exception
    {
        // Constructors
        public DeviceNotFoundException(string message)
            : base(message)
        { }

        // Ensure Exception is Serializable
        protected DeviceNotFoundException(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        { }
    }
}
