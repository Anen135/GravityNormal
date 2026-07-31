using System;

[Serializable]
public class GitIssue
{
    public string Id;
    public string Title;

    public string BranchName
    {
        get
        {
            return $"{Id}-{Normalize(Title)}";
        }
    }


    static string Normalize(string text)
    {
        text = text.ToLower();

        text = text
            .Replace(" ", "-")
            .Replace("_", "-");


        foreach(char c in text)
        {
            if (!char.IsLetterOrDigit(c) && c != '-')
            {
                text = text.Replace(c.ToString(), "");
            }
        }


        while(text.Contains("--"))
            text=text.Replace("--","-");


        return text.Trim('-');
    }
}