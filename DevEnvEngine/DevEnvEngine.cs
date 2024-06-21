using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DevEnvEngine;

public class DevEnvEngine
{
    const string DEV_REPOS_ENV_VAR = "DEV_REPOS";
    const string WORK_REPO_ENV_VAR = "WORK_REPO";

    static int Main(string[] args)
    {
        Dictionary<string, string> engParams = ArgsParser.Run(args);

        switch (engParams["command"])
        {
            case "add_repo":
                FnAddRepo(engParams["name"], engParams["path"]);
                break;

            case "list_repos":
                FnListRepos();
                break;

            case "build_runtime":
                FnBuildRuntime(engParams["repo"], engParams["args"]);
                break;

            case "set_repo":
                FnSetRepo(engParams["name"]);
                break;
        }

        return 0;
    }

    static void FnAddRepo(string name, string path)
    {
        string lName = name.ToLower();
        string absPath = Path.GetFullPath(path);
        Console.WriteLine($"{lName},\"{absPath}\"");
    }

    static void FnListRepos()
    {
        string repos = Environment.GetEnvironmentVariable(DEV_REPOS_ENV_VAR);

        if (string.IsNullOrWhiteSpace(repos))
        {
            Console.WriteLine("No repos have been added yet!");
            return ;
        }

        foreach (string r in repos.Split(':'))
        {
            string[] rinfo = r.Split(',');
            Console.WriteLine($"{rinfo[0]}, {rinfo[1]}");
        }
    }

    static void FnSetRepo(string repoName)
    {
        IEnumerable<string[]> repos =
            Environment.GetEnvironmentVariable(DEV_REPOS_ENV_VAR)
                       .Split(':')
                       .Select(r => r.Split(','));

        string setPath = repos.FirstOrDefault(r => r[0] == repoName.ToLower())[1];
        Console.WriteLine(setPath);
    }

    static void FnBuildRuntime(string repo, string buildArgs)
    {
        string root = !string.IsNullOrEmpty(repo)
            ? repo
            : Environment.GetEnvironmentVariable(WORK_REPO_ENV_VAR);

        Console.WriteLine($"\"{repo}/build.sh\" {buildArgs}");
    }

    static void FnBuildTests(string repo)
    {
        return ;
    }

    static void FnGenerateLayout(string repo)
    {
        return ;
    }
}
