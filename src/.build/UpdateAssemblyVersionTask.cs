using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Text.RegularExpressions;

public class UpdateAssemblyVersionTask : Task
{
    [Required]
    public ITaskItem VersionFile { get; set; }

    [Required]
    public int Revision { get; set; }

    public string PreReleaseSuffix { get; set; }

    public override bool Execute()
    {
        try
        {
            var path = VersionFile.GetMetadata("FullPath");

            // max version number (for any part of the version) is 65535
            const string maxVersionNumberRegex = @"(\d{1,4}|[1-5]\d{4}|6[0-4]\d{3}|65[0-4]\d{2}|655[0-2]\d|6553[0-5])";

            var versionAttributeRegex = String.Format(@"\[assembly\: AssemblyVersion\(""{0}\.{0}\.{0}\.{0}""\)\]",
                maxVersionNumberRegex);
            var versionInfoAttributeRegex = @"\[assembly\: AssemblyInformationalVersion\(""[A-Za-z0-9.\-]+?""\)\]";

            var fileText = File.ReadAllText(path);

            var versionPattern = String.Format(@"\[assembly\: AssemblyVersion\(""{0}\.{0}\.{0}\.{0}""\)\]",
                maxVersionNumberRegex);
            var match = Regex.Match(fileText, versionPattern, RegexOptions.Multiline);

            var major = int.Parse(match.Groups[1].Value);
            var minor = int.Parse(match.Groups[2].Value);
            var build = int.Parse(match.Groups[3].Value);

            Environment.SetEnvironmentVariable("VERSION_MAJOR", major.ToString());
            Environment.SetEnvironmentVariable("VERSION_MINOR", minor.ToString());
            Environment.SetEnvironmentVariable("VERSION_BUILD", build.ToString());

            var version = new Version(major, minor, build, Revision);
            var versionAttribute = String.Format(@"[assembly: AssemblyVersion(""{0}"")]", version);

            var resultText = Regex.Replace(fileText, versionAttributeRegex, versionAttribute, RegexOptions.Multiline);

            var infoVersion = new Version(major, minor, build);

            var versionInfoAttribute = String.Format(@"[assembly: AssemblyInformationalVersion(""{0}"")]",
                String.IsNullOrWhiteSpace(PreReleaseSuffix)
                    ? infoVersion.ToString()
                    : String.Format("{0}-{1}{2:d4}", infoVersion, PreReleaseSuffix, Revision));

            resultText = Regex.Replace(resultText, versionInfoAttributeRegex, versionInfoAttribute,
                RegexOptions.Multiline);

            File.WriteAllText(path, resultText);

            return true;
        }
        catch (Exception ex)
        {
            Log.LogErrorFromException(ex);

            return false;
        }
    }
}
