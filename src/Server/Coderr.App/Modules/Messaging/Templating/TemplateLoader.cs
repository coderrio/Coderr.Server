using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

namespace codeRR.Server.App.Modules.Messaging.Templating
{
    /// <summary>
    ///     Loads a mail template.
    /// </summary>
    public class TemplateLoader
    {
        private static readonly Dictionary<string, TemplateMapping> _templates =
            new Dictionary<string, TemplateMapping>();

        /// <summary>
        ///     Load template
        /// </summary>
        /// <param name="templateName">TODO: Describe name/path convention</param>
        /// <returns>templare if found; otherwise <c>null</c>.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "The static field is just an implementation detail.")]
        public Template Load(string templateName)
        {
            if (_templates.Count == 0)
                LoadTemplates();

            TemplateMapping mapping;
            if (!_templates.TryGetValue(templateName, out mapping))
                throw new ArgumentOutOfRangeException("templateName", templateName,
                    "Failed to find template '" + templateName + "'.");

            var templateStream = mapping.Assembly.GetManifestResourceStream(mapping.FullResourceName);

            var template = new Template {Content = templateStream, Resources = new Dictionary<string, Stream>()};
            foreach (var kvp in mapping.ResourceNames)
            {
                var resource = mapping.Assembly.GetManifestResourceStream(kvp.Value);
                template.Resources.Add(kvp.Key, resource);
            }
            return template;
        }

        private static void LoadTemplates()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.IsDynamic)
                    continue;

                LoadTemplates(assembly);
            }
        }

        private static void LoadTemplates(Assembly assembly)
        {
            var resources = assembly.GetManifestResourceNames();
            var templateNames =
                resources.Where(x => x.EndsWith(".Template.md", StringComparison.OrdinalIgnoreCase)
                                     || x.EndsWith(".Template.html", StringComparison.OrdinalIgnoreCase));


            foreach (var templateName in templateNames)
            {
                var mapping = new TemplateMapping {FullResourceName = templateName};

                var toRemove = templateName.EndsWith(".html")
                    ? "Template.html".Length
                    : "Template.md".Length;
                var ns = templateName.Remove(templateName.Length - toRemove, toRemove);
                var fullImageNames = resources.Where(x => x.StartsWith(ns) && x != templateName).ToList();
                var images = new Dictionary<string, string>();
                foreach (var imagePath in fullImageNames)
                {
                    if (imagePath.EndsWith("Template.md") || imagePath.EndsWith("Template.html"))
                        continue;

                    var imageName = imagePath.Remove(0, ns.Length);
                    images.Add(imageName, imagePath);
                }

                var pos = ns.TrimEnd('.').LastIndexOf('.');
                var name = ns.Substring(pos).Trim('.');
                mapping.TemplateName = name;
                mapping.ResourceNames = images;
                mapping.Assembly = assembly;

                if (_templates.ContainsKey(templateName))
                    throw new InvalidOperationException("Already contains " + templateName);

                _templates[mapping.TemplateName] = mapping;
            }
        }

        private class TemplateMapping
        {
            public Assembly Assembly { get; set; }
            public string FullResourceName { get; set; }
            public Dictionary<string, string> ResourceNames { get; set; }
            public string TemplateName { get; set; }
        }
    }
}