using System;

namespace Coderr.Tests.Runners.AppDomain.DTO
{
    [Serializable]
    public enum TestOutcome 
    {
        Ignored,
        Success,
        Failed,
        NotFound
    }
}