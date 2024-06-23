using System;

using DevEnv = DevEnvEngine.DevEnvEngine;

public class Program
{
    static int Main(string[] args)
    {
        string command = args[0];
        int exitCode = 0;

        switch (command)
        {
            case "add_repo":
                exitCode = DevEnv.FnAddRepo(args[1..]);
                break;

            case "list_repos":
                DevEnv.FnListRepos();
                break;

            case "set_repo":
                exitCode = DevEnv.FnSetRepo(args[1..]);
                break;

            default:
                Console.WriteLine($"Command '{command}' was not recognized.");
                exitCode = -2;
                break;
        }

        return exitCode;
    }
}
