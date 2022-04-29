using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;

[ExecuteInEditMode]
public class ScriptedObject : MonoBehaviour
{
    public enum ObjectType { BLOCK, FLOOR, WALL, BACKGROUND, CEILING }
    [SerializeField]
    public ObjectType type = ObjectType.BLOCK;

    [SerializeField]
    public GameObject blocksHolder;

    [SerializeField]
    public GameObject block;

    [SerializeField]
    public int layer;

    [SerializeField]
    public List<GameObject> tiles = new List<GameObject>();

    [HideInInspector]
    public float xLength;
    [HideInInspector]
    public float yLength;
    [HideInInspector]
    public Vector3 currPos;
    [HideInInspector]
    public Vector3 currScale;

    [HideInInspector]
    public bool canAdjustHorizontal = true;
    [HideInInspector]
    public bool canAdjustVertical = true;

    private float xOffset = 0;
    private float yOffset = 0;

    public virtual void Awake()
    {
        currPos = transform.position;
        currScale = transform.localScale;
    }

    public virtual void Start()
    {
        try
        {
            UpdateTiles();
            currPos = transform.position;
            currScale = transform.localScale;
        }
        catch(Exception e)
        {

        }
    }

    public virtual void Update()
    {
        xLength = Mathf.RoundToInt(transform.localScale.x);
        yLength = Mathf.RoundToInt(transform.localScale.y);

        if (Tools.current == Tool.Move)
        {
            UseMoveTool();
        }
        if (Tools.current == Tool.Rect)
        {
            UseRectTool();
        }
    }

    public virtual void UseMoveTool() 
    {
        transform.position = RoundToGrid(transform.position);
        currPos = transform.position;
    }

    public virtual void UseRectTool() { }
    public virtual void UpdateTiles() 
    {
        try
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        catch (Exception e)
        { 
        }
    }

    public virtual void UpdateLayer()
    {
        foreach (GameObject block in tiles)
        {
            if (block != null && tiles.Contains(block))
            {
                try
                {
                    block.GetComponentInChildren<SpriteRenderer>().sortingOrder = layer;
                }
                catch (Exception e)
                {

                }
            }
        }

        
    }

    public virtual void UpdateFloor()
    {

    }

    public virtual void UpdateWall()
    {

    }

    public virtual Vector3 RoundToGrid(Vector3 position)
    {
        if (type == ObjectType.FLOOR)
        {
            xOffset = (-.5f / xLength) * (xLength - 1);
            yOffset = 0;
        }
        else if (type == ObjectType.WALL)
        {
            xOffset = 0;
            yOffset = (-.5f / yLength) * (yLength - 1) + .5f / yLength;
        }
        float xPos = Mathf.RoundToInt(transform.position.x) + xOffset;
        float yPos = Mathf.RoundToInt(transform.position.y) + yOffset;
        return new Vector3(xPos, yPos, 0);
    }

    public virtual void RemoveExtraObjects()
    {
        Transform[] objs = blocksHolder.GetComponentsInChildren<Transform>();
        objs[0] = null;
        foreach (Transform obj in objs)
        {
            if (obj != null && obj.tag == "Block" && !tiles.Contains(obj.gameObject))
            {
                DestroyImmediate(obj.gameObject);
            }
        }
    }

    public void RefreshObject()
    {
        UpdateTiles();
        currPos = transform.position;
        currScale = transform.localScale;
    }
}
