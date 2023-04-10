using System;

namespace Logger
{
    [Flags]
    public enum LogTag
    {
        None = 0,
        General = 1 << 0,
        UI = 1 << 1,
        Battle = 1 << 2,
        Addressable = 1 << 3,
        Analytics = 1 << 4,
        Afk = 1 << 5,
        GameFacade = 1 << 6,
    }
}