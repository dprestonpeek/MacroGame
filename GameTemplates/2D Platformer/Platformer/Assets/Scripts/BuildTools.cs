using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
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

    [MenuItem("MacroBunny/Add/Player")]
    public static void AddPlayer()
    {
        GameObject playerParent = new GameObject("PlayerParent");
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Cube);
        player.name = "Player";
        player.transform.localScale = new Vector3(1, 2, 1);
        player.transform.parent = playerParent.transform;

        PlayerMovement movement = player.AddComponent<PlayerMovement>();
        movement.rb = player.AddComponent<Rigidbody>();
        movement.rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        PlayerAnimation anim = player.AddComponent<PlayerAnimation>();
        InputBridge bridge = player.AddComponent<InputBridge>();
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
            newScene.GetRootGameObjects()[0].AddComponent<CameraSettings>();
            string outputPath = LevelsFolder + levelName + ".unity";
            if (!Directory.Exists(LevelsFolder))
            {
                Directory.CreateDirectory(LevelsFolder);
            }

            foreach (GameObject go in InitDefaultObjects())
            {
                PrefabUtility.InstantiatePrefab(go);
            }
            EditorSceneManager.SaveScene(newScene, outputPath);
            EditorSceneManager.OpenScene(outputPath);
            Close();
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

        defaultObjs[1] = lightObj;          //directional light

        return defaultObjs;
    }
}
