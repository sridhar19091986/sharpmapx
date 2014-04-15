using System.IO;

namespace Portable.IO
{
    public static class Extensions
    {
        public static void Close(this BinaryReader value)
        {
            value.Dispose();
        }

        public static void Close(this BinaryWriter value)
        {
            value.Dispose();
        }

        public static void Close(this Stream value)
        {
            value.Dispose();
        }
    }
}
