using System;

namespace GameLiftDemo.UtilException.CLException
{
    public class MissingCommandLineParameterException : Exception
    {
        public MissingCommandLineParameterException()
        {
        }
        public MissingCommandLineParameterException(string message)
            : base("Missing command line argument: " + message)
        {
        }
    }
}