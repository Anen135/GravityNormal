using System.Collections.Generic;

public interface IIssueProvider
{
    List<GitIssue> GetIssues();
}