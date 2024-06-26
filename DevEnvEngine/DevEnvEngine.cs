using System;
using System.Collections.Generic;
using System.IO;

namespace DevEnv;

internal static class DevEnvEngine
{
    internal const string DEV_REPOS_ENV_VAR = "DEV_REPOS";
    internal const string WORK_REPO_ENV_VAR = "WORK_REPO";

    internal const string DEV_ARCH_ENV_VAR = "DEV_ARCH";
    internal const string DEV_OS_ENV_VAR = "DEV_OS";
    internal const string DEV_CONFIGURATION_ENV_VAR = "DEV_CONFIGURATION";

    internal static readonly string BUILD_SCRIPT_NAME =
        Environment.OSVersion.Platform == PlatformID.Win32NT
        ? "build.cmd" : "build.sh";

    internal static readonly string MAIN_SCRIPT_FLAG_PREFIX =
        Environment.OSVersion.Platform == PlatformID.Win32NT
        ? "-" : "--";

    internal static readonly string TEST_SCRIPT_FLAG_PREFIX =
        Environment.OSVersion.Platform == PlatformID.Win32NT
        ? string.Empty : "-";

    /// <summary>
    /// Parses a tuple containing an identifier that will be used as key, and a
    /// path to a local repository. Then, it prints it in a 'key,path' format,
    /// which the shell reads and appends it to the DEV_REPOS environment variable.
    /// </summary>
    /// <remarks>
    /// If more than two parameters are received, then the third one and on are
    /// just ignored. If less than two, then an error is returned.
    /// </remarks>
    /// <returns>0 if all good, otherwise -1.</returns>

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

        if (DevEnvUtils.IsRepoRegistered(key))
        {
            Console.WriteLine("There is already a repo registered with the given"
                              + $" key '{key}'.");
            return -1;
        }

