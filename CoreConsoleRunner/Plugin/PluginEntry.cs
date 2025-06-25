using Autodesk.AutoCAD.Runtime;
using CoreConsoleRunner.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
[assembly: ExtensionApplication(typeof(PluginEntry))]

namespace CoreConsoleRunner.Plugin
{
    /// <summary>
    /// Attaches or allocates a console window (with AttachConsole(-1) or AllocConsole()).
    /// This helps see Console.WriteLine() outputs in a traditional AutoCAD session(UI), especially when debugging inside AutoCAD itself.
    /// </summary>

    public class PluginEntry : IExtensionApplication
    {
        public void Initialize()
        {
            if (!AttachConsole(-1))  // Attach to a parent process console
            {
                AllocConsole(); // Alloc a new console
            }
        }

        public void Terminate()
        {
            FreeConsole();
        }
        // http://stackoverflow.com/questions/3917202/how-do-i-include-a-console-in-winforms/3917353#3917353
        //Allocates a new console for current process.
        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();

        //Frees the console
        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        // http://stackoverflow.com/a/8048269/492
        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int pid);
    }
}
