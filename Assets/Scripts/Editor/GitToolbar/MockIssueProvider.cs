using System.Collections.Generic;


public class MockIssueProvider : IIssueProvider
{
    public List<GitIssue> GetIssues()
    {
        return new()
        {
            new GitIssue
            {
                Id="GAME-101",
                Title="Fix character controller"
            },

            new GitIssue
            {
                Id="GAME-102",
                Title="Add weapon system"
            },

            new GitIssue
            {
                Id="GAME-103",
                Title="Optimize texture loading"
            }
        };
    }
}