        Console.WriteLine($"{key},{path}");
        return 0;
    }

    /// <summary>
    /// Reads and parses the contents of the DEV_REPOS environment variable, and
    /// prints them in a friendly-readable way. Will use a table in the future.
    /// </summary>
    /// <remarks>
    /// Displays a brief message indicating no repos have been registered if said
    /// case occurs.
    /// </remarks>

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
        // 'key,path' format, and a semi-colon ';' is used as the entries separator.

        foreach (string r in repos.Split(';'))
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

    /// <summary>
    /// Looks up the received key in the DEV_REPOS environment variable, and then
    /// prints the path mapped to said key. The shell then sets this path to the
    /// WORK_REPO environment variable.
    /// </summary>
    /// <remarks>
    /// If more than two parameters are received, then the third one and on are
    /// just ignored. If less than two, then an error is returned.
    /// </remarks>
    /// <returns>0 if all good, -1 otherwise.</returns>

    public static int FnSetRepo(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("A string representing the key mapping the repo in"
                              + " DEV_REPOS, is required to set it.");
            return -1;
        }

        string repoPath = DevEnvUtils.GetRepoFromKey(args[0]);

        if (string.IsNullOrEmpty(repoPath))
        {
            Console.WriteLine($"The received key '{args[0]}' was not found. Make"
                              + " sure to first register it with addrepo.");
            return -1;
        }

        Console.WriteLine(repoPath);
        return 0;
    }

    /// <summary>
    /// Looks up the test(s) that match the provided wildcards in 'src/tests/*'.
    /// It will return any file that matches, as we can't know beforehand whether
    /// the user wants the cs, csproj, or what specific kind of file. Working on
    /// designing a solution for that currently.
    /// </summary>
    /// <remarks>
    /// If more than one wildcard is provided, then the results of each one will
    /// be displayed in their own groups for cleaner and easier readability. This
    /// function's search is also case insensitive, and unreadable files for whatever
    /// reason (permissions being the most common), are just ignored.
    /// </remarks>

    public static void FnFindTest(string[] args)
    {
        if (args.Length <= 0)
        {
            Console.WriteLine("A test name or wildcard is needed to search for it.");
            return ;
        }

        string repo = Environment.GetEnvironmentVariable(WORK_REPO_ENV_VAR);

        if (string.IsNullOrEmpty(repo))
        {
            Console.WriteLine("No currently active repo.");
            return ;
        }

        string testsPath = Path.Join(repo, "src", "tests");

        foreach (string testPattern in args)
        {
            IEnumerable<string> results = Directory.EnumerateFiles(
                path: testsPath,
                searchPattern: testPattern,
                enumerationOptions: new EnumerationOptions
                {
                    IgnoreInaccessible = true,
                    MatchCasing = MatchCasing.CaseInsensitive,
                    RecurseSubdirectories = true
                });

            Console.WriteLine($"\nResults matching '{testPattern}':\n");
            Console.WriteLine(string.Join("\n", results));
        }
    }

    /// <summary>
    /// Docs go here.
    /// </summary>

    public static int FnBuildSubset(string[] args)
    {
        if (args.Length <= 0 || (args.Length == 1 && args[0].StartsWith("--repo=")))
        {
            Console.WriteLine("A subset name or expression is needed for running"
                              + " the build.");
            return -1;
        }

        int subsetIndex = 0;
        List<string> argsList = new List<string>();

        if (args[0].StartsWith("--repo="))
        {
            argsList.Add(args[0]);
            subsetIndex = 1;
        }

        string arch = Environment.GetEnvironmentVariable(DEV_ARCH_ENV_VAR);
        string os = Environment.GetEnvironmentVariable(DEV_OS_ENV_VAR);
        string config = Environment.GetEnvironmentVariable(DEV_CONFIGURATION_ENV_VAR);

        argsList.Add($"{MAIN_SCRIPT_FLAG_PREFIX}subset {args[subsetIndex]}");
        argsList.Add($"{MAIN_SCRIPT_FLAG_PREFIX}configuration {config}");
        argsList.Add($"{MAIN_SCRIPT_FLAG_PREFIX}arch {arch}");
        argsList.Add($"{MAIN_SCRIPT_FLAG_PREFIX}os {os}");

        return FnRepoMainScript(argsList.ToArray());
    }

    /// <summary>
    /// Docs go here.
    /// </summary>

    public static int FnGenerateLayout(string[] args)
    {
        List<string> argsList = new List<string>();

        if (args.Length >= 1 && args[0].StartsWith("--repo="))
        {
            argsList.Add(args[0]);
        }

        string arch = Environment.GetEnvironmentVariable(DEV_ARCH_ENV_VAR);
        string os = Environment.GetEnvironmentVariable(DEV_OS_ENV_VAR);
        string config = Environment.GetEnvironmentVariable(DEV_CONFIGURATION_ENV_VAR);

        argsList.Add($"{TEST_SCRIPT_FLAG_PREFIX}{arch}");
        argsList.Add($"{TEST_SCRIPT_FLAG_PREFIX}{config}");
        argsList.Add($"{TEST_SCRIPT_FLAG_PREFIX}generatelayoutonly");

        if (!(os == "windows"))
            argsList.Add($"{TEST_SCRIPT_FLAG_PREFIX}os {os}");

        return FnTestsBuildScript(argsList.ToArray());
    }

    /// <summary>
    /// Forms the call to the build script located at the root of the runtime repo
    /// clone specified by the environment variable 'WORK_REPO', or the --repo= flag.
    /// Then, it prints the constructed command for the shell to execute.
    /// </summary>
    /// <returns>0 if all good, -1 otherwise.</returns>

    public static int FnRepoMainScript(string[] args)
    {
        return RunRuntimeRepoScripts("main", args);
    }

    /// <summary>
    /// Forms the call to the build script located in the path 'src/tests' of the
    /// runtime repo clone specified by the environment variable 'WORK_REPO', or
    /// the --repo= flag.
    /// Then, it prints the constructed command for the shell to execute.
    /// </summary>
    /// <returns>0 if all good, -1 otherwise.</returns>

    public static int FnTestsBuildScript(string[] args)
    {
        return RunRuntimeRepoScripts("clrtests", args);
    }

    /// <summary>
    /// Helper function to do the common lifting of FnRepoMainScript()
    /// and FnTestsBuildScript().
    /// </summary>

    private static int RunRuntimeRepoScripts(string component, string[] args)
    {
        string workRepo = string.Empty;
        int scriptArgsStart = 0;

        if (args.Length > 0 && args[0].StartsWith("--repo="))
        {
            workRepo = Path.GetFullPath(args[0].Split('=')[1]);
            scriptArgsStart = 1;
        }
        else
        {
            workRepo = Environment.GetEnvironmentVariable(WORK_REPO_ENV_VAR);
        }

        if (string.IsNullOrWhiteSpace(workRepo))
        {
            Console.WriteLine("There is no specified repo to build. Make sure to"
                              + " either pass it with the '--repo=' flag, or set"
                              + " the WORK_REPO env var with 'setrepo'.");
            return -1;
        }

        string scriptPath = string.Empty;
        string buildArgs = string.Join(' ', args[scriptArgsStart..]);

        if (!string.IsNullOrWhiteSpace(buildArgs))
            buildArgs = $" {buildArgs}";

        switch (component)
        {
            case "main":
                scriptPath = Path.Join(workRepo, BUILD_SCRIPT_NAME);
                break;

            case "clrtests":
                scriptPath = Path.Join(workRepo, "src", "tests", BUILD_SCRIPT_NAME);
                break;

            default:
                Console.WriteLine("RunRuntimeRepoScripts(): Received unrecognized"
                                  + $" component '{component}'.");
                return -1;
        }

        Console.WriteLine($"{scriptPath}{buildArgs}");
        return 0;
    }
}
