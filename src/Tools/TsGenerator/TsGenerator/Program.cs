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
            var pos = path.IndexOf(@"\src\Tools\");
            path = path.Substring(0, pos + 5);

            var destDir = Path.Combine(path, @"Server\Coderr.Server.Web\Scripts\Models\");
            ResolvePath = Path.Combine(path, @"Server\Coderr.Server.Api\bin\Debug\net452\");
            var srcPath= Path.Combine(path, @"Server\Coderr.Server.Api\bin\Debug\net452\");
            AppDomain.CurrentDomain.AssemblyResolve += OnLoadAssembly;

            var assemblies = Directory.GetFiles(srcPath, "*api*.dll").Select(Assembly.LoadFile).ToList();
            var nameFilters = assemblies.Select(x => x.GetName().Name.Replace("Coderr", "codeRR")).ToArray();

            //var nameFilters = new[]
            //{
            //    "OneTrueError.Core.WebApi",
            //    "OneTrueError.GlobalCore.WebApi",
            //    "OneTrueError.Core.Api",
            //    "OneTrueError.GlobalCore.Api",
            //    "OneTrueError"
            //};

            var tsGen = new TypescriptGenerator("codeRR", nameFilters);
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
