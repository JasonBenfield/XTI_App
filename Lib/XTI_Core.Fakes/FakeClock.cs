using System;

namespace XTI_Core.Fakes
{
    public sealed class FakeClock : Clock
    {
        public FakeClock(DateTime? now = null)
        {
            this.now = now ?? DateTime.Now;
        }

        private DateTime now;

        public DateTime Now() => now;

        public DateTime Today() => now.Date;

        public void Set(DateTime now) => this.now = now;

        public void Add(TimeSpan timeSpan) => now = now.Add(timeSpan);

        public override string ToString() => $"{nameof(FakeClock)} {now:M/dd/yy h:mm:ss tt}";
    }
}
