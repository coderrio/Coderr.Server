using System;

namespace Coderr.Server.Abstractions
{
    [Flags]
    public enum ServerType
    {
        Community = 0,
        Premise = 1,
        PremisePlus = 3,
        Live = 4
    }
}