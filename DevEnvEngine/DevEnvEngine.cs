using System;
using System.IO;
using System.Linq;

namespace DevEnvEngine;

internal static class DevEnvEngine
{
    const string DEV_REPOS_ENV_VAR = "DEV_REPOS";
    const string WORK_REPO_ENV_VAR = "WORK_REPO";

    public static int FnAddRepo(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("A name to identify the repo and the path where it"
                              + " is located are required.");
            return -1;
        }

        // FIXME: Error out if the given key already exists.

        string key = args[0].ToLower();
        string path = Path.GetFullPath(args[1]);

        if (!Directory.Exists(path))
        {
            Console.WriteLine($"The given path '{path}' was not found :(");
            return -1;
        }

        Console.WriteLine($"{key},\"{path}\"");
        return 0;
    }

    public static void FnListRepos()
    {
        string repos = Environment.GetEnvironmentVariable(DEV_REPOS_ENV_VAR);

        if (string.IsNullOrWhiteSpace(repos))
        {
            Console.WriteLine("No repos have been added yet!");
            return ;
        }

        // The DEV_REPOS environment variable stores the added repos in a similar
        // fashion to Unix's PATH environment variable. Each entry follows the
        // 'key,path' format, and a colon ':' is used as the entries separator.

        foreach (string r in repos.Split(':'))
        {
            // Each entry is already divided by a comma ',', but we do this
            // processing, just to print them in a prettier format. Coming soon,
            // we will print them in a small table instead.

            string[] repoInfo = r.Split(',');
            string repoKey = repoInfo[0];
            string repoPath = repoInfo[1];
            Console.WriteLine($"{repoKey}, {repoPath}");
        }
    }

    public static int FnSetRepo(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("The path to the repo, or its key if it has been"
                              + " added previously with 'addrepo', is required.");
            return -1;
        }

        // If the parameter we received is an existing path, then we assume that's
        // the repo source the user wants to set as active. Otherwise, we treat it
        // as a key to search in the list of added repos.

        if (Directory.Exists(args[0]))
        {
            Console.WriteLine(args[0]);
            return 0;
        }

        string reposStr = Environment.GetEnvironmentVariable(DEV_REPOS_ENV_VAR);

        if (string.IsNullOrEmpty(reposStr))
        {
            Console.WriteLine("No repos have been added yet, so finding by key"
                              + " is not possible right now. Try registering it"
                              + " first by calling 'addrepo' with its data.");
            return -1;
        }

        string[] repoInfo = reposStr.Split(':')
                                    .Select(r => r.Split(','))
                                    .FirstOrDefault(rInfo => rInfo[0] == args[0]);

        string repoPath = repoInfo is null ? string.Empty : repoInfo[1];

        if (string.IsNullOrEmpty(repoPath))
        {
            Console.WriteLine($"The given key '{args[0]}' was not found in"
                              + " the DEV_REPOS env var. Make sure to add it"
                              + " first by calling 'addrepo' with its data.");
            return -1;
        }

        Console.WriteLine(repoPath);
        return 0;
    }

    public static void FnBuildSubsets(string[] args)
    {
        string workRepo = Environment.GetEnvironmentVariable(WORK_REPO_ENV_VAR);
        string buildArgs = string.Join(' ', args);

        if (!string.IsNullOrWhiteSpace(buildArgs))
            buildArgs = $" {buildArgs}";

        Console.WriteLine($"{workRepo}/build.sh{buildArgs}");
    }

    public static void FnGenerateLayout(string[] args)
    {
        string workRepo = Environment.GetEnvironmentVariable(WORK_REPO_ENV_VAR);
        string buildArgs = string.Join(' ', args);

        if (!string.IsNullOrWhiteSpace(buildArgs))
            buildArgs = $" {buildArgs}";

        Console.WriteLine($"{workRepo}/src/tests/build.sh{buildArgs}");
    }
}
