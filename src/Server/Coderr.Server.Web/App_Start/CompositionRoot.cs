using System;
using System.Data;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using codeRR.Server.App;
using codeRR.Server.App.Configuration;
using codeRR.Server.Infrastructure;
using codeRR.Server.Infrastructure.Configuration;
using codeRR.Server.ReportAnalyzer;
using codeRR.Server.SqlServer.Core.Users;
using codeRR.Server.Web.IoC;
using Griffin.Container;
using Griffin.Container.Mvc5;
using Griffin.Data;
using log4net;

namespace codeRR.Server.Web
{
    public interface IConfiguration<out TConfigType>
    {
        TConfigType Value { get; }
    }

    public class CompositionRoot
    {
        public static IContainer Container;
        private readonly ILog _logger = LogManager.GetLogger(typeof(CompositionRoot));

        public void Build(Action<ContainerRegistrar> action, ConfigurationStore configStore)
        {
            var builder = new ContainerRegistrar();

            //need to invoke first to allow plugins to override default behaviour
            action(builder);

            builder.RegisterComponents(Lifetime.Scoped, Assembly.GetExecutingAssembly());
            builder.RegisterService(CreateConnection, Lifetime.Scoped);
            builder.RegisterService(CreateTaskInvoker, Lifetime.Singleton);

            RegisterBuiltInComponents(builder);
            RegisterQueues(builder);

            builder.RegisterService(x => Container, Lifetime.Singleton);
            builder.RegisterService(x => x);
            builder.RegisterConcrete<AnalysisDbContext>();
            builder.RegisterInstance(configStore);
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            builder.RegisterService(x => configStore.Load<BaseConfiguration>());
            var ioc = builder.Build();

            DependencyResolver.SetResolver(new GriffinDependencyResolver(ioc));
            GlobalConfiguration.Configuration.DependencyResolver = new GriffinWebApiDependencyResolver2(ioc);
            Container = new GriffinContainerAdapter(ioc);
        }

        private IAdoNetUnitOfWork CreateConnection(IServiceLocator arg)
        {
            var con = DbConnectionFactory.Open(Startup.ConnectionStringName, true);
            return new AdoNetUnitOfWork(con, true, IsolationLevel.RepeatableRead);
        }

        private IScopedTaskInvoker CreateTaskInvoker(IServiceLocator arg)
        {
            var invoker = new ScopedTaskInvoker(Container);
            invoker.TaskExecuted += (sender, args) =>
            {
                var db = args.Scope.Resolve<AnalysisDbContext>();
                db.UnitOfWork.SaveChanges();
            };
            return invoker;
        }

        private void RegisterBuiltInComponents(ContainerRegistrar builder)
        {
            builder.RegisterComponents(Lifetime.Scoped, typeof(AppType).Assembly);
            builder.RegisterComponents(Lifetime.Scoped, typeof(UserRepository).Assembly);
            builder.RegisterComponents(Lifetime.Scoped, typeof(ReportAnalyzer.Handlers.Reports.ReportAnalyzer).Assembly);
        }

        private void RegisterQueues(ContainerRegistrar builder)
        {
            builder.RegisterComponents(Lifetime.Scoped, typeof(SetupTools).Assembly);

            //var queueProviderTypeStr = ConfigurationManager.AppSettings["QueueProviderType"];
            //if (string.IsNullOrEmpty(queueProviderTypeStr))
            //{
            //    builder.RegisterConcrete<MessageQueueProvider>(Lifetime.Singleton);
            //    return;
            //}

            //var type = Type.GetType(queueProviderTypeStr, true);
            //builder.RegisterConcrete(type, Lifetime.Singleton);
        }
    }
}