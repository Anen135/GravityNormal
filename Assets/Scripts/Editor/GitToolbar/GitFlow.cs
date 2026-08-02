public static class GitFlow
{
    public static string Init()
    {
        return Git.Run("flow init");
    }

    // Feature

    public static string FeatureStart(string name)
    {
        name = name.Replace(" ", "-");
        return Git.Run( $"flow feature start {name}" );
    }


    public static string FeatureFinish()
    {
        return Git.Run(
            "flow feature finish"
        );
    }


    public static string FeaturePublish()
    {
        return Git.Run(
            "flow feature publish"
        );
    }


    public static string FeatureList()
    {
        return Git.Run(
            "flow feature list"
        );
    }



    // Release

    public static string ReleaseStart(string version)
    {
        return Git.Run(
            $"flow release start {version}"
        );
    }


    public static string ReleaseFinish()
    {
        return Git.Run(
            "flow release finish"
        );
    }



    // Hotfix

    public static string HotfixStart(string version)
    {
        return Git.Run(
            $"flow hotfix start {version}"
        );
    }


    public static string HotfixFinish()
    {
        return Git.Run(
            "flow hotfix finish"
        );
    }


    public static string Publish(string branch)
    {
        return Git.Run(
            $"flow {branch} publish"
        );
    }


    public static string DeleteRemote(string branch)
    {
        return Git.Run(
            $"flow {branch} delete"
        );
    }
}