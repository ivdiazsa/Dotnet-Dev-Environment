using System;
using System.Collections.Generic;
using System.IO;

namespace DevEnvEngine;

public class DevEnvEngine
{
    const string DEV_REPOS_ENV_VAR = "DEV_REPOS";

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
}
