using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class ScriptedTools
{
    public static void RemoveExtraObjects()
    {
        ScriptedObject[] objects = GameObject.FindObjectsOfType<ScriptedObject>();
        foreach (ScriptedObject obj in objects)
        {
            obj.RemoveExtraObjects();
        }
    }

    public static void AddScriptedFloor()
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "ScriptedFloor";

        ScriptedFloor scriptedFloor = floor.AddComponent<ScriptedFloor>();
        GameObject blocks = new GameObject("Blocks");
        scriptedFloor.blocksHolder = blocks;
        blocks.transform.parent = floor.transform;
        floor.tag = "Floor";
        scriptedFloor.type = ScriptedObject.ObjectType.FLOOR;

        PrefabUtility.InstantiatePrefab(floor);
    }

    public static void AddScriptedWall()
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "ScriptedWall";

        ScriptedWall scriptedWall = wall.AddComponent<ScriptedWall>();
        GameObject blocks = new GameObject("Blocks");
        scriptedWall.blocksHolder = blocks;
        blocks.transform.parent = wall.transform;
        wall.tag = "Wall";
        scriptedWall.type = ScriptedObject.ObjectType.WALL;

        PrefabUtility.InstantiatePrefab(wall);
    }

    public static void AddScriptedRect()
    {
        GameObject rect = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rect.name = "ScriptedRect";

        ScriptedRect scriptedRect = rect.AddComponent<ScriptedRect>();
        GameObject blocks = new GameObject("Blocks");
        scriptedRect.blocksHolder = blocks;
        blocks.transform.parent = rect.transform;

        PrefabUtility.InstantiatePrefab(rect);
    }
}
