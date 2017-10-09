using Amazon.Kinesis;

namespace log4net.Ext.Appender.AwsKinesis
{
    public interface IAwsKinesisFactory
    {
        IAmazonKinesis Create();
    }
}