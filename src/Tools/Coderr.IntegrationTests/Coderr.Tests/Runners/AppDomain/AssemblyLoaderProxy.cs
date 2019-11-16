using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Coderr.Tests.Runners.AppDomain
{
    public class AssemblyLoaderProxy : MarshalByRefObject
    {
        public AssemblyLoaderProxy()
        {
            System.AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        public void Load(string path)
        {
            ValidatePath(path);

            Assembly.Load(path);
        }

        public void LoadFrom(string path)
        {
            ValidatePath(path);

            Assembly.LoadFrom(path);
        }

        private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var wantedName = new AssemblyName(args.Name);

            var dirs = _directories;
            if (args.RequestingAssembly != null)
            {
                dirs = new List<string>(_directories);
                var dir = Path.GetDirectoryName(args.RequestingAssembly.Location);
                dirs.Add(dir);
            }

            AssemblyName name = null;
            foreach (var directory in dirs)
            {
                CompareName(directory, wantedName, ref name);
            }

            if (name == null)
                return null;

            var fullPath = new Uri(name.CodeBase).LocalPath;
            return Assembly.LoadFile(fullPath);
        }

        private static void CompareName(string directory, AssemblyName wantedName, ref AssemblyName bestMatch)
        {
            if (bestMatch?.Version == wantedName.Version)
                return;

            var fullPath = Path.Combine(directory, wantedName.Name + ".dll");
            if (!File.Exists(fullPath))
            {
                return;
            }

            var foundName = AssemblyName.GetAssemblyName(fullPath);
            if (foundName.Version == wantedName.Version || bestMatch == null)
            {
                bestMatch = foundName;
                return;
            }

            if (foundName.Version < wantedName.Version)
            {
                if (bestMatch == null)
                    bestMatch = foundName;
                return;
            }

            // Lets take a version as close as possible
            if (bestMatch.Version > foundName.Version)
            {
                bestMatch = foundName;
            }
        }

        private void ValidatePath(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (!File.Exists(path))
                throw new ArgumentException($"path \"{path}\" does not exist");
        }
        List<string> _directories = new List<string>();

        public void AddDirectory(string directory)
        {
            _directories.Add(directory);
        }
    }
}