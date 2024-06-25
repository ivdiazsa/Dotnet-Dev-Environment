using System;
using System.Runtime.InteropServices;
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
            case "arch_setup":
                SetupArch();
                break;

            case "os_setup":
                SetupOS();
                break;

            case "add_repo":
                exitCode = DevEnvEngine.FnAddRepo(args[1..]);
                break;

            case "list_repos":
                DevEnvEngine.FnListRepos();
                break;

            case "set_repo":
                exitCode = DevEnvEngine.FnSetRepo(args[1..]);
                break;

            case "build_subsets":
                exitCode = DevEnvEngine.FnBuildSubset(args[1..]);
                break;

            case "generate_layout":
                exitCode = DevEnvEngine.FnGenerateLayout(args[1..]);
                break;

            case "build_clr_tests":
                exitCode = DevEnvEngine.FnBuildTest(args[1..]);
                break;

            case "find_test":
                DevEnvEngine.FnFindTest(args[1..]);
                break;

            default:
                Console.WriteLine($"Command '{command}' was not recognized.");
                exitCode = -2;
                break;
        }

        return exitCode;
    }

    private static void SetupArch()
    {
        Architecture arch = RuntimeInformation.OSArchitecture;
        Console.WriteLine(arch.ToString().ToLower());
    }

    private static void SetupOS()
    {
        PlatformID os = Environment.OSVersion.Platform;

        switch (os)
        {
            case PlatformID.Win32NT:
                Console.WriteLine("windows");
                break;

            case PlatformID.MacOSX:
                Console.WriteLine("osx");
                break;

            case PlatformID.Unix:
                Console.WriteLine("linux");
                break;

            default:
                Console.WriteLine("other");
                break;
        }
    }
}
