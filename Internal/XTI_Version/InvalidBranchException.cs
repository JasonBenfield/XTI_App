using System;

namespace XTI_Version
{
    public sealed class InvalidBranchException : Exception
    {
        public InvalidBranchException(string branchName)
            : base($"Branch name \"{branchName}\" is not valid")
        {
        }
    }
}
