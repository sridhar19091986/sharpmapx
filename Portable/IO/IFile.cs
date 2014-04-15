using System.IO;

namespace Portable.IO
{
    public interface IFile
    {
        bool FileExists(string filename);
        string ReadAllText(string filename);
        string ChangeExtension(string filename, string extension);
        string GetDirectoryName(string filename);
        string GetFileNameWithoutExtension(string filename);
        string PathCombine(string folder, string file);
        Stream CreateFileStream(string filename, FileMode fileMode);
        Stream CreateFileStream(string filename, FileMode fileMode, FileAccess fileAccess, FileShare fileShare);
        Stream CreateFileStream(string filename, FileMode fileMode, FileAccess fileAccess);
    }
}
