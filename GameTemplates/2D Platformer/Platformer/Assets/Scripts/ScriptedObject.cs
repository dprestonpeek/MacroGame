using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ScriptedObject : MonoBehaviour
{
    [SerializeField]
    public enum ObjectType { BLOCK, FLOOR, WALL, BACKGROUND, CEILING }
    ObjectType type = ObjectType.BLOCK;

    [SerializeField]
    public GameObject blocksHolder;

    [SerializeField]
    GameObject block;

    [SerializeField]
    List<GameObject> tiles;

    private float xLength;
    private Vector3 currPos;
    private Vector3 currScale;

    private void Awake()
    {
        currPos = transform.position;
        currScale = transform.localScale;
        xLength = Mathf.RoundToInt(transform.localScale.x);
        tiles = new List<GameObject>();
    }

    private void Update()
    {
        xLength = Mathf.RoundToInt(transform.localScale.x);
        if (Tools.current == Tool.Move)
        {
            transform.position = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
            currPos = transform.position;
        }
        if (Tools.current == Tool.Rect)
        {
            transform.localScale = new Vector2(xLength, transform.localScale.y);
            transform.position = currPos;
            //currScale = transform.localScale;
        }

        if (xLength != tiles.Count)
        {
            if (xLength > 0)
            {
                UpdateTiles();
            }
        }

            
    }

    private void UpdateTiles()
    {
        if (tiles.Count > xLength)
        {
            for (int i = (int)xLength - 1; i < tiles.Count; i++)
            {
                DestroyImmediate(tiles[i].gameObject);
                tiles.RemoveAt(i);
            }
        }
        else if (tiles.Count < xLength)
        {
            for (int i = tiles.Count - 1; i < xLength; i++)
            {
                GameObject newTile = Instantiate(block, blocksHolder.transform);
                tiles.Add(newTile);
            }
        }

        for (int i = 0; i < xLength; i++)
        {
            if (tiles[i].Equals(null))
            {
                tiles[i] = Instantiate(block, blocksHolder.transform, false);
            }

            float index = i;
            tiles[i].transform.localPosition = new Vector2((index / (xLength)), 0);
            tiles[i].transform.localScale = new Vector2(1 / xLength, 1);
        }

        if (xLength == 1)
        {
            blocksHolder.transform.localPosition = new Vector2(0, 0);
        }
        else if (xLength > 1)
        {
            blocksHolder.transform.localPosition = new Vector2((-.5f / xLength) * (xLength - 1), 0);
        }
    }

    private void OnGUI()
    {
        //transform.localScale = new Vector2(Mathf.RoundToInt(transform.localScale.x), transform.localScale.y);
        //gameObject.AddComponent<ScriptedFloor>();
    }

    public static Mesh CreateCubeMesh(MeshFilter filter)
    {
        Vector3[] vertices = {
            new Vector3 (0, 0, 0),
            new Vector3 (1, 0, 0),
            new Vector3 (1, 1, 0),
            new Vector3 (0, 1, 0),
            new Vector3 (0, 1, 1),
            new Vector3 (1, 1, 1),
            new Vector3 (1, 0, 1),
            new Vector3 (0, 0, 1),
        };

        int[] triangles = {
            0, 2, 1, //face front
			0, 3, 2,
            2, 3, 4, //face top
			2, 4, 5,
            1, 2, 5, //face right
			1, 5, 6,
            0, 7, 4, //face left
			0, 4, 3,
            5, 4, 7, //face back
			5, 7, 6,
            0, 6, 7, //face bottom
			0, 1, 6
        };

        Mesh mesh = filter.sharedMesh;
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.Optimize();
        mesh.RecalculateNormals();

        return mesh;
    }
}
