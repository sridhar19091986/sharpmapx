using System.Reflection;

namespace SharpMap.Utilities
{
#if PCL
    public static class AssemblyExtensions
    {
        public static AssemblyName GetName(this Assembly assembly)
        {
            return new AssemblyName(assembly.FullName);
        }
    }
#endif
}
