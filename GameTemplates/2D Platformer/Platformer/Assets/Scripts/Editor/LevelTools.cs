using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class LevelTools
{
    public static void CreateNewLevelMap()
    {

        EditorWindow window = EditorWindow.GetWindow(typeof(NewLevelMap));
        window.Show();
    }

    public static void CreateNewLevel()
    {
        EditorWindow window = EditorWindow.GetWindow(typeof(NewLevel));
        window.Show();
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
            CameraSettings camSettings = newScene.GetRootGameObjects()[0].AddComponent<CameraSettings>();
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

public class NewLevelMap : EditorWindow
{
    const string LevelMapsFolder = @"Assets/Scenes/LevelMaps/";
    string levelMapName = "";
    private void OnGUI()
    {
        GUILayout.Label("LevelMap Settings", EditorStyles.boldLabel);
        levelMapName = EditorGUILayout.TextField("LevelMap Name", levelMapName);
        if (GUILayout.Button("Create LevelMap"))
        {
            levelMapName = EditorGUILayout.TextField("LevelMap Name: ", levelMapName);
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            newScene.name = levelMapName;
            CameraSettings camSettings = newScene.GetRootGameObjects()[0].AddComponent<CameraSettings>();
            string outputPath = LevelMapsFolder + levelMapName + ".unity";
            if (!Directory.Exists(LevelMapsFolder))
            {
                Directory.CreateDirectory(LevelMapsFolder);
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
