using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Silentor.TB.Common.Exceptions
{
    [Serializable]
    public class ChunkException : CommonException
    {
        public ChunkException() { }
        public ChunkException(string message) : base(message) { }
        public ChunkException(string message, Exception inner) : base(message, inner) { }
        protected ChunkException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
