using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using log4net.Appender;
using log4net.Core;
using log4net.Ext.Appender.AwsKinesis;
using log4net.Ext.Appender.Resources;

namespace log4net.Ext.Appender
{
    public class AwsKinesisAppender : AppenderSkeleton
    {
        private IAmazonKinesis awsKinesis;

        public string StreamName { get; set; }

        public IAwsKinesisFactory ClientFactory { get; set; }

        protected override bool RequiresLayout
        {
            get { return true; }
        }

        public AwsKinesisAppender()
        {
            this.StreamName = String.Empty;

            this.ClientFactory = new AwsKinesisFactory();
        }

        public override void ActivateOptions()
        {
            base.ActivateOptions();

            DisposeThenInitializeClient();
        }

        private void DisposeThenInitializeClient()
        {
            DisposeClient();

            InitializeClient();
        }

        private void InitializeClient()
        {
            try
            {
                awsKinesis = ClientFactory.Create();
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(Resource.AwsKinesisClientCreationError, ex);
            }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            var request = CreateRequest(loggingEvent);

            awsKinesis.PutRecordAsync(request)
                .ContinueWith(HandleError, TaskContinuationOptions.OnlyOnFaulted);
        }

        private PutRecordRequest CreateRequest(LoggingEvent loggingEvent)
        {
            return new PutRecordRequest
            {
                StreamName = this.StreamName,
                Data = Stream(loggingEvent),
                PartitionKey = Guid.NewGuid().ToString()
            };
        }

        private MemoryStream Stream(LoggingEvent loggingEvent)
        {
            MemoryStream result = new MemoryStream();

            try
            {
                using (var writer = new StreamWriter(result, Encoding.UTF8, bufferSize: 1024, leaveOpen: true))
                {
                    Layout.Format(writer, loggingEvent);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(String.Format(Resource.LayoutFormatStreamWriteErrorFormat, Layout.GetType(), loggingEvent), ex);

                result = new MemoryStream(0);
            }

            return result;
        }

        private void HandleError(Task task)
        {
            ErrorHandler.Error(Resource.AwsKinesisSendError, task.Exception);
        }

        protected override void OnClose()
        {
            base.OnClose();

            DisposeClient();
        }

        private void DisposeClient()
        {
            using (awsKinesis) { }

            awsKinesis = null;
        }
    }
}