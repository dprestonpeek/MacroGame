using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ScriptedObject : MonoBehaviour
{
    //public enum ObjectType { BLOCK, FLOOR, WALL, BACKGROUND, CEILING }
    //[SerializeField]
    //ObjectType type = ObjectType.BLOCK;

    [SerializeField]
    public GameObject blocksHolder;

    [SerializeField]
    public GameObject block;

    [SerializeField]
    int layer;

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

    private void Awake()
    {
        currPos = transform.position;
        currScale = transform.localScale;
    }

    private void Update()
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

        UpdateTiles();

        if (layer != transform.position.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, layer);
        }    
    }

    public virtual void UseMoveTool() 
    {
        transform.position = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        currPos = transform.position;
    }

    public virtual void UseRectTool() { }
    public virtual void UpdateTiles() 
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }
}
