using log4net.Ext.Appender;

namespace log4net.Ext.Tests.Appender
{
    internal class TestAwsKinesisAppender : AwsKinesisAppender
    {
        protected override bool RequiresLayout
        {
            get { return base.RequiresLayout; }
        }

        public bool ExposedRequiresLayout
        {
            get { return this.RequiresLayout; }
        }
    }
}