using System;

namespace XTI_Version
{
    public sealed class PublishVersionException : Exception
    {
        public PublishVersionException(string message) : base(message)
        {
        }
    }
}
