using System;
using DotNetCqs;
using Griffin.Container;
using Griffin.Cqs.InversionOfControl;
using Griffin.Data;
using log4net;
using codeRR.App.Core.Accounts.Requests;
using codeRR.Infrastructure.Queueing;
using codeRR.ReportAnalyzer.Scanners;
using codeRR.SqlServer.Core.Users;

namespace codeRR.Web.Cqs
{
    public class CqsBuilder
    {
        private static readonly MyEventHandlerRegistry _registry = new MyEventHandlerRegistry();
        private readonly ILog _log = LogManager.GetLogger(typeof(CqsBuilder));
        private readonly IMessageQueueProvider _queueProvider = new QueueProvider();
        private IEventBus _eventBus;

        public EventHandlerRegistry EventHandlerRegistry
        {
            get { return _registry; }
        }

        public static void CloseUnitOfWorks(IContainerScope scope)
        {
            scope.Resolve<IAdoNetUnitOfWork>().SaveChanges();
        }

        public ICommandBus CreateCommandBus(IContainer container)
        {
            var iocBus = new IocCommandBus(container);
            iocBus.CommandInvoked += OnCommandInvoked;
            iocBus.ScopeCreated += OnCommandScope;
            return iocBus;
        }

        public IEventBus CreateEventBus(IContainer container)
        {
            // should not happen, but does sometimes. 
            // seems related to Mvc contra WebApi
            // but can't figure out why
            if (_eventBus != null)
                return _eventBus;

            _registry.ScanAssembly(typeof(ValidateNewLoginHandler).Assembly);
            _registry.ScanAssembly(typeof(UserRepository).Assembly);
            _registry.ScanAssembly(typeof(ScanForNewErrorReports).Assembly);

            //var inner = new SeparateScopesIocEventBus(container, _registry);
            var inner = new IocEventBus(container);
            inner.EventPublished += (sender, args) => CloseUnitOfWorks(args.Scope);
            //inner.ScopeClosing += (sender, args) => CloseUnitOfWorks(args.Scope);
            inner.HandlerFailed += (sender, args) =>
            {
                foreach (var failure in args.Failures)
                {
                    _log.Error(failure.Handler.GetType().FullName + " failed to handle " + args.ApplicationEvent,
                        failure.Exception);
                }
            };

            var bus = new QueuedEventBus(inner, _queueProvider);
            _eventBus = bus;

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

        private void OnCommandScope(object sender, ScopeCreatedEventArgs e)
        {
        }
    }
}