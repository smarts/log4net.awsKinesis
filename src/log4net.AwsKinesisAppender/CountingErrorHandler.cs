using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net.Core;

namespace log4net.Ext.Core
{
    public class CountingErrorHandler : IErrorHandler
    {
        public IErrorsPerMinuteCollection ErrorsPerMinute { get; set; }

        public CountingErrorHandler()
        {
            ErrorsPerMinute = new CacheErrorsPerMinuteCollection();
        }

        public void Error(string message) =>
            Error(message, null);

        public void Error(string message, Exception e) =>
            Error(message, e, ErrorCode.GenericFailure);

        public void Error(string message, Exception e, ErrorCode errorCode) =>
            ErrorsPerMinute.Increment();
    }

    public interface IErrorsPerMinuteCollection
    {
        int Count(int? minute = null);

        IErrorsPerMinuteCollection Increment();
    }

    public class CacheErrorsPerMinuteCollection : IErrorsPerMinuteCollection
    {
        public ObjectCache Cache { get; set; }

        public ITime Time { get; set; }

        public string CacheRegion { get; set; }

        public CacheErrorsPerMinuteCollection()
        {
            Cache = MemoryCache.Default;
            Time = new SystemTime();
            CacheRegion = base.GetHashCode().ToString();
        }

        public int Count(int? minute = null)
        {
            var key = Key(minute ?? Time.UtcNow.Minute);

            return (Cache.Get(key) as AtomicInteger)?.Value ?? 0;
        }

        public IErrorsPerMinuteCollection Increment()
        {
            var cacheItem = new CacheItem(Key(Time.UtcNow.Minute), new AtomicInteger(1));
            cacheItem = Cache.AddOrGetExisting(cacheItem, new CacheItemPolicy
            {
                AbsoluteExpiration = Time.UtcNow.AddMinutes(2)
            });

            if (cacheItem != null)
            {
                ((AtomicInteger) cacheItem.Value).Increment();
            }

            return this;
        }

        private string Key(int minute) =>
            String.Join(".", GetType().FullName, CacheRegion, minute);

        private class AtomicInteger
        {
            private int value;

            public AtomicInteger(int value)
            {
                this.value = value;
            }

            public int Increment() => Interlocked.Increment(ref value);

            public int Value => Interlocked.CompareExchange(ref value, 0, 0);
        }
    }

    public interface ITime
    {
        DateTime UtcNow { get; }
    }

    public class SystemTime : ITime
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}