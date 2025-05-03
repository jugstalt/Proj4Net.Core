using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Proj4Net.Core;
using Serilog;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.Deploy);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Versions‑/Build‑Label")]
    readonly string Version = typeof(CoordinateReferenceSystem).Assembly.GetName().Version.ToString();
    [Parameter("Platform to build - win-x64/linux-x64")]
    readonly string Platform = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
        ? "linux-x64"
        : "win-x64";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
        });

    Target Restore => _ => _
        .Executes(() =>
        {
        });

    Target Test => _ => _
        .Before(Deploy)
        .Executes(() =>
        {
            Log.Information($"Run tests for WebGIS {Version} on platform {Platform}");

            DotNetTasks.DotNetTest(s => s
                .SetProjectFile(RootDirectory / "src" / "Proj4Net.Core.Tests" / "Proj4Net.Core.Tests.csproj")
                .SetProcessWorkingDirectory(RootDirectory)
                .SetVerbosity(DotNetVerbosity.minimal)
            );
        });

    Target Deploy => _ => _
        .DependsOn(Restore)
        .DependsOn(Test)
        .Executes(() =>
        {
            foreach (var platform in new[] { "win-x64", "linux-x64" })
            {
                foreach (var configuration in new[] { "Release", "Debug" })
                {
                    Log.Information($"Deploy Cs2Cs.exe {Version} for platform {platform}-{configuration}");

                    (RootDirectory / "publish" / "cs2cs" / platform / configuration).DeleteDirectory();
                    DotNetTasks.DotNetPublish(s => s
                        .SetProject(RootDirectory / "src" / "Cs2Cs.Core" / "Cs2Cs.Core.csproj")
                        .SetConfiguration(configuration)
                        .SetProperty("DeployOnBuild", "true")
                        //.SetOutputDirectory(RootDirectory / "publish" / "cs2cs" / platform / configuration)
                        .SetPublishProfile($"{platform}-{configuration.ToLower()}")
                    //.SetRuntime(platform)
                    //.EnableNoRestore()
                    );
                }
            }

            // win-x64 => zip folder
            (RootDirectory / "publish" / "cs2cs" / "win-x64" / "zip").DeleteDirectory();
            (RootDirectory / "publish" / "cs2cs" / "win-x64" / "zip").CreateDirectory();
            (RootDirectory / "publish" / "cs2cs" / "win-x64" / "debug" / "Cs2Cs.Core.exe")
                 .CopyToDirectory(RootDirectory / "publish" / "cs2cs" / "win-x64" / "zip")
                 .Rename("Cs2Cs.Core-debug.exe");
            (RootDirectory / "publish" / "cs2cs" / "win-x64" / "release" / "Cs2Cs.Core.exe")
                .CopyToDirectory(RootDirectory / "publish" / "cs2cs" / "win-x64" / "zip");

            // linux-x64 => zip folder
            (RootDirectory / "publish" / "cs2cs" / "linux-x64" / "zip").DeleteDirectory();
            (RootDirectory / "publish" / "cs2cs" / "linux-x64" / "zip").CreateDirectory();
            (RootDirectory / "publish" / "cs2cs" / "linux-x64" / "debug" / "Cs2Cs.Core.exe")
                 .CopyToDirectory(RootDirectory / "publish" / "cs2cs" / "linux-x64" / "zip")
                 .Rename("cs2cs.core-debug");
            (RootDirectory / "publish" / "cs2cs" / "linux-x64" / "release" / "Cs2Cs.Core.exe")
                .CopyToDirectory(RootDirectory / "publish" / "cs2cs" / "linux-x64" / "zip")
                .Rename("cs2cs.core");

            // zip win-x64
            (RootDirectory / "publish" / "cs2cs" / $"cs2cs-win-x64-{Version}.zip").DeleteFile();
            (RootDirectory / "publish" / "cs2cs" / "win-x64" / "zip").ZipTo(
                RootDirectory / "publish" / "cs2cs" / $"cs2cs-win-x64-{Version}.zip",
                compressionLevel: CompressionLevel.SmallestSize,
                fileMode: FileMode.CreateNew);

            // zip linux-x64
            (RootDirectory / "publish" / "cs2cs" / $"cs2cs-linux-x64-{Version}.zip").DeleteFile();
            (RootDirectory / "publish" / "cs2cs" / "linux-x64" / "zip").ZipTo(
                RootDirectory / "publish" / "cs2cs" / $"cs2cs-linux-x64-{Version}.zip",
                compressionLevel: CompressionLevel.SmallestSize,
                fileMode: FileMode.CreateNew);
        });

}
