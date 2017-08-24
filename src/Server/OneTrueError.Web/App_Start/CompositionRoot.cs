using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Griffin.Container;
using Griffin.Container.Mvc5;
using Griffin.Data;
using OneTrueError.App.Core.Accounts.Requests;
using OneTrueError.Infrastructure.Queueing;
using OneTrueError.ReportAnalyzer.Scanners;
using OneTrueError.SqlServer.Core.Users;
using OneTrueError.Web.IoC;

namespace OneTrueError.Web
{
    public class CompositionRoot
    {
        public static IContainer Container;

        public void Build(Action<ContainerRegistrar> action)
        {
            var builder = new ContainerRegistrar();
            builder.RegisterComponents(Lifetime.Scoped, Assembly.GetExecutingAssembly());
            builder.RegisterService(CreateConnection, Lifetime.Scoped);
            builder.RegisterService(CreateTaskInvoker, Lifetime.Singleton);
            action(builder);

            builder.RegisterComponents(Lifetime.Scoped, typeof(ValidateNewLoginHandler).Assembly);
            builder.RegisterComponents(Lifetime.Scoped, typeof(UserRepository).Assembly);
            builder.RegisterComponents(Lifetime.Scoped, typeof(ScanForNewErrorReports).Assembly);
            builder.RegisterComponents(Lifetime.Scoped, typeof(QueueProvider).Assembly);

            builder.RegisterService(x => Container, Lifetime.Singleton);
            builder.RegisterService(x => x);

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            var ioc = builder.Build();

            DependencyResolver.SetResolver(new GriffinDependencyResolver(ioc));
            GlobalConfiguration.Configuration.DependencyResolver = new GriffinWebApiDependencyResolver2(ioc);
            Container = new GriffinContainerAdapter(ioc);
        }

        private IAdoNetUnitOfWork CreateConnection(IServiceLocator arg)
        {
            var conStr = ConfigurationManager.ConnectionStrings["Db"];
            var connection = new SqlConnection(conStr.ConnectionString);
            connection.Open();
            return new AdoNetUnitOfWork(connection, true);
        }

        private IScopedTaskInvoker CreateTaskInvoker(IServiceLocator arg)
        {
            var invoker = new ScopedTaskInvoker(Container);
            invoker.TaskExecuted += (sender, args) => args.Scope.Resolve<IAdoNetUnitOfWork>().SaveChanges();
            return invoker;
        }
    }
}