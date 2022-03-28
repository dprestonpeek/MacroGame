using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildTools
{
    private EditorWindow TextBoxInput;

    [MenuItem("MacroBunny/Create/New Level", false, 1)]
    public static void CreateNewLevel()
    {
        EditorWindow window = EditorWindow.GetWindow(typeof(NewLevel));
        window.Show();
    }

    [MenuItem("MacroBunny/Add/Scripted Object/Floor")]
    public static void AddScriptedFloor()
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "ScriptedFloor";

        ScriptedFloor scriptedFloor = floor.AddComponent<ScriptedFloor>();
        GameObject blocks = new GameObject("Blocks");
        scriptedFloor.blocksHolder = blocks;
        blocks.transform.parent = floor.transform;

        PrefabUtility.InstantiatePrefab(floor);
    }
}

public class NewLevel : EditorWindow
{
    const string LevelsFolder = @"Assets/Scenes/Levels/";
    string levelName = "";
    private void OnGUI()
    {
        GUILayout.Label("Level Settings", EditorStyles.boldLabel);
        levelName = EditorGUILayout.TextField("Level Name", levelName);
        if (GUILayout.Button("Create Level"))
        {
            levelName = EditorGUILayout.TextField("Level Name: ", levelName);
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            newScene.name = levelName;
            string outputPath = LevelsFolder + levelName + ".unity";
            if (!Directory.Exists(LevelsFolder))
            {
                Directory.CreateDirectory(LevelsFolder);
            }
            bool successful = EditorSceneManager.SaveScene(newScene, outputPath);
            EditorSceneManager.OpenScene(outputPath);

            foreach (GameObject go in InitDefaultObjects())
            {
                PrefabUtility.InstantiatePrefab(go);
            }

            if (successful)
            {
                Close();
            }
        }
    }

    private GameObject[] InitDefaultObjects()
    {
        GameObject[] defaultObjs = new GameObject[2];

        GameObject env = new GameObject()
        {
            name = "Environment"
        };
        defaultObjs[0] = env;               //empty environment obj

        GameObject lightObj = new GameObject("Directional Light");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = Color.white;

        defaultObjs[1] = lightObj;

        return defaultObjs;
    }
}
