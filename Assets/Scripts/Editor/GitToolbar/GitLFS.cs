using UnityEngine;

public static class GitLFS
{
    public static string Install()
    {
        return Git.Run("lfs install");
    }

    public static string Status()
    {
        return Git.Run("lfs status");
    }

    public static string Track(string pattern)
    {
        return Git.Run($"lfs track \"{pattern}\"");
    }

    public static string Untrack(string pattern)
    {
        return Git.Run($"lfs untrack \"{pattern}\"");
    }

    public static string Pull()
    {
        return Git.Run("lfs pull");
    }

    public static string Fetch()
    {
        return Git.Run("lfs fetch");
    }

    public static string FetchAll()
    {
        return Git.Run("lfs fetch --all");
    }

    public static string Checkout()
    {
        return Git.Run("lfs checkout");
    }

    public static string Locks()
    {
        return Git.Run("lfs locks");
    }

    public static string Lock(string file)
    {
        return Git.Run($"lfs lock \"{file}\"");
    }

    public static string Unlock(string file)
    {
        return Git.Run($"lfs unlock \"{file}\"");
    }

    public static string Migrate(string pattern)
    {
        return Git.Run(
            $"lfs migrate import --include=\"{pattern}\""
        );
    }
}