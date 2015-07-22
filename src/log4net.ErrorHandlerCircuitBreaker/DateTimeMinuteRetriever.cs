using System;

namespace log4net.Ext.ErrorHandler
{
    // Wrapper for DateTime operation to allow for unit test mocking
    internal class DateTimeMinuteRetriever : IDateTimeMinuteRetriever
    {
        public int GetMinute()
        {
            return DateTime.Now.Minute;
        }
    }
}
