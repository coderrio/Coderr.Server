using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TsGenerator
{
    class Program
    {
        private static string ResolvePath;
        static void Main(string[] args)
        {
            KeyValuePair<string, int> testar = new KeyValuePair<string, int>();

            var s = testar.GetType().FullName;
            var l = Type.GetType(s);

            var path = AppDomain.CurrentDomain.BaseDirectory;
            var pos = path.IndexOf(@"src3");
            path = path.Substring(0, pos + 5);

            var destDir = Path.Combine(path, @"OneTrueError\OneTrueError.Web\Scripts\Models\");
            ResolvePath = Path.Combine(path, @"OneTrueError\OneTrueError.Api\bin\Debug");
            var srcPath= Path.Combine(path, @"OneTrueError\OneTrueError.Api\bin\Debug");
            AppDomain.CurrentDomain.AssemblyResolve += OnLoadAssembly;

            var assemblies = Directory.GetFiles(srcPath, "*api*.dll").Select(Assembly.LoadFile);
            var nameFilters = assemblies.Select(x => x.GetName().Name).ToArray();

            //var nameFilters = new[]
            //{
            //    "OneTrueError.Core.WebApi",
            //    "OneTrueError.GlobalCore.WebApi",
            //    "OneTrueError.Core.Api",
            //    "OneTrueError.GlobalCore.Api",
            //    "OneTrueError"
            //};

            var tsGen = new TypescriptGenerator("OneTrueError", nameFilters);
            //tsGen.Generate(new[] { asm1, asm2 }, @"C:\temp\Typescript");
            tsGen.Generate(assemblies, destDir);
            
        }

        private static Assembly OnLoadAssembly(object sender, ResolveEventArgs args)
        {
            var pos = args.Name.IndexOf(',');
            var name = args.Name.Substring(0, pos);

            var fullPath = Path.Combine(ResolvePath, name + ".dll");
            return Assembly.LoadFile(fullPath);
        }
    }
}
