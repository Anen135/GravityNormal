using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


public class GitFlowFeatureWindow : EditorWindow
{

    List<GitIssue> issues;

    int selected;


    [MenuItem("Git Flow/Create Feature From Issue")]
    static void Open()
    {
        GetWindow<GitFlowFeatureWindow>(
            "Feature"
        );
    }



    void OnEnable()
    {
        IIssueProvider provider =
            new GitHubIssueProvider();

        issues = provider.GetIssues();
    }



    void OnGUI()
    {

        GUILayout.Label(
            "Select Issue",
            EditorStyles.boldLabel
        );


        string[] names = new string[issues.Count];


        for(int i=0;i<issues.Count;i++)
        {
            names[i] =
                $"{issues[i].Id}: {issues[i].Title}";
        }


        selected =
            EditorGUILayout.Popup(
                selected,
                names
            );



        GUILayout.Space(10);


        var issue = issues[selected];


        EditorGUILayout.HelpBox(
            "Branch:\nfeature/" 
            + issue.BranchName,
            MessageType.Info
        );



        if(GUILayout.Button(
            "Create Feature Branch"))
        {
            string result =
                GitFlow.FeatureStart(issue.BranchName);


            Debug.Log(result);

            Close();
        }

    }
}