using System.Diagnostics;
using System.IO;
using UnityEngine;

public static class Git
{
    public static string RepositoryPath =>
        Directory.GetParent(Application.dataPath).FullName;

    public static string Run(string arguments)
    {
        var process = new Process();

        process.StartInfo = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = arguments,
            WorkingDirectory = RepositoryPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        return string.IsNullOrWhiteSpace(error)
            ? output
            : output + "\n" + error;
    }

    public static string Status(bool porcelain = false)
    {
        return Run("status" + (porcelain ? " --porcelain" : ""));
    }

    public static string Pull() => Run("pull");

    public static string Push() => Run("push");

    public static string Fetch() => Run("fetch");

    public static string Branches() => Run("branch -r");

    public static string Log(int count = 20, bool oneline = true, bool graph = false, bool decorate = false) =>
        Run($"log" + (count > 0 ? $" -n {count}" : "") + (oneline ? " --oneline" : "") + (graph ? " --graph" : "") + (decorate ? " --decorate" : ""));

    public static string Checkout(string branch) =>
        Run($"checkout {branch}");

    public static string AddAll() =>
        Run("add .");

    public static string Commit(string message) =>
        Run($"commit -m \"{message}\"");

}