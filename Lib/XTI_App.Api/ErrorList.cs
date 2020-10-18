using System.Collections.Generic;
using System.Linq;

namespace XTI_App.Api
{
    public sealed class ErrorList
    {
        private readonly List<ErrorModel> errors = new List<ErrorModel>();

        public void Add(string message, string source = null) => Add(new ErrorModel(message, source));

        public void Add(ErrorModel error) => errors.Add(error);

        public bool Any() => errors.Count > 0;

        public IEnumerable<ErrorModel> Errors() => errors.ToArray();

        public override string ToString()
        {
            var joinedErrors = string.Join("\r\n", errors.Select(e => e.Message));
            return $"{nameof(ErrorList)}\r\n{joinedErrors}";
        }
    }
}
