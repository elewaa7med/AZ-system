using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Loader;
using System.Reflection;
using System.IO;

namespace SmartAdmin.WebUI
{
    public class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        public IntPtr LoadUnmanagedLibrary(string absolutePath)
        {
            return LoadUnmanagedDll(absolutePath);
        }
        protected override IntPtr LoadUnmanagedDll(String unmanagedDllName)
        {
            return LoadUnmanagedDllFromPath(unmanagedDllName);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            throw new NotImplementedException();
        }
    }
    public static class WkHtmlToPdf
    {
        public static void Preload(Microsoft.AspNetCore.Hosting.IHostingEnvironment _env)
        {
            var wkHtmlToPdfContext = new CustomAssemblyLoadContext();
            var architectureFolder = (IntPtr.Size == 8) ? "64 bit" : "32 bit";
            var wkHtmlToPdfPath = Path.Combine(_env.WebRootPath, $"v0.12.4\\{architectureFolder}\\libwkhtmltox");
            wkHtmlToPdfContext.LoadUnmanagedLibrary(wkHtmlToPdfPath);
        }
    }
}
