using System;

namespace Order.Service
{
    public interface IDateTimeProvider
    {
        DateTime GetCurrentUtcDate();
    }
}