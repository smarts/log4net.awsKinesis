using System;
using System.Linq;
using log4net.Core;
using log4net.Util;

namespace log4net.Ext.ErrorHandler
{
    public class CircuitBreakerErrorHandler : ICircuitBreakerErrorHandler
    {
        private readonly ErrorDictionary errorByMinute;

        private static readonly object Lock = new object();
        private static int currentMinute;

        public int TripErrorCountPerMinute { get; set; }

        public IDateTimeMinuteRetriever MinuteRetriever { get; set; }

        public IErrorHandler InnerErrorHandler { get; set; }

        public CircuitBreakerErrorHandler()
        {
            errorByMinute = ErrorDictionary.Instance;

            // Defaults
            TripErrorCountPerMinute = 10;
            MinuteRetriever = new DateTimeMinuteRetriever();
            InnerErrorHandler = new OnlyOnceErrorHandler();
        }

        public bool IsTripped
        {
            get
            {
                var minuteKey = GetMinute();

                int errorCount;
                var errorsFound = errorByMinute.TryGetValue(minuteKey, out errorCount);

                return errorsFound && errorCount >= TripErrorCountPerMinute;
            }
        }

        public void Error(string message, Exception e, ErrorCode errorCode)
        {
            IncrementError();
            if (InnerErrorHandler != null)
            {
                InnerErrorHandler.Error(message, e, errorCode);
            }
        }

        public void Error(string message, Exception e)
        {
            IncrementError();
            if (InnerErrorHandler != null)
            {
                InnerErrorHandler.Error(message, e);
            }
        }

        public void Error(string message)
        {
            IncrementError();
            if (InnerErrorHandler != null)
            {
                InnerErrorHandler.Error(message);
            }
        }

        private void IncrementError()
        {
            var minuteKey = GetMinute();

            errorByMinute.AddOrUpdate(minuteKey, 1, (key, value) => value + 1);
        }

        private int GetMinute()
        {
            var newMinute = MinuteRetriever.GetMinute();

            lock (Lock)
            {
                if (newMinute == currentMinute)
                {
                    return currentMinute;
                }
                currentMinute = newMinute;
            }

            var keys = errorByMinute.Keys.Where(x => x != newMinute && x != newMinute - 1 && x != newMinute + 59);
            foreach (var key in keys)
            {
                int throwaway;
                errorByMinute.TryRemove(key, out throwaway);
            }
            return newMinute;
        }
    }
}
