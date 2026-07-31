using System.Collections.Generic;
using UnityEngine;

public class JiraIssueProvider : IIssueProvider
{
    public List<GitIssue> GetIssues()
    {
        // REST API Jira
        // GET /rest/api/3/search
        // In progress
        return new List<GitIssue>();
    }
}