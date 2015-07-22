using System;
using System.Collections.Concurrent;

namespace log4net.Ext.ErrorHandler
{
    /// <summary>
    /// Singleton wrapper for the dictionary object used by CircuitBreakerErrorHandler
    /// </summary>
    internal class ErrorDictionary : ConcurrentDictionary<int, int>
    {
        private static readonly Lazy<ErrorDictionary> Lazy = new Lazy<ErrorDictionary>(() => new ErrorDictionary());

        public static ErrorDictionary Instance { get { return Lazy.Value; } }

        private ErrorDictionary()
        {
        }
    }
}
