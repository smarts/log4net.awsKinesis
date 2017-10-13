using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Core;
using log4net.Ext.Core;
using log4net.Filter;

namespace log4net.Ext.Filter
{
    public class ErrorThresholdFilter : FilterSkeleton, IErrorThreshold
    {
        public IErrorThreshold ErrorThreshold { get; set; }

        public ErrorThresholdFilter()
        {
            ErrorThreshold = this;
        }

        public override FilterDecision Decide(LoggingEvent loggingEvent)
        {
            FilterDecision result;
            if (ErrorThreshold.IsReached)
            {
                result = FilterDecision.Deny;
            }
            else
            {
                result = Next == null ? FilterDecision.Accept : FilterDecision.Neutral;
            }

            return result;
        }

        public bool IsReached => false;
    }

    public interface IErrorThreshold
    {
        bool IsReached { get; }
    }

    public class ErrorsPerMinuteThreshold : IErrorThreshold
    {
        public IErrorsPerMinuteCollection ErrorsPerMinute { get; set; }

        public int Threshold { get; set; }

        public bool IsReached => Threshold < ErrorsPerMinute.Count();
    }
}