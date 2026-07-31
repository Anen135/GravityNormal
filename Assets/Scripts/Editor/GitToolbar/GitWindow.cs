using UnityEditor;
using UnityEngine;

public class GitWindow : EditorWindow
{
    string commitMessage = "";
    Vector2 scroll;
    string output = "";

    [MenuItem("Git/Window")]
    static void Open()
    {
        GetWindow<GitWindow>("Git");
    }

    void OnGUI()
    {
        GUILayout.Label("Repository", EditorStyles.boldLabel);

        if (GUILayout.Button("Status"))
            output = Git.Status();

        if (GUILayout.Button("Fetch"))
            output = Git.Fetch();

        if (GUILayout.Button("Pull"))
            output = Git.Pull();

        if (GUILayout.Button("Push"))
            output = Git.Push();

        GUILayout.Space(10);

        GUILayout.Label("Commit");

        commitMessage = EditorGUILayout.TextField(commitMessage);

        if (GUILayout.Button("Commit"))
        {
            Git.AddAll();
            output = Git.Commit(commitMessage);
            commitMessage = "";
        }

        GUILayout.Space(10);

        scroll = GUILayout.BeginScrollView(scroll);

        GUILayout.TextArea(output, GUILayout.ExpandHeight(true));

        GUILayout.EndScrollView();
    }
}