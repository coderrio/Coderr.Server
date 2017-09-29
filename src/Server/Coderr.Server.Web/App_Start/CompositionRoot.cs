using System;
using System.Configuration;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using codeRR.Server.App.Core.Accounts.Requests;
using codeRR.Server.Infrastructure;
using codeRR.Server.Infrastructure.Queueing;
using codeRR.Server.ReportAnalyzer;
using codeRR.Server.ReportAnalyzer.Scanners;
using codeRR.Server.SqlServer.Core.Users;
using codeRR.Server.Web.IoC;
using Griffin.Container;
using Griffin.Container.Mvc5;
using Griffin.Data;
using log4net;

namespace codeRR.Server.Web
{
    public class CompositionRoot
    {
        public static IContainer Container;
        private readonly ILog _logger = LogManager.GetLogger(typeof(CompositionRoot));

        public void Build(Action<ContainerRegistrar> action)
        {
            var builder = new ContainerRegistrar();
            builder.RegisterComponents(Lifetime.Scoped, Assembly.GetExecutingAssembly());
            builder.RegisterService(CreateConnection, Lifetime.Scoped);
            builder.RegisterService(CreateTaskInvoker, Lifetime.Singleton);
            action(builder);

            RegisterBuiltInComponents(builder);
            RegisterQueues(builder);

            builder.RegisterService(x => Container, Lifetime.Singleton);
            builder.RegisterService(x => x);
            builder.RegisterConcrete<AnalysisDbContext>();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            var ioc = builder.Build();

            DependencyResolver.SetResolver(new GriffinDependencyResolver(ioc));
            GlobalConfiguration.Configuration.DependencyResolver = new GriffinWebApiDependencyResolver2(ioc);
            Container = new GriffinContainerAdapter(ioc);
        }

        private IAdoNetUnitOfWork CreateConnection(IServiceLocator arg)
        {
            return new AdoNetUnitOfWork(ConnectionFactory.Create(), true);
        }

        private IScopedTaskInvoker CreateTaskInvoker(IServiceLocator arg)
        {
            var invoker = new ScopedTaskInvoker(Container);
            invoker.TaskExecuted += (sender, args) =>
            {
                var db = args.Scope.Resolve<IAdoNetUnitOfWork>();
                db.SaveChanges();
            };
            return invoker;
        }

        private void RegisterBuiltInComponents(ContainerRegistrar builder)
        {
            builder.RegisterComponents(Lifetime.Scoped, typeof(ValidateNewLoginHandler).Assembly);
            builder.RegisterComponents(Lifetime.Scoped, typeof(UserRepository).Assembly);
            builder.RegisterComponents(Lifetime.Scoped, typeof(ScanForNewErrorReports).Assembly);
        }

        private void RegisterQueues(ContainerRegistrar builder)
        {
            builder.RegisterComponents(Lifetime.Scoped, typeof(QueueProvider).Assembly);

            var queueProviderTypeStr = ConfigurationManager.AppSettings["QueueProviderType"];
            if (string.IsNullOrEmpty(queueProviderTypeStr))
            {
                builder.RegisterConcrete<QueueProvider>(Lifetime.Singleton);
                return;
            }

            var type = Type.GetType(queueProviderTypeStr, true);
            builder.RegisterConcrete(type, Lifetime.Singleton);
        }
    }
}