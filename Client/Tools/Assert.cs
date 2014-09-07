using System;
using System.Diagnostics;
using Silentor.TB.Client.Exceptions;

namespace Silentor.TB.Client.Tools
{
    public static class Assert
    {
        [Conditional("DEBUG")]
        public static void That(bool condition, string message = "Assert failed")
        {
            if (!condition) throw new AssertFailException(message);
        }

        [Conditional("DEBUG")]
        public static void That<T>(T expected, T current, string message = " is not equal to ") where T : IEquatable<T>
        {
            if (!expected.Equals(current)) throw new AssertFailException(expected + message + current);
        }

    }
}


