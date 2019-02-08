using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CompareFiles
{
    public static class StringExtensions
    {
        /// <summary>
        /// input nodes
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static byte[] ComputeHash(this string nodes)
        {
            return SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(nodes));
        }
    }
}
