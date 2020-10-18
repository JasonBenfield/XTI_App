using System;

namespace XTI_Core
{
    public sealed class UtcClock : Clock
    {
        public DateTime Now() => DateTime.UtcNow;

        public DateTime Today() => DateTime.UtcNow.Date;
    }
}
