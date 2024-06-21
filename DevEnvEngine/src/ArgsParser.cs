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
                ParseAddRepoOpts(args, result);
                break;

            case "list_repos":
                break;

            default:
                throw new ArgumentException(
                    $"Got an unexpected command '{command}' :("
                );
        }

        result.Add("command", command);
        return result;
    }

    private static void ParseAddRepoOpts(string[] args,
                                         Dictionary<string, string> result)
    {
        int it = 1;
        string name = string.Empty;
        string path = string.Empty;

        // FIXME: Add special handling for when we receive flags without their
        //        respective expected values.

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
}
