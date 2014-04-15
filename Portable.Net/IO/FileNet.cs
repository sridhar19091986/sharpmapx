using System.IO;

namespace Portable.IO
{
    public class FileNet: IFile
    {
        public bool FileExists(string filename)
        {
            return File.Exists(filename);
        }

        public string ChangeExtension(string filename, string extension)
        {
            return Path.ChangeExtension(filename, extension);
        }

        public string GetDirectoryName(string filename)
        {
            return Path.GetDirectoryName(filename);
        }

        public string GetFileNameWithoutExtension(string filename)
        {
            return Path.GetFileNameWithoutExtension(filename);
        }

        public string PathCombine(string folder, string file)
        {
            return Path.Combine(folder, file);
        }

        public Stream CreateFileStream(string filename, FileMode fileMode)
        {
            return new FileStream(filename, (System.IO.FileMode)fileMode);
        }

        public Stream CreateFileStream(string filename, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            return new FileStream(filename, (System.IO.FileMode)fileMode, (System.IO.FileAccess)fileAccess, (System.IO.FileShare)fileShare);
        }

        public Stream CreateFileStream(string filename, FileMode fileMode, FileAccess fileAccess)
        {
            return new FileStream(filename, (System.IO.FileMode)fileMode, (System.IO.FileAccess)fileAccess);
        }

        public string ReadAllText(string filename)
        {
            return File.ReadAllText(filename);
        }
    }
}
