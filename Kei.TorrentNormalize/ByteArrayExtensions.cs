using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kei.TorrentNormalize
{
    public static class ByteArrayExtensions
    {

        public static bool StartsWith(this byte[] b, byte[] compare)
        {
            if (b == null)
            {
                throw new ArgumentNullException("b");
            }
            if (compare == null)
            {
                throw new ArgumentNullException("compare");
            }
            if (b.Length < compare.Length)
            {
                return false;
            }
            for (var i = 0; i < compare.Length; i++)
            {
                if (b[i] != compare[i])
                {
                    return false;
                }
            }
            return true;
        }

    }
}
