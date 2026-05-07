using System;

namespace Dommel.IntegrationTests;

public static class CI
{
    public static bool IsAppVeyor => EnvBool("APPVEYOR");

    public static bool IsTravis => EnvBool("TRAVIS");

    public static bool IsGitHubActions => EnvBool("GITHUB_ACTIONS");

    private static bool EnvBool(string env) => bool.TryParse(Environment.GetEnvironmentVariable(env), out var b) && b;
}
