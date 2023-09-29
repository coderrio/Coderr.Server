using System.Threading.Tasks;

namespace Coderr.Server.ReportAnalyzer.Abstractions
{
    /// <summary>
    /// Class can be used to commit a unit of work.
    /// </summary>
    public interface ISaveable
    {
        Task SaveChanges();
    }
}