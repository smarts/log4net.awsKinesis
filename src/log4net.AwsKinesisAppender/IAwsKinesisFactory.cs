using Amazon.Kinesis;
using log4net.Core;

namespace log4net.Ext.Appender.AwsKinesis
{
    public interface IAwsKinesisFactory
    {
        IAmazonKinesis Create(IErrorHandler errorHandler);
    }
}