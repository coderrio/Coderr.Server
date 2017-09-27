using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using codeRR.Server.Web.Areas.Receiver.Helpers;
using Newtonsoft.Json;

namespace codeRR.Server.Web.Areas.Receiver.Models
{
    public class SamplingCounter
    {
        private readonly List<SamplingSetting> _settings = new List<SamplingSetting>();

        public bool CanAccept(string appKey)
        {
            if (string.IsNullOrEmpty(appKey))
                return false;

            var item = _settings.FirstOrDefault(x => x.AppKey.Equals(appKey));
            if (item == null)
                return true;

            return item.CanAccept();
        }

        public void Load()
        {
            var path = GetFilePath();
            if (!File.Exists(path))
                return;

            var json = File.ReadAllText(path, Encoding.UTF8);
            var items = JsonConvert.DeserializeObject<IEnumerable<SamplingSetting>>(json);
            _settings.AddRange(items);
        }

        public void Save()
        {
            var file = GetFilePath();
            var json = JsonConvert.SerializeObject(_settings.ToArray());
            File.WriteAllText(file, json, Encoding.UTF8);
        }

        private static string GetFilePath()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            //if (Debugger.IsAttached)
            //path = Path.GetFullPath(path + "..\\..");

            path = Path.Combine(path, "SamplingSettings.json");
            return path;
        }
    }
}