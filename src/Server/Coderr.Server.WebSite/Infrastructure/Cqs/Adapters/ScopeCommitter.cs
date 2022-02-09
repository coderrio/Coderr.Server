using System;
using Griffin.Data;

namespace Coderr.Server.WebSite.Infrastructure.Cqs.Adapters
{
    /// <summary>
    ///     Commits unit of work when scope is closing if no exceptions have been thrown in the current scope
    /// </summary>
    public class ScopeCommitter : IDisposable
    {
        private readonly IAdoNetUnitOfWork _uow;

        public ScopeCommitter(IAdoNetUnitOfWork uow)
        {
            _uow = uow;
        }

        public Exception Exception { get; set; }

        public void Dispose()
        {
            if (Exception == null) _uow.SaveChanges();
        }
    }
}