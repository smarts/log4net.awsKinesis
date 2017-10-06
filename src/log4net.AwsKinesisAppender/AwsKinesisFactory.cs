using Amazon;
using Amazon.Kinesis;

namespace log4net.Ext.Appender.AwsKinesis
{
    public class AwsKinesisFactory : IAwsKinesisFactory
    {
        public IAmazonKinesis Create()
        {
            return new AmazonKinesisClient();
        }
    }
}