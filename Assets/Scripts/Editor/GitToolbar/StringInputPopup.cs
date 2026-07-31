using UnityEditor;
using UnityEngine;
using System;


public class StringInputPopup : EditorWindow
{
    string inputValue;
    string promt;
    Action<string> callback;


    public static void Open(
        string porno,
        string defaultValue,
        Action<string> callback)
    {
        var window =
            CreateInstance<StringInputPopup>();

        window.promt = porno;
        window.inputValue = defaultValue;
        window.callback = callback;


        window.titleContent =
            new GUIContent(porno);


        window.position =
            new Rect(
                Screen.width / 2,
                Screen.height / 2,
                350,
                110
            );


        window.ShowUtility();


        EditorApplication.delayCall += () =>
        {
            window.Focus();
            GUI.FocusControl("Input");
        };
    }



    void OnGUI()
    {
        GUILayout.Label(
            promt,
            EditorStyles.boldLabel
        );


        GUI.SetNextControlName("Input");

        inputValue =
            EditorGUILayout.TextField(
                inputValue
            );


        GUILayout.Space(10);


        GUILayout.BeginHorizontal();


        if(GUILayout.Button("OK"))
        {
            Accept();
        }


        if(GUILayout.Button("Cancel"))
        {
            Close();
        }


        GUILayout.EndHorizontal();



        if(Event.current.type == EventType.KeyDown)
        {
            if(Event.current.keyCode == KeyCode.Return)
            {
                Accept();
            }


            if(Event.current.keyCode == KeyCode.Escape)
            {
                Close();
            }
        }
    }



    void Accept()
    {
        string result =
            inputValue?.Trim();


        if(!string.IsNullOrEmpty(result))
        {
            callback?.Invoke(result);
        }


        Close();
    }
}