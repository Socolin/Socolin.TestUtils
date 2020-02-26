using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NGit
{
    static class Constants
    {
        public static byte[] EncodeASCII(string s)
        {
            byte[] r = new byte[s.Length];
            for (int k = r.Length - 1; k >= 0; k--)
            {
                char c = s[k];
                if (c > 127)
                {
                    throw new ArgumentException("notASCIIString");
                }
                r[k] = unchecked((byte)c);
            }
            return r;
        }
    }
}
