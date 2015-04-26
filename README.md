# log4net.Ext.AwsKinesis
A log4net appender that writes to an AWS Kinesis stream.

## Getting Started
### Installing
Get the NuGet package: `log4net.Ext.AwsKinesis`

### Configuring
The library takes advantage of the AWS and log4net configurations. A basic setup might look like the following:
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net" />
    <section name="aws" type="Amazon.AWSSection, AWSSDK" />
  </configSections>
  <aws profileName="development" />
  <log4net>
    <appender name="AwsKinesis" type="log4net.Ext.Appender.AwsKinesisAppender, log4net.Ext.AwsKinesis">
      <streamName value="MyStream" />
      <layout type="log4net.Layout.PatternLayout">
        <conversionPattern value="%date [%thread] %-5level %logger [%property{NDC}] - %message" />
      </layout>
    </appender>
    <root>
      <appender-ref ref="AwsKinesis" />
    </root>
  </log4net>
</configuration>
```
Configuration Links:
* [log4net](https://logging.apache.org/log4net/release/config-examples.html)
* [AWS .NET SDK Credentials](http://docs.aws.amazon.com/AWSSdkDocsNET/latest/DeveloperGuide/net-dg-config-creds.html#net-dg-config-creds-assign)
