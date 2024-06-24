using System;
using DevEnv;

// ENHANCEMENT IDEAS:
//
// - Make a list of constants of different types of errors, rather than always
//   returning 255/-1.
//
// - Refactor some duplicated code and comment it as needed :)
//
// - Runtime specific: Handle the build script args here, rather than relying
//   on the not-always-friendly handling of the repo's build scripts.

public class Program
{
    static int Main(string[] args)
    {
        string command = args[0];
        int exitCode = 0;

        switch (command)
        {
            case "add_repo":
                exitCode = DevEnvEngine.FnAddRepo(args[1..]);
                break;

            case "list_repos":
                DevEnvEngine.FnListRepos();
                break;

            case "set_repo":
                exitCode = DevEnvEngine.FnSetRepo(args[1..]);
                break;

            // case "build_subsets":
            //     DevEnv.FnBuildSubsets(args[1..]);
            //     break;

            // case "generate_layout":
            //     DevEnv.FnGenerateLayout(args[1..]);
            //     break;

            default:
                Console.WriteLine($"Command '{command}' was not recognized.");
                exitCode = -2;
                break;
        }

        return exitCode;
    }
}
