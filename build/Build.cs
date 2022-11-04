using System.Collections.Generic;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.Docker;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NerdbankGitVersioning;
using Serilog;
using static Nuke.Common.Tools.Docker.DockerTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    [GitRepository] private readonly GitRepository Repository;

    [Solution] private readonly Solution Solution;

    [NerdbankGitVersioning]
    private readonly NerdbankGitVersioning VersionInfo;

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    private readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    [Parameter] private readonly string ContainerRegistry;
    [Parameter] private readonly string DockerUsername;
    [Parameter] private readonly string DockerPassword;

    private string OutputDirectory => RootDirectory / ".output";
    private string Dockerfile => RootDirectory / "deploy" / "docker" / "Dockerfile";

    public static int Main() => Execute<Build>(x => x.Deploy);
    Target BuildInfo => _ => _
        .Executes(() =>
        {
            Log.Information("Solution: {Value}", Solution);
            Log.Information("SemVer2: {Value}", VersionInfo.SemVer2);
            Log.Information("Branch = {Value}", Repository.Branch);
            Log.Information("Commit = {Value}", Repository.Commit);
            Log.Information("Tags = {Value}", Repository.Tags);
        });

    Target Clean => _ => _
        .DependsOn(BuildInfo)
        .Executes(() =>
        {
            DotNetClean(_ => _.SetProject(Solution));
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(_ => _.SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target BuildDockerImages => _ => _
        .Requires(() => ContainerRegistry)
        .Requires(() => DockerUsername)
        .Requires(() => DockerPassword)
        .DependsOn(Compile)
        .Executes(() =>
        {
            DockerLogin(_ => _
                .SetServer(ContainerRegistry)
                .SetUsername(DockerUsername)
                .SetPassword(DockerPassword));

            const string ContainerNamespace = "pingpongmania";

            foreach (var (projectName, imageName) in ProjectsToDockerise())
            {
                var imageTag = $"{ContainerRegistry}/{ContainerNamespace}/{imageName}:{VersionInfo.SemVer2}";
                BuildDockerImageForService(projectName, imageTag);
            }
        });

    private IEnumerable<(string, string)> ProjectsToDockerise()
    {
        yield return ("PingService", "ping-service");
        yield return ("PongService", "pong-service");
        yield return ("PlayService", "play-service");
    }

    private void BuildDockerImageForService(string projectName, string imageTag)
    {
        var project = Solution.GetProject(projectName);
        var publishDirectory = $"{OutputDirectory}/{projectName}";


        DotNetPublish(_ => _
            .SetProject(project)
            .SetConfiguration(Configuration)
            .SetOutput(publishDirectory)
            .EnableNoBuild());

        DockerBuild(_ => _
            .AddBuildArg($"ASSEMBLY_NAME={projectName}.dll")
            .SetPath(publishDirectory)
            .SetFile(Dockerfile)
            .SetTag(imageTag));

        DockerPush(_ => _.SetName(imageTag));
    }

    Target Deploy => _ => _.DependsOn(BuildDockerImages);
}
