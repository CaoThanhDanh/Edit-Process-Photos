using System;

namespace IOCore.Libs
{
    public class LimitReachedException : Exception
    {
    }

    public class PasswordRequiredException : Exception
    {
    }

    public class IncorrectPasswordException : Exception
    {
    }

    public class PasswordAndConfirmPasswordNotMatchException : Exception
    {
    }

    public class MediaStreamNotFoundException : Exception
    {
    }

    public class UnacceptedInputException : Exception
    {
    }

    public class InputNotFoundException : Exception
    {
    }

    public class MissingLibException : Exception
    {
        public MissingLibException(string message) : base(message) { }
    }
}
