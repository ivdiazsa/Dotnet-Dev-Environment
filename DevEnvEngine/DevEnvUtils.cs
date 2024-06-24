using System;
using System.Linq;

namespace DevEnv;

internal static class DevEnvUtils
{
    public static bool IsRepoRegistered(string repoKey)
    {
        return !string.IsNullOrEmpty(GetRepoFromKey(repoKey));
    }

    public static string GetRepoFromKey(string repoKey)
    {
        string regRepos = Environment.GetEnvironmentVariable(
            DevEnvEngine.DEV_REPOS_ENV_VAR);

        if (string.IsNullOrEmpty(regRepos))
            return string.Empty;

        string[] repoInfo = regRepos.Split(';')
                                    .Select(r => r.Split(','))
                                    .FirstOrDefault(rInfo => rInfo[0] == repoKey);

        return repoInfo is null ? string.Empty : repoInfo[1];
    }
}
