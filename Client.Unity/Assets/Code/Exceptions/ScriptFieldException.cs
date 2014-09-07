using System;
using System.Runtime.Serialization;
using Silentor.TB.Client.Exceptions;

namespace Assets.Scripts.Exceptions
{
    [Serializable]
    public class ScriptFieldException : ClientException
    {
        public string FieldName { get; private set; }

        public ScriptFieldException() : base("Some script field is null")
        {
        }

        public ScriptFieldException(string fieldName, string message = "Script field is null") : base(message)
        {
            FieldName = fieldName;
        }

        public ScriptFieldException(string fieldName, string message, Exception inner) : base(message, inner)
        {
            FieldName = fieldName;
        }

        protected ScriptFieldException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}