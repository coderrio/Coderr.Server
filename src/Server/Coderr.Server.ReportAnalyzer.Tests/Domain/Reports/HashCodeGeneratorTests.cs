using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Coderr.Server.Domain.Core.ErrorReports;
using Coderr.Server.ReportAnalyzer.ErrorReports;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Coderr.Server.ReportAnalyzer.Tests.Domain.Reports
{
    public class HashCodeGeneratorTests
    {
        [Fact]
        public void Should_strip_line_numbers_to_reduce_the_number_of_incidents()
        {
            var ex1 = new ErrorReportException {StackTrace = @"   at System.Web.Compilation.AssemblyBuilder.Compile():line 64
at System.Web.Compilation.BuildProvidersCompiler.PerformBuild():line 65
at System.Web.Compilation.BuildManager.CompileWebFile(VirtualPath virtualPath):line 67"};
            var ex2 = new ErrorReportException
            {
                StackTrace = @"   at System.Web.Compilation.AssemblyBuilder.Compile():line 12
at System.Web.Compilation.BuildProvidersCompiler.PerformBuild():line 23
at System.Web.Compilation.BuildManager.CompileWebFile(VirtualPath virtualPath):line 53"
            };
            var report1 = new ErrorReportEntity(1, "fjkkfjjkf", DateTime.UtcNow, ex1, new List<ErrorReportContextCollection>());
            var report2 = new ErrorReportEntity(1, "fjkkfjjkf", DateTime.UtcNow, ex2, new List<ErrorReportContextCollection>());

            var sut = new HashCodeGenerator(new IHashCodeSubGenerator[0]);
            var actual1 = sut.GenerateHashCode(report1);
            var actual2 = sut.GenerateHashCode(report2);

            actual1.Should().Be(actual2);
        }

        [Fact]
        public void should_match_line_numbers_and_without_line_numbers_to_reduce_the_number_of_incidents()
        {
            var ex1 = new ErrorReportException { StackTrace = @"at System.Web.Compilation.AssemblyBuilder.Compile():line 64
at System.Web.Compilation.BuildProvidersCompiler.PerformBuild():line 65
at System.Web.Compilation.BuildManager.CompileWebFile(VirtualPath virtualPath):line 67" };
            var ex2 = new ErrorReportException
            {
                StackTrace = @"at System.Web.Compilation.AssemblyBuilder.Compile()
at System.Web.Compilation.BuildProvidersCompiler.PerformBuild()
at System.Web.Compilation.BuildManager.CompileWebFile(VirtualPath virtualPath)"
            };
            var report1 = new ErrorReportEntity(1, "fjkkfjjkf", DateTime.UtcNow, ex1, new List<ErrorReportContextCollection>());
            var report2 = new ErrorReportEntity(1, "fjkkfjjkf", DateTime.UtcNow, ex2, new List<ErrorReportContextCollection>());

            var sut = new HashCodeGenerator(new IHashCodeSubGenerator[0]);
            var actual1 = sut.GenerateHashCode(report1);
            var actual2 = sut.GenerateHashCode(report2);

            actual1.Should().Be(actual2);
        }


        [Fact]
        public void Should_clean_aspnet()
        {
            var stacktrace = @"   at System.Text.Encoding.GetBytes(String s)
   at Coderr.Server.Web.Controllers.OnboardingController.CalculateMd5Hash(String input) in C:\src\1tcompany\coderr\oss\codeRR.Server\src\Server\Coderr.Server.Web\Controllers\OnboardingController.cs:line 47
   at Coderr.Server.Web.Controllers.OnboardingController.Library(String id, String appKey)
   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.InvokeActionMethodAsync()
   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.InvokeNextActionFilterAsync()
   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.Rethrow(ActionExecutedContext context)
   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.InvokeInnerFilterAsync()
   at Microsoft.AspNetCore.Mvc.Internal.ResourceInvoker.InvokeNextExceptionFilterAsync()";

            var sut = new HashCodeGenerator(new IHashCodeSubGenerator[0]);
            var data = HashCodeGenerator.CleanStackTrace(stacktrace);

            data.Should().Be(@"System.Text.Encoding.GetBytes(String s)
Coderr.Server.Web.Controllers.OnboardingController.CalculateMd5Hash(String input) in C:\src\1tcompany\coderr\oss\codeRR.Server\src\Server\Coderr.Server.Web\Controllers\OnboardingController.cs
Coderr.Server.Web.Controllers.OnboardingController.Library(String id, String appKey)");
        }
        [Fact]
        public void Should_clean_async()
        {
            var stacktrace = @"   at System.Data.SqlClient.SqlCommand.<>c.<ExecuteDbDataReaderAsync>b__108_0(Task`1 result)
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.Tasks.Task.<>c.<.cctor>b__276_1(Object obj)
   at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot)
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at Coderr.Server.SqlServer.Core.Incidents.Queries.GetCollectionHandler.<HandleAsync>d__2.MoveNext() in D:\AgentWork\10\s\src\Server\Coderr.Server.SqlServer\Core\Incidents\Queries\GetCollectionHandler.cs:line 46
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at DotNetCqs.DependencyInjection.MessageInvoker.<InvokeQueryHandler>d__22.MoveNext()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at DotNetCqs.DependencyInjection.MessageInvoker.<InvokeQueryHandler>d__22.MoveNext()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at DotNetCqs.DependencyInjection.MessageInvoker.<ProcessAsync>d__9.MoveNext()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at DotNetCqs.MessageProcessor.ExecuteQueriesInvocationContext.<QueryAsync>d__13`1.MoveNext()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at DotNetCqs.Bus.ScopedQueryBus.<QueryAsync>d__3`1.MoveNext()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at Coderr.Server.Common.App.HighlightedData.CustomContextDataProvider.<CollectAsync>d__2.MoveNext() in D:\AgentWork\10\s\src\Server\Common\Coderr.Server.Common.App\HighlightedData\CustomContextDataProvider.cs:line 27
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at Coderr.Server.SqlServer.Core.Incidents.Queries.GetIncidentHandler.<HandleAsync>d__6.MoveNext() in D:\AgentWork\10\s\src\Server\Coderr.Server.SqlServer\Core\Incidents\Queries\GetIncidentHandler.cs:line 107
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at DotNetCqs.DependencyInjection.MessageInvoker.<InvokeQueryHandler>d__22.MoveNext()
";

            var sut = new HashCodeGenerator(new IHashCodeSubGenerator[0]);
            var data = HashCodeGenerator.CleanStackTrace(stacktrace);

            data.Should().Be(
                @"   at System.Data.SqlClient.SqlCommand.<>c.<ExecuteDbDataReaderAsync>b__108_0(Task`1 result)
   at Coderr.Server.SqlServer.Core.Incidents.Queries.GetCollectionHandler.HandleAsync in D:\AgentWork\10\s\src\Server\Coderr.Server.SqlServer\Core\Incidents\Queries\GetCollectionHandler.cs()
   at DotNetCqs.DependencyInjection.MessageInvoker.InvokeQueryHandler()
   at DotNetCqs.DependencyInjection.MessageInvoker.InvokeQueryHandler()
   at DotNetCqs.DependencyInjection.MessageInvoker.ProcessAsync()
   at DotNetCqs.MessageProcessor.ExecuteQueriesInvocationContext.QueryAsync()
   at DotNetCqs.Bus.ScopedQueryBus.QueryAsync()
   at Coderr.Server.Common.App.HighlightedData.CustomContextDataProvider.CollectAsync() in D:\AgentWork\10\s\src\Server\Common\Coderr.Server.Common.App\HighlightedData\CustomContextDataProvider.cs()
   at Coderr.Server.SqlServer.Core.Incidents.Queries.GetIncidentHandler.HandleAsync() in D:\AgentWork\10\s\src\Server\Coderr.Server.SqlServer\Core\Incidents\Queries\GetIncidentHandler.cs()
   at DotNetCqs.DependencyInjection.MessageInvoker.InvokeQueryHandler()
");
        }

        [Fact]
        public void Should_ignore_java_lambdas()
        {
            var ex1 = new ErrorReportException { StackTrace = @"at java.lang.NumberFormatException->forInputString(null:-1)
at java.lang.Integer->parseInt(null:-1)
at java.lang.Integer->parseInt(null:-1)
at se.fleetech.gateway.app.services.alarms.EngineCoolantTemperatureAlarmStateChangeServiceImpl->handleET1Message(EngineCoolantTemperatureAlarmStateChangeServiceImpl.java:110)
at se.fleetech.gateway.app.services.alarms.EngineCoolantTemperatureAlarmStateChangeServiceImpl->actorCallback(EngineCoolantTemperatureAlarmStateChangeServiceImpl.java:58)
at se.fleetech.gateway.app.services.alarms.EngineCoolantTemperatureAlarmStateChangeServiceImpl$$Lambda$19/30866698->accept(null:-1)
at se.fleetech.gateway.app.services.general.ActorService->lambda$start$0(ActorService.java:45)
at se.fleetech.gateway.app.services.general.ActorService$$Lambda$36/31222037->run(null:-1)
at java.util.concurrent.ThreadPoolExecutor->runWorker(null:-1)
at java.util.concurrent.ThreadPoolExecutor$Worker->run(null:-1)
at java.lang.Thread->run(null:-1)" };
            var ex2 = new ErrorReportException
            {
                StackTrace = @"at java.lang.NumberFormatException->forInputString(null:-1)
at java.lang.Integer->parseInt(null:-1)
at java.lang.Integer->parseInt(null:-1)
at se.fleetech.gateway.app.services.alarms.EngineCoolantTemperatureAlarmStateChangeServiceImpl->handleET1Message(EngineCoolantTemperatureAlarmStateChangeServiceImpl.java:112)
at se.fleetech.gateway.app.services.alarms.EngineCoolantTemperatureAlarmStateChangeServiceImpl->actorCallback(EngineCoolantTemperatureAlarmStateChangeServiceImpl.java:62)
at se.fleetech.gateway.app.services.alarms.EngineCoolantTemperatureAlarmStateChangeServiceImpl$$Lambda$19/14039178->accept(null:-1)
at se.fleetech.gateway.app.services.general.ActorService->lambda$start$0(ActorService.java:45)
at se.fleetech.gateway.app.services.general.ActorService$$Lambda$36/12952319->run(null:-1)
at java.util.concurrent.ThreadPoolExecutor->runWorker(null:-1)
at java.util.concurrent.ThreadPoolExecutor$Worker->run(null:-1)
at java.lang.Thread->run(null:-1)"
            };

            var report1 = new ErrorReportEntity(1, "fjkkfjjkf", DateTime.UtcNow, ex1, new List<ErrorReportContextCollection>());
            var report2 = new ErrorReportEntity(1, "dddwqw", DateTime.UtcNow, ex2, new List<ErrorReportContextCollection>());

            var sut = new HashCodeGenerator(new IHashCodeSubGenerator[0]);
            var actual1 = sut.GenerateHashCode(report1);
            var actual2 = sut.GenerateHashCode(report2);

            actual1.Should().Be(actual2);
        }
    }
}