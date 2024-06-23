using System;
using System.IO;
using System.Linq;

namespace DevEnvEngine;

internal static class DevEnvEngine
{
    const string DEV_REPOS_ENV_VAR = "DEV_REPOS";

    public static int FnAddRepo(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("A name to identify the repo and the path where it"
                              + " is located are required.");
            return -1;
        }

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

        foreach (string r in repos.Split(':'))
        {
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

        // Two possibilities:
        // 1)- Get the key of an already stored repo.
        // 2)- Get the path of a repo, and optionally a key to store it.

        // For number 1:
        string key = args[0];

        return 0;
    }
}
