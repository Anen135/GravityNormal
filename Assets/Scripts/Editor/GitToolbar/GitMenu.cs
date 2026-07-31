using UnityEditor;
using UnityEngine;

public static class GitMenu
{
    #region LFS
    [MenuItem("Git/LFS/Install")]
    static void LfsInstall()
    {
        Debug.Log(GitLFS.Install());
    }


    [MenuItem("Git/LFS/Status")]
    static void LfsStatus()
    {
        Debug.Log(GitLFS.Status());
    }


    [MenuItem("Git/LFS/Pull")]
    static void LfsPull()
    {
        Debug.Log(GitLFS.Pull());
    }


    [MenuItem("Git/LFS/Fetch All")]
    static void LfsFetch()
    {
        Debug.Log(GitLFS.FetchAll());
    }
    #endregion

    #region Flow

    [MenuItem("Git Flow/Init")]
    static void FlowInit()
    {
        Debug.Log(GitFlow.Init());
    }

    [MenuItem("Git Flow/Feature List")]
    static void FeatureList()
    {
        Debug.Log( GitFlow.FeatureList() );
    }

    [MenuItem("Git Flow/Start new feature")]
    static void FeatureStart()
    {
        StringInputPopup.Open( "Feature name", "new-feature", name => { Debug.Log("[Git Flow] " + GitFlow.FeatureStart(name)); });
    }

    [MenuItem("Git Flow/Finish current feature")]
    static void FeatureFinish()
    {
        Debug.Log("[Git Flow] " + GitFlow.FeatureFinish());
    }

    #endregion

    #region Git

    [MenuItem("Git/Status")]
    static void Status()
    {
        Debug.Log(Git.Status(porcelain: true));
    }

    [MenuItem("Git/Pull")]
    static void Pull()
    {
        Debug.Log(Git.Pull());
    }

    [MenuItem("Git/Push")]
    static void Push()
    {
        Debug.Log(Git.Push());
    }

    [MenuItem("Git/Fetch")]
    static void Fetch()
    {
        Debug.Log(Git.Fetch());
    }

    [MenuItem("Git/Log")]
    static void Log()
    {
        Debug.Log(Git.Log());
    }

    #endregion
}