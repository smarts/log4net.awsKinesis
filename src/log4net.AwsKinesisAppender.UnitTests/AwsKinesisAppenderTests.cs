using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using log4net.Core;
using log4net.Ext.Appender;
using log4net.Ext.Appender.AwsKinesis;
using log4net.Layout;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace log4net.Ext.Tests.Appender
{
    [TestClass]
    public class AwsKinesisAppenderTests
    {
        [TestMethod]
        public void Ctor_SetsStreamNameToEmptyString()
        {
            // Arrange
            AwsKinesisAppender target;

            // Act
            target = new AwsKinesisAppender();

            // Assert
            Assert.AreEqual(String.Empty, target.StreamName);
        }

        [TestMethod]
        public void Ctor_SetsClientFactoryToInstanceOfDefaultType()
        {
            // Arrange
            AwsKinesisAppender target;

            // Act
            target = new AwsKinesisAppender();

            // Assert
            var clientFactory = target.ClientFactory;

            Assert.IsNotNull(clientFactory);

            Assert.AreEqual(clientFactory.GetType().ToString(), "log4net.Ext.Appender.AwsKinesis.AwsKinesisFactory");
        }

        [TestMethod]
        public void RequiresLayout_ReturnsTrue()
        {
            // Arrange
            var target = new TestAwsKinesisAppender();

            // Act
            var result = target.ExposedRequiresLayout;

            // Assert
            Assert.IsTrue(result, "RequiresLayout property should be true");
        }

        [TestMethod]
        public void ActivateOptions_DoesNotThrowException_IfClientIsNull()
        {
            // Arrange
            var target = AwsKinesisAppender();

            // Act
            target.ActivateOptions();

            // Assert
            Mock.Get(target.ErrorHandler).Verify(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never());
        }

        [TestMethod]
        public void ActivateOptions_DisposesClient()
        {
            // Arrange
            var clientFactory = Mock.Of<IAwsKinesisFactory>(x => x.Create() == Mock.Of<IAmazonKinesis>());

            var target = AwsKinesisAppender(clientFactory: clientFactory);

            // Act
            // this call sets the client to non-null value
            target.ActivateOptions();

            // this call resets the client by disposing the current non-null value
            target.ActivateOptions();

            // Assert
            Mock.Get(clientFactory.Create()).Verify(x => x.Dispose(), Times.Once());
        }

        [TestMethod]
        public void ActivateOptions_HandlesClientDisposalError()
        {
            // Arrange
            var client = new Mock<IAmazonKinesis>();
            client.Setup(x => x.Dispose()).Throws(new Exception());

            var clientFactory = Mock.Of<IAwsKinesisFactory>(x => x.Create() == client.Object);

            var target = AwsKinesisAppender(clientFactory: clientFactory);

            // Act
            // this call sets the client to non-null value
            target.ActivateOptions();

            // this call resets the client by disposing the current non-null value
            target.ActivateOptions();

            // Assert
            Mock.Get(target.ErrorHandler).Verify(x => x.Error(It.Is<string>(y => y.Contains("disposing")), It.IsAny<Exception>()), Times.Once());
        }

        [TestMethod]
        public void ActivateOptions_CreatesClient()
        {
            // Arrange
            var target = AwsKinesisAppender();

            // Act
            target.ActivateOptions();

            // Assert
            Mock.Get(target.ClientFactory).Verify(x => x.Create(), Times.Once());
        }

        [TestMethod]
        public void Close_DoesNotThrowException_IfClientIsNull()
        {
            // Arrange
            var mockErrorHandler = new Mock<IErrorHandler>();

            AwsKinesisAppender target = AwsKinesisAppender(errorHandler: mockErrorHandler.Object);

            // Act
            target.Close();

            // Assert
            mockErrorHandler.Verify(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never());
        }

        [TestMethod]
        public void DoAppend_UsesStreamNameInAwsKinesisRequest()
        {
            // Arrange
            var client = Mock.Of<IAmazonKinesis>(y => y.PutRecordAsync(It.IsAny<PutRecordRequest>(), default(CancellationToken)) == Task.FromResult(new PutRecordResponse()));

            var clientFactory = Mock.Of<IAwsKinesisFactory>(x => x.Create() == client);

            var streamName = "foo";

            var target = AwsKinesisAppender(streamName, clientFactory: clientFactory);

            target.Layout = Mock.Of<ILayout>();

            target.ActivateOptions();

            // Act
            target.DoAppend(new LoggingEvent(new LoggingEventData()));

            // Assert
            Mock.Get(client).Verify(x =>
                x.PutRecordAsync(It.Is<PutRecordRequest>(y => y.StreamName == streamName), default(CancellationToken)), Times.Once());
        }

        [TestMethod]
        public void DoAppend_HandlesLayoutFormatError()
        {
            // Arrange
            var clientFactory = Mock.Of<IAwsKinesisFactory>(x => 
                x.Create() ==
                Mock.Of<IAmazonKinesis>(y =>
                    y.PutRecordAsync(It.IsAny<PutRecordRequest>(), default(CancellationToken)) ==
                    Task.FromResult(new PutRecordResponse())));

            var target = AwsKinesisAppender(clientFactory: clientFactory);

            var layout = new Mock<ILayout>();
            layout.Setup(x => x.Format(It.IsAny<TextWriter>(), It.IsAny<LoggingEvent>()))
                .Throws(new Exception());

            target.Layout = layout.Object;

            target.ActivateOptions();

            // Act
            target.DoAppend(new LoggingEvent(new LoggingEventData()));

            // Assert
            Mock.Get(target.ErrorHandler).Verify(x => x.Error(It.Is<string>(y => y.Contains(layout.Object.GetType().ToString())), It.IsAny<Exception>()), Times.Once());
        }

        [TestMethod]
        public void DoAppend_HandlesAwsKinesisSendError()
        {
            // Arrange
            Task<PutRecordResponse> task = null;

            var client = new Mock<IAmazonKinesis>();
            client.Setup(x => x.PutRecordAsync(It.IsAny<PutRecordRequest>(), default(CancellationToken)))
                .Returns(() => { task = new Task<PutRecordResponse>(() => { throw new Exception(); }); task.Start(); return task; });

            var clientFactory = Mock.Of<IAwsKinesisFactory>(x => x.Create() == client.Object);

            var target = AwsKinesisAppender(clientFactory: clientFactory);

            target.Layout = Mock.Of<ILayout>();

            target.ActivateOptions();

            // Act
            target.DoAppend(new LoggingEvent(new LoggingEventData()));

            // force the task and all continuations to execute
            try
            {
                task.Wait();
            }
            catch (Exception)
            {
            }

            // Assert
            Mock.Get(target.Layout).Verify(x => x.Format(It.IsAny<TextWriter>(), It.IsAny<LoggingEvent>()), Times.Once());

            Mock.Get(target.ErrorHandler).Verify(x => x.Error(It.Is<string>(y => y.Contains("sending")), It.IsAny<Exception>()), Times.Once());
        }

        private static AwsKinesisAppender AwsKinesisAppender(string streamName = null, IAwsKinesisFactory clientFactory = null, IErrorHandler errorHandler = null)
        {
            var result = new AwsKinesisAppender();

            if (streamName != null)
            {
                result.StreamName = streamName;
            }

            result.ClientFactory = clientFactory ?? Mock.Of<IAwsKinesisFactory>();

            result.ErrorHandler = errorHandler ?? Mock.Of<IErrorHandler>();

            return result;
        }
    }
}