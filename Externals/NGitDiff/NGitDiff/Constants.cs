using System;
using System.Collections.Generic;
using System.Text;

namespace NGit
{
    static class Constants
    {
        public static readonly string CHARACTER_ENCODING = "UTF-8";
        public static readonly Encoding CHARSET;

        static Constants()
        {
            CHARSET = Sharpen.Extensions.GetEncoding(CHARACTER_ENCODING);
        }
    }
}
