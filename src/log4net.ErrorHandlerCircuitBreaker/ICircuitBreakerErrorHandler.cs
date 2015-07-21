using log4net.Core;

namespace log4net.Ext.ErrorHandler
{
    public interface ICircuitBreakerErrorHandler : IErrorHandler
    {
        bool IsTripped { get; }
    }
}