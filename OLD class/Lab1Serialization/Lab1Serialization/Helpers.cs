using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1Serialization
{
    public static class Helpers
    {
        public static string DecodeUtf8(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        public static byte[] EncodeUtf8(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }


    }
}
