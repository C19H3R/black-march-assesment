using UnityEditor;
using UnityEngine;
public class ObstacleEditor : EditorWindow
{
    

    public bool[] grid= new bool[100];
    public ObstaclePosSO obsPosData;

    [MenuItem("Assignment Tools/Assignment 2/Object Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ObstacleEditor));
    }

    private void OnGUI()
    {

        GUILayout.Label("Object Position Editor");
        GUILayout.Label("");
        GUILayout.Label("10x10 Grid");

        obsPosData = (ObstaclePosSO)EditorGUILayout.ObjectField(obsPosData, typeof(ObstaclePosSO), false) ;

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                grid[i * 10 + j] = EditorGUI.Toggle(new Rect(10 + j * 20, 100 + i * 20, 10, 10), grid[i * 10 + j]);
            }
        }
        if (GUI.Button(new Rect(20, 300, 150, 25), "update Scriptable Object"))
        {
            obsPosData.obstaclePos = grid;
        }
    }

}

