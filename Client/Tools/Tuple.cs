using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Silentor.TB.Client.Tools
{
    public class Tuple<T1, T2>
    {
        public readonly T1 First;
        public readonly T2 Second;

        public Tuple()
        {
            First = default(T1);
            Second = default(T2);
        }

        public Tuple(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }

        public override bool Equals(Object obj)
        {
            var that = obj as Tuple<T1, T2>;

            if (that == null)
            {
                return false;
            }

            return Equals(that);
        }

        public bool Equals(Tuple<T1, T2> that)
        {
            if (that == null)
            {
                return false;
            }

            return object.Equals(First, that.First) && object.Equals(Second, that.Second);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 29 + (First == null ? 0 : First.GetHashCode());
                hash = hash * 29 + (Second == null ? 0 : Second.GetHashCode());
                return hash;
            }
        }
    }
}
