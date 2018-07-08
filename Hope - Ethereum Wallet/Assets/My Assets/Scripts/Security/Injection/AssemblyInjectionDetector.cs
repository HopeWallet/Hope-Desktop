using Hope.Security.HashGeneration;
using System;
using System.Reflection;

namespace Hope.Security.Injection
{
    //
    // TODO
    //

    public sealed class AssemblyInjectionDetector
    {

        public AssemblyInjectionDetector()
        {
            AppDomain.CurrentDomain.AssemblyLoad += NewAssemblyLoaded;
            AppDomain.CurrentDomain.GetAssemblies().ForEach(DisplayAssemblyInfo);
        }

        private void NewAssemblyLoaded(object sender, AssemblyLoadEventArgs args)
        {
            DisplayAssemblyInfo(args.LoadedAssembly);
        }

        private void DisplayAssemblyInfo(Assembly assembly)
        {
            var name = assembly.FullName;
            var nameHash = name.GetSHA384Hash();

            //UnityEngine.Debug.Log(name + " | | | =====> " + nameHash);
        }
    }
}
