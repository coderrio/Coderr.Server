using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coderr.Server.Domain.Core.ErrorReports;
using Coderr.Server.ReportAnalyzer.ErrorReports;
using FluentAssertions;
using Xunit;

namespace Coderr.Server.ReportAnalyzer.Tests.ErrorsReports.HashCodeGenerators
{
    public class HashCodeGeneratorTests
    {

        [Fact]
        public void Should_remove_AT_from_stacktrace_to_avoid_multi_language_issues()
        {
            var exceptionWithAt = new ErrorReportException()
            {
                StackTrace =
                    @"at System.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at System.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at System.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at System.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString)
   at System.Data.SqlClient.SqlCommand.RunExecuteReaderTds(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, Boolean async, Int32 timeout, Task& task, Boolean asyncWrite, SqlDataReader ds)
   at System.Data.SqlClient.SqlCommand.InternalExecuteNonQuery(TaskCompletionSource`1 completion, Boolean sendToPipe, Int32 timeout, Boolean asyncWrite, String methodName)
   at System.Data.SqlClient.SqlCommand.ExecuteNonQuery()
   at Coderr.Server.Infrastructure.Configuration.Database.DatabaseStore.Store(IConfigurationSection section)
   at Coderr.Server.Premise.App.LicenseWrapper.IncreaseReportCount()
   at Coderr.Server.Web.Controllers.ReportReceiverController.PremiseLicenseCheck()
   at Coderr.Server.Web.Controllers.ReportReceiverController.Post(String appKey, String sig)
   at Microsoft.AspNetCore.Mvc.Internal.ActionMethodExecutor.TaskOfIActionResultExecutor.Execute(IActionResultTypeMapper mapper, ObjectMethodExecutor executor, Object controller, Object[] arguments)
   at System.Threading.Tasks.ValueTask`1.get_Result()
   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.InvokeActionMethodAsync():55
   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.InvokeNextActionFilterAsync()
   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.Rethrow(ActionExecutedContext context)
   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.InvokeInnerFilterAsync()
   at Microsoft.AspNetCore.Mvc.Internal.ResourceInvoker.InvokeNextExceptionFilterAsync():33"
            };
            var entityWithAt = new ErrorReportEntity(1, "flldfd", DateTime.UtcNow, exceptionWithAt, new ErrorReportContextCollection[0]);
            var exceptionWithoutAt = new ErrorReportException()
            {
                StackTrace =
                    @"System.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   System.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   System.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   System.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString)
   System.Data.SqlClient.SqlCommand.RunExecuteReaderTds(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, Boolean async, Int32 timeout, Task& task, Boolean asyncWrite, SqlDataReader ds)
   System.Data.SqlClient.SqlCommand.InternalExecuteNonQuery(TaskCompletionSource`1 completion, Boolean sendToPipe, Int32 timeout, Boolean asyncWrite, String methodName)
   System.Data.SqlClient.SqlCommand.ExecuteNonQuery()
   Coderr.Server.Infrastructure.Configuration.Database.DatabaseStore.Store(IConfigurationSection section)
   Coderr.Server.Premise.App.LicenseWrapper.IncreaseReportCount()
   Coderr.Server.Web.Controllers.ReportReceiverController.PremiseLicenseCheck()
   Coderr.Server.Web.Controllers.ReportReceiverController.Post(String appKey, String sig)
   Microsoft.AspNetCore.Mvc.Internal.ActionMethodExecutor.TaskOfIActionResultExecutor.Execute(IActionResultTypeMapper mapper, ObjectMethodExecutor executor, Object controller, Object[] arguments)
   System.Threading.Tasks.ValueTask`1.get_Result()
   Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.InvokeActionMethodAsync():55
   Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.InvokeNextActionFilterAsync()
   Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.Rethrow(ActionExecutedContext context)
   Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
   Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.InvokeInnerFilterAsync()
   Microsoft.AspNetCore.Mvc.Internal.ResourceInvoker.InvokeNextExceptionFilterAsync():33"
            };
            var entityWithoutAt = new ErrorReportEntity(1, "flldfd", DateTime.UtcNow, exceptionWithoutAt, new ErrorReportContextCollection[0]);
            var sut = new HashCodeGenerator(new IHashCodeSubGenerator[0]);

            var result1 = sut.GenerateHashCode(entityWithAt);
            var result2 = sut.GenerateHashCode(entityWithAt);

            result1.HashCode.Should().Be(result2.HashCode);
        }
    }
}
