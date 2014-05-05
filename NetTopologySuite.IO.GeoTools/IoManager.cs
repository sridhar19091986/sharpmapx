using Portable.IO;

namespace NetTopologySuite.IO.GeoTools
{
    public static class IoManager
    {
        public static IFile File { get; set; }

        public static void Initialize(IFile fileManager)
        {
            File = fileManager;
        }
    }
}
