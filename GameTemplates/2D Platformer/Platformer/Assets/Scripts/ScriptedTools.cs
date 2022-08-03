using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class ScriptedTools
{
#if UNITY_EDITOR
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
        floor.transform.parent = GameObject.FindGameObjectWithTag("Environment").transform;

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
        wall.transform.parent = GameObject.FindGameObjectWithTag("Environment").transform;

        ScriptedWall scriptedWall = wall.AddComponent<ScriptedWall>();
        GameObject blocks = new GameObject("Blocks");
        scriptedWall.blocksHolder = blocks;
        blocks.transform.parent = wall.transform;
        wall.tag = "Wall";
        scriptedWall.type = ScriptedObject.ObjectType.WALL;

        PrefabUtility.InstantiatePrefab(wall);
    }

    public static void AddScriptedRectParent()
    {
        GameObject rectParent = new GameObject("ScriptedRectParent");
        rectParent.transform.parent = GameObject.FindGameObjectWithTag("Environment").transform;
        rectParent.AddComponent<ScriptedRectParent>();
        PrefabUtility.InstantiatePrefab(rectParent);
        AddScriptedRect(rectParent);
    }

    public static void AddScriptedRect(GameObject parent)
    {
        AddScriptedRect(parent, null, null, -1, -1);
    }

    public static Object AddScriptedRect(GameObject parent, GameObject block, GameObject prevRect, float xLength, int rectHeight)
    {
        GameObject rect = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rect.name = "ScriptedRect";
        rect.transform.parent = parent.transform;

        ScriptedRect scriptedRect = rect.AddComponent<ScriptedRect>();
        GameObject blocks = new GameObject("Blocks");
        scriptedRect.blocksHolder = blocks;
        blocks.transform.parent = rect.transform;
        rect.tag = "Rect";
        scriptedRect.type = ScriptedObject.ObjectType.RECT;

        if (xLength > -1 && block != null)
        {
            scriptedRect.block = block;
            scriptedRect.xLength = xLength;
            rect.transform.localScale = prevRect.transform.localScale;
            rect.transform.localPosition = prevRect.transform.localPosition - (Vector3.up * (rectHeight - 1));
        }

        Object newRect = PrefabUtility.InstantiatePrefab(rect);
        scriptedRect.UseRectTool();
        return newRect;
    }
#endif
}
