using System;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class BuildTools
{
    [MenuItem("MacroBunny/Create/New LevelMap")]
    public static void CreateNewLevelMap()
    {
        LevelTools.CreateNewLevelMap();
    }

    [MenuItem("MacroBunny/Create/New Level")]
    public static void CreateNewLevel()
    {
        LevelTools.CreateNewLevel();
    }
    [MenuItem("MacroBunny/Scripted Objects/Remove extra objects")]
    public static void RefreshScriptedObjects()
    {
        ScriptedTools.RemoveExtraObjects();
    }

    [MenuItem("MacroBunny/Scripted Objects/Floor")]
    public static void AddScriptedFloor()
    {
        ScriptedTools.AddScriptedFloor();
    }

    [MenuItem("MacroBunny/Scripted Objects/Wall")]
    public static void AddScriptedWall()
    {
        ScriptedTools.AddScriptedWall();
    }

    [MenuItem("MacroBunny/Scripted Objects/Rect")]
    public static void AddScriptedRect()
    {
        ScriptedTools.AddScriptedRectParent();
    }

    [MenuItem("MacroBunny/Add/Player")]
    public static void AddPlayer()
    {
        PlayerTools.AddPlayer();
    }

    [MenuItem("MacroBunny/Add/Camera Reposition Trigger")]
    public static void AddCameraRepositionTrigger()
    {
        CameraTools.AddCameraRepositionTrigger();
    }
}