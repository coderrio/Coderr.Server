using System;
using DotNetCqs;
using Griffin.Container;
using Griffin.Cqs.InversionOfControl;
using Griffin.Data;
using log4net;
using OneTrueError.App.Core.Accounts.Requests;
using OneTrueError.Infrastructure.Queueing;
using OneTrueError.ReportAnalyzer.Scanners;
using OneTrueError.SqlServer.Core.Users;

namespace OneTrueError.Web.Cqs
{
    public class CqsBuilder
    {
        private readonly ILog _log = LogManager.GetLogger(typeof (CqsBuilder));
        private readonly IMessageQueueProvider _queueProvider = new QueueProvider();

        public static void CloseUnitOfWorks(IContainerScope scope)
        {
            scope.Resolve<IAdoNetUnitOfWork>().SaveChanges();
        }

        public ICommandBus CreateCommandBus(IContainer container)
        {
            var iocBus = new IocCommandBus(container);
            iocBus.CommandInvoked += OnCommandInvoked;
            return iocBus;
        }

        public IEventBus CreateEventBus(IContainer container)
        {
            var registry = new EventHandlerRegistry();
            registry.ScanAssembly(typeof (ValidateNewLoginHandler).Assembly);
            registry.ScanAssembly(typeof (UserRepository).Assembly);
            registry.ScanAssembly(typeof (ScanForNewErrorReports).Assembly);

            var inner = new SeparateScopesIocEventBus(container, registry);
            var bus = new QueuedEventBus(inner, _queueProvider);
            inner.ScopeClosing += (sender, args) => CloseUnitOfWorks(args.Scope);
            inner.HandlerFailed += (sender, args) =>
            {
                foreach (var failure in args.Failures)
                {
                    _log.Error(failure.Handler.GetType().FullName + " failed to handle " + args.ApplicationEvent,
                        failure.Exception);
                }
            };
            bus.Start();

            return bus;
        }

        public IQueryBus CreateQueryBus(IContainer container)
        {
            var bus = new IocQueryBus(container);
            bus.QueryExecuted += (sender, args) => { CloseUnitOfWorks(args.Scope); };
            return bus;
        }

        public IRequestReplyBus CreateRequestReplyBus(IContainer container)
        {
            var bus = new IocRequestReplyBus(container);
            bus.RequestInvoked += (sender, args) => { CloseUnitOfWorks(args.Scope); };
            return bus;
        }

        private void OnCommandInvoked(object sender, CommandInvokedEventArgs e)
        {
            _log.Debug("Invoked " + e.Command);
            try
            {
                CloseUnitOfWorks(e.Scope);
            }
            catch (Exception exception)
            {
                _log.Fatal("failed to commit", exception);
            }
        }
    }
}