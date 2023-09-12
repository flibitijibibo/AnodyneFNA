using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace AnodyneSharp.Utilities
{
#nullable enable
    internal static class AssemblyReaderUtil
    {
        public static Stream? GetStream(string path)
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            return asm.GetManifestResourceStream($"{asm.GetName().Name}.{path}");
        }
    }
#nullable restore
}
