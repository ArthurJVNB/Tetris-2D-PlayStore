using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameplayManager))]
public class GameplayManagerInspectorLayout : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GameplayManager gameplay = target as GameplayManager;
        
        if (GUILayout.Button("Create Grid"))
        {
            gameplay.CreateGrid();
        }

        if (GUILayout.Button("Spawn Tetromino"))
        {
            gameplay.SpawnTetromino();
        }
    }
}
