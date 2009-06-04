using System;

namespace MyLife
{
    public class MyLifeException : ApplicationException
    {
        public MyLifeException()
        {
        }

        public MyLifeException(string message) : base(message)
        {
        }
    }
}