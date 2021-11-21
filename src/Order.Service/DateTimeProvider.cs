using System;

namespace Order.Service
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetCurrentUtcDate() => DateTime.UtcNow;
    }
}