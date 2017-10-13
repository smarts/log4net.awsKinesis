using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Appender;
using log4net.Ext.Core;
using log4net.Ext.Filter;

namespace log4net.Ext.Appender
{
    public class CircuitBreakerForwardingAppender : ForwardingAppender
    {
        private readonly CacheErrorsPerMinuteCollection errorsPerMinute;

        private int allowedErrorsPerMinute;

        public int AllowedErrorsPerMinute
        {
            get => allowedErrorsPerMinute;
            set
            {
                AddFilter(new ErrorThresholdFilter
                {
                    ErrorThreshold = new ErrorsPerMinuteThreshold
                    {
                        ErrorsPerMinute = errorsPerMinute,
                        Threshold = value
                    }
                });

                allowedErrorsPerMinute = value;
            }
        }

        public CircuitBreakerForwardingAppender()
        {
            errorsPerMinute = new CacheErrorsPerMinuteCollection();

            base.ErrorHandler = new CountingErrorHandler
            {
                ErrorsPerMinute = errorsPerMinute
            };
        }
    }
}
