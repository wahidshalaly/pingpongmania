using Nuke.Common;
using Nuke.Common.ProjectModel;
using Serilog;
using Nuke.Common.Tools.NerdbankGitVersioning;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;



class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    private readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution]
    private readonly Solution Solution;

    [NerdbankGitVersioning]
    private readonly NerdbankGitVersioning NerdbankGitVersioning;


    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            DotNetClean(_ => _.SetProject(Solution));
        });

    Target Restore => _ => _
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

    Target Print => _ => _
        .Executes(() =>
        {
            Log.Information("Solution: {Value}", Solution);
            Log.Information("SemVer2: {Value}", NerdbankGitVersioning.CloudBuildNumber);
        });
}
