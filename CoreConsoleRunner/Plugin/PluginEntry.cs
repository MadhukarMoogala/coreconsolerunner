using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using CoreConsoleRunner.Plugin;
[assembly: ExtensionApplication(typeof(PluginEntry))]

namespace CoreConsoleRunner.Plugin
{
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
