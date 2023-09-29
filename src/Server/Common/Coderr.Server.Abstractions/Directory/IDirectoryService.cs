using System.Collections.Generic;

namespace Coderr.Server.Abstractions.Directory
{
    public interface IDirectoryService
    {
        bool IsInDomain { get; }
        IEnumerable<IDirectoryEntry> Find(string text, bool groups, bool users, int pageNumber = 1, int pageSize = 20);
        string FindEmail(string samAccountName);
        IDirectoryEntry GetBySid(string sid);
    }
}