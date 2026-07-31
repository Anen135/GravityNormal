using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public class GitHubIssueProvider : IIssueProvider
{
    public List<GitIssue> GetIssues()
    {
        string json = RunGh(
            "issue list --state open --json number,title"
        );

        return Parse(json);
    }



    string RunGh(string args)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "gh",
                Arguments = args,

                RedirectStandardOutput = true,
                RedirectStandardError = true,

                UseShellExecute = false,
                CreateNoWindow = true,

                StandardOutputEncoding = System.Text.Encoding.UTF8,
                StandardErrorEncoding = System.Text.Encoding.UTF8
            }
        };


        process.Start();


        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if(process.ExitCode != 0)
        {
            UnityEngine.Debug.LogError(error);
            return "[]";
        }


        return output;
    }



    List<GitIssue> Parse(string json)
    {
        var result = new List<GitIssue>();
        var wrapper = JsonUtility.FromJson<IssueArray>( "{ \"items\":" + json + "}");


        foreach(var item in wrapper.items)
        {
            result.Add(
                new GitIssue
                {
                    Id = $"{item.number}",
                    Title = item.title
                }
            );
        }


        return result;
    }



    [Serializable]
    class IssueArray
    {
        public Issue[] items;
    }



    [Serializable]
    class Issue
    {
        public int number;
        public string title;
    }
}