using System;
using System.Collections.Generic;

namespace DevEnvEngine;

internal static class ArgsParser
{
    public static Dictionary<string, string> Run(string[] args)
    {
        var result = new Dictionary<string, string>();
        string command = args[0];

        switch (command)
        {
            case "add_repo":
            case "set_repo":
                ParseAddSetRepoOpts(args, result);
                break;

            case "list_repos":
                break;

            case "build_runtime":
                ParseBuildRuntimeOpts(args, result);
                break;

            default:
                throw new ArgumentException(
                    $"Got an unexpected command '{command}' :("
                );
        }

        result.Add("command", command);
        return result;
    }

    // FIXME: Add special handling for when we receive flags without their
    //        respective expected values.

    private static void ParseAddSetRepoOpts(string[] args,
                                            Dictionary<string, string> result)
    {
        int it = 1;
        string name = string.Empty;
        string path = string.Empty;

        while (true)
        {
            string nextArg = args[it];

            switch (nextArg)
            {
                case "-n": case "--name":
                    name = args[it+1];
                    break;
                case "-p": case "--path":
                    path = args[it+1];
                    break;
                default:
                    throw new ArgumentException(
                        $"AddRepo: Got an unrecognized option '{nextArg}' :("
                    );
            }

            it += 2;
            if (it >= args.Length) break;
        }

        result.Add("name", name);
        result.Add("path", path);
    }

    private static void ParseBuildRuntimeOpts(string[] args,
                                              Dictionary<string, string> result)
    {
        string repo = string.Empty;

        if (args[1] != "-r" && args[1] != "--repo")
        {
            throw new ArgumentException(
                $"BuildRuntime: Got an unrecognized option '{args[1]}' :("
            );
        }

        result.Add("repo", args[2]);
        result.Add("args", string.Join(' ', args[3..]));
    }
}
