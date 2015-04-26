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
    /// <summary>
    /// Appender that logs to an AWS Kinesis stream.
    /// </summary>
    /// <remarks>
    /// See http://docs.aws.amazon.com/AWSSdkDocsNET/latest/DeveloperGuide/net-dg-config.html
    /// for information on how to configure the AWS SDK.
    /// </remarks>
    public class AwsKinesisAppender : AppenderSkeleton
    {
        private IAmazonKinesis awsKinesis;

        /// <summary>
        /// The name of the AWS Kinesis stream to which this appender will send log events.
        /// </summary>
        public string StreamName { get; set; }

        /// <summary>
        /// The factory that creates the AWS Kinesis client.
        /// </summary>
        public IAwsKinesisFactory ClientFactory { get; set; }

        protected override bool RequiresLayout
        {
            get { return true; }
        }

        /// <summary>
        /// Initializes an instance of <see cref="AwsKinesisAppender"/> with the empty string as
        /// the AWS Kinesis stream name and the default implementation of the AWS Kinesis factory.
        /// </summary>
        public AwsKinesisAppender()
        {
            StreamName = String.Empty;

            ClientFactory = new AwsKinesisFactory();
        }

        /// <summary>
        /// Intialize the appender based on the configured options.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is part of the <see cref="IOptionHandler"/> delayed object
        /// activation scheme. The <see cref="ActivateOptions"/> method must 
        /// be called on this object after the configuration properties have
        /// been set. Until <see cref="ActivateOptions"/> is called this
        /// object is in an undefined state and must not be used. 
        /// </para>
        /// <para>
        /// If any of the configuration properties are modified then 
        /// <see cref="ActivateOptions"/> must be called again.
        /// </para>
        /// </remarks>
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
                StreamName = StreamName,
                Data = Stream(loggingEvent),
                PartitionKey = Guid.NewGuid().ToString()
            };
        }

        /// <summary>
        /// Writes a <see cref="LoggingEvent"/> to a <see cref="MemoryStream"/> in
        /// UTF-8 encoding using <see cref="Layout"/>.
        /// </summary>
        /// <param name="loggingEvent">The logging event to be serialized.</param>
        /// <returns>
        /// A memory stream with the contents of <paramref name="loggingEvent"/>
        /// formatted using <see cref="Layout"/> in UTF-8.
        /// </returns>
        /// <remarks>
        /// Any exception thrown by downstream calls is passed to <see cref="ErrorHandler"/>.
        /// </remarks>
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
            try
            {
                using (awsKinesis) { }

                awsKinesis = null;
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(Resource.AwsKinesisClientDisposalError, ex);
            }
        }
    }
}