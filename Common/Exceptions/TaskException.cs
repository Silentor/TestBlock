using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Silentor.TB.Common.Exceptions
{
    [Serializable]
    public class TaskException : CommonException
    {
        public TaskException() { }
        public TaskException(string message) : base(message) { }
        public TaskException(string message, Exception inner) : base(message, inner) { }
        protected TaskException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
