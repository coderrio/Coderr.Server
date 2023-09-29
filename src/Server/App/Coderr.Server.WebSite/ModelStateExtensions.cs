using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Coderr.Server.WebSite
{
    public static class ModelStateExtensions
    {
        public static string ToSummary(this ModelStateDictionary modelState)
        {
            var sb = new StringBuilder();
            foreach (var kvp in modelState)
            {
                sb.Append($"{kvp.Key}: ");
                var errors = kvp.Value.Errors.Select(x => x.ErrorMessage);
                sb.AppendLine(string.Join(",", errors));
            }

            return sb.ToString();
        }
    }
}
