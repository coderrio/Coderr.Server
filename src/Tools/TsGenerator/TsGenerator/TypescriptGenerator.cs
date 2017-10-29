using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TsGenerator
{
    public class TypescriptGenerator
    {
        private readonly Dictionary<Type, GeneratedTypeScript> _generatedTypes =
            new Dictionary<Type, GeneratedTypeScript>();

        private readonly string[] _namespaceFilter;
        private readonly string _rootNamespace;

        public TypescriptGenerator(string rootNamespace, string[] namespaceFilter)
        {
            _rootNamespace = rootNamespace;
            _namespaceFilter = namespaceFilter;
        }

        public string CreateFullPath(string targetFolder, string moduleName, string scriptName)
        {
            var parts = moduleName.Split('.');
            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            int steps = 0;
            foreach (var part in parts)
            {
                targetFolder = Path.Combine(targetFolder, part);
                if (!Directory.Exists(targetFolder))
                    Directory.CreateDirectory(targetFolder);
            }

            return Path.Combine(targetFolder, scriptName + ".ts");
        }

        public string CreateClass(GeneratedTypeScript scriptInfo, bool includeModule)
        {
            var sb = new StringBuilder();
            if (includeModule)
                sb.AppendFormat("module " + scriptInfo.ModuleName + " {{\r\n");
            sb.AppendFormat("    export class {0} {{\r\n", scriptInfo.TypeName);
            sb.AppendFormat("        public static TYPE_NAME: string = '" + scriptInfo.TypeName + "';\r\n");


            var properties = GetInterfaceMembers(scriptInfo.DotNetType);
            foreach (var mi in properties)
            {
                sb.AppendFormat("        public {0}: {1};\r\n", mi.Name, GetTypeName(mi, scriptInfo));
            }
            var constructor = scriptInfo.DotNetType.GetConstructors().FirstOrDefault();
            if (constructor != null)
            {
                var contructorParameterMappings = new Dictionary<string, string>();
                foreach (var parameter in constructor.GetParameters())
                {
                    var property =
                        properties.FirstOrDefault(x => x.Name.Equals(parameter.Name, StringComparison.OrdinalIgnoreCase));
                    if (property != null)
                    {
                        contructorParameterMappings.Add(parameter.Name, property.Name);
                    }
                }
                if (contructorParameterMappings.Count > 0)
                {
                    sb.Append("        public constructor(");
                    foreach (var parameter in constructor.GetParameters())
                    {
                        sb.Append(parameter.Name + ": " + GetTypeName(parameter.ParameterType, scriptInfo) + ", ");
                    }
                    sb.Remove(sb.Length - 2, 2);
                    sb.AppendLine(") {");
                    foreach (var parameter in contructorParameterMappings)
                    {
                        sb.AppendFormat("            this.{0} = {1};\r\n", parameter.Value, parameter.Key);
                    }
                    sb.AppendLine("        }");
                }


            }


            sb.AppendLine("    }");

            if (includeModule)
                sb.AppendLine("}");
            return sb.ToString();
        }

        public void Generate(IEnumerable<Assembly> assemblies, string targetFolder)
        {
            var everythingFile = Path.Combine(targetFolder, "AllModels.ts");
            if (File.Exists(everythingFile))
                File.Delete(everythingFile);

            //initial scanning just to map each type
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsPublic)
                    {
                        continue;
                    }
                    if (typeof(Attribute).IsAssignableFrom(type))
                        continue;

                    if (type.GetCustomAttributes().Any(x => x.GetType().Name.Contains("Ignore")))
                        continue;

                    _generatedTypes.Add(type, new GeneratedTypeScript
                    {
                        DotNetType = type,
                        ModuleName = GetNamespace(type.Namespace),
                        TypeName = type.ToGenericTypeString()
                    });


                }
            }

            foreach (var kvp in _generatedTypes)
            {
                if (kvp.Key.IsEnum)
                    kvp.Value.Code = GetEnum(kvp.Value, false);
                else if (kvp.Key.Name != "Class1" && !kvp.Key.IsInterface)
                {
                    kvp.Value.Code = CreateClass(kvp.Value, false);
                }

            }

            var perModule = _generatedTypes.Values.GroupBy(x => x.ModuleName);
            var moduleBuilder = new StringBuilder();
            var allBuilder = new StringBuilder();
            //allBuilder.AppendLine("module " + _rootNamespace + " {");
            foreach (var module in perModule)
            {
                moduleBuilder.Clear();
                if (module.Key != _rootNamespace)
                    moduleBuilder.AppendLine("module " + module.Key + " {");
                foreach (var script in module)
                {
                    moduleBuilder.Append(script.Code);
                    moduleBuilder.AppendLine();
                }

                if (module.Key != _rootNamespace)
                    moduleBuilder.AppendLine("}");

                var str = moduleBuilder.ToString();
                //str = str.Replace("module " + _rootNamespace + ".", "module ");
                allBuilder.Append(str);
                var fullPath = CreateFullPath(targetFolder, module.Key, "Models");
                //File.WriteAllText(fullPath, moduleBuilder.ToString(), Encoding.UTF8);
                //File.AppendAllText(everythingFile, sb.ToString(), Encoding.UTF8);
            }
            //allBuilder.AppendLine("}");
            File.WriteAllText(everythingFile, allBuilder.ToString(), Encoding.UTF8);
        }

        public string GetEnum(GeneratedTypeScript type, bool includeModule)
        {
            var sb = new StringBuilder();
            var values = (int[])Enum.GetValues(type.DotNetType);
            if (includeModule)
                sb.AppendFormat("module " + type.ModuleName + " {{\r\n");
            sb.AppendLine("    export enum " + type.TypeName + " {");
            foreach (var val in values)
            {
                var name = Enum.GetName(type.DotNetType, val);
                sb.AppendFormat("        {0} = {1},\r\n", name, val);
            }
            sb.AppendLine("    }");

            if (includeModule)
                sb.AppendLine("}");
            return sb.ToString();
        }

        private string GetFullName(Type type)
        {
            var ns = type.Namespace;
            foreach (var x in _namespaceFilter)
            {
                if (ns.StartsWith(x))
                    ns = ns.Remove(0, x.Length).Trim('.');
            }

            if (string.IsNullOrEmpty(ns))
                return _rootNamespace + "." + type.Name;

            return _rootNamespace + "." + ns + "." + type.Name;
        }

        private IEnumerable<MemberInfo> GetInterfaceMembers(Type type)
        {
            return from field in type.GetMembers()
                   where (field.MemberType == MemberTypes.Field || field.MemberType == MemberTypes.Property)
                         && field.GetCustomAttributes().All(x => x.GetType().Name != "IgnoreField")
                   select field;
        }

        private string GetNamespace(string typeNamespace)
        {
            var ns = typeNamespace;
            foreach (var x in _namespaceFilter)
            {
                if (ns.StartsWith(x))
                    ns = ns.Remove(0, x.Length).Trim('.');
            }

            if (_namespaceFilter.Any(x => ns.Equals(x)))
                return _rootNamespace;

            if (string.IsNullOrEmpty(ns))
                return _rootNamespace;

            return _rootNamespace + "." + ns;
        }

        private string GetTypeName(MemberInfo mi, GeneratedTypeScript containingType)
        {
            var t = (mi is PropertyInfo) ? ((PropertyInfo)mi).PropertyType : ((FieldInfo)mi).FieldType;
            return GetTypeName(t, containingType);
        }

        private string GetTypeName(Type type, GeneratedTypeScript containingType)
        {
            if (type.IsPrimitive)
            {
                if (type == typeof(bool)) return "boolean";
                if (type == typeof(char)) return "string";
                return "number";
            }
            if (type == typeof(decimal)) return "number";
            if (type == typeof(string)) return "string";
            if (type == typeof(Guid)) return "string";
            if (type.IsEnum)
            {
                var name = GetNamespace(type.Namespace);
                if (name == containingType.ModuleName)
                    return type.Name;

                return GetFullName(type);
            }


            if (type.IsArray)
            {
                var at = type.GetElementType();
                return GetTypeName(at, containingType) + "[]";
            }
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                var collectionType = type.GetGenericArguments()[0];
                // all my enumerables are typed, so there is a generic argument
                return GetTypeName(collectionType, containingType) + "[]";
            }
            if (Nullable.GetUnderlyingType(type) != null)
            {
                return GetTypeName(Nullable.GetUnderlyingType(type), containingType);
            }

            GeneratedTypeScript propertyType;
            if (_generatedTypes.TryGetValue(type, out propertyType))
            {
                var containingModuleName = GetNamespace(containingType.ModuleName);
                if (propertyType.ModuleName != containingModuleName)
                    return propertyType.ModuleName + "." + propertyType.TypeName;

                return propertyType.TypeName;
            }

            return "any";
        }
    }
}