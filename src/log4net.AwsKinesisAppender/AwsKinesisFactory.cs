using Amazon;
using Amazon.Kinesis;

namespace log4net.Ext.Appender.AwsKinesis
{
    internal class AwsKinesisFactory : IAwsKinesisFactory
    {
        public IAmazonKinesis Create()
        {
            return AWSClientFactory.CreateAmazonKinesisClient();
        }
    }
}