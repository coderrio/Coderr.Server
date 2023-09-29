using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coderr.Server.ReportAnalyzer.Feedback
{
    public interface IUserFeedbackRepository
    {
        Task CreateAsync(NewFeedback feedback);
    }
}
