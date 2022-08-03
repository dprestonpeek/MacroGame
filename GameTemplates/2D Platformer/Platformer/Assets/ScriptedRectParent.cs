using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class ScriptedRectParent : MonoBehaviour
{
    [HideInInspector]
    public int xLength = 0;
    public int RectHeight = 1;
    private int rectHeight = 1;
    private GameObject[] children = new GameObject[0];
    
    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        rectHeight = GetChildren().Length;

        if (RectHeight > rectHeight)
        {
            ScriptedRect floor = GetComponentInChildren<ScriptedRect>();
            if (ScriptedTools.AddScriptedRect(gameObject, floor.block, floor.gameObject, floor.xLength, ++rectHeight) != null)
            {
                rectHeight++;
            }
        }
        while (rectHeight > 1 && RectHeight < rectHeight)
        {
            for (int i = rectHeight - 1; i > RectHeight - 1; i--)
            {
                DestroyImmediate(GetChildren()[i]);
            }
        }
    }

    public GameObject[] GetChildren()
    {
        ScriptedRect[] rects = GetComponentsInChildren<ScriptedRect>();
        children = new GameObject[rects.Length];
        for (int i = 0; i < rects.Length; i++)
        {
            children[i] = rects[i].gameObject;
        }
        return children;
    }
}
