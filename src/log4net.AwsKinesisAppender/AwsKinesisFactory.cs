using Amazon;
using Amazon.Kinesis;
using log4net.Core;
using log4net.Ext.ErrorHandler;

namespace log4net.Ext.Appender.AwsKinesis
{
    public class AwsKinesisFactory : IAwsKinesisFactory
    {
        public IAmazonKinesis Create(IErrorHandler errorHandler)
        {
            return IsCircuitBreakerTripped(errorHandler)
                ? new NoOpAmazonKinesisClient()
                : AWSClientFactory.CreateAmazonKinesisClient();
        }

        private static bool IsCircuitBreakerTripped(IErrorHandler errorHandler)
        {
            var circuitBreaker = errorHandler as ICircuitBreakerErrorHandler;
            return circuitBreaker != null && circuitBreaker.IsTripped;
        }
    }
}