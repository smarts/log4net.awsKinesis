using log4net.Appender;
using log4net.Core;

namespace log4net.Ext.Appender
{
    public class AwsKinesisAppender : AppenderSkeleton
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            // TODO: forward event to AWS Kinesis
        }
    }
}