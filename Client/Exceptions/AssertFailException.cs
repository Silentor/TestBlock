using System;

namespace Silentor.TB.Client.Exceptions
{
    [Serializable]
    public class AssertFailException : ClientException
    {
        public AssertFailException() { }
        public AssertFailException(string message) : base(message) { }
        public AssertFailException(string message, Exception inner) : base(message, inner) { }
        protected AssertFailException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
