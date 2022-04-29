using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptedRect : ScriptedObject
{
    [SerializeField]
    List<GameObject> yTiles = new List<GameObject>();

    public override void Start()
    {
        base.Start();
    }

    public override void UseMoveTool()
    {
        base.UseMoveTool();
    }

    public override void UseRectTool()
    {
        base.UseRectTool();

        transform.localScale = new Vector3(xLength, yLength, 1);
        transform.position = currPos;
    }

    public override void Update()
    {
        base.Update();
        UpdateTiles();
        if (!Application.isPlaying)
        {
            base.UpdateLayer();
        }
    }

    public override void UpdateTiles()
    {
        base.UpdateTiles();

        if (xLength != tiles.Count && block != null)
        {
            if (xLength > 0)
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
                    tiles[i].transform.localPosition = new Vector3(index / xLength, 0);
                    tiles[i].transform.localScale = new Vector2(1 / xLength, 1);
                }

                if (xLength == 1)
                {
                    blocksHolder.transform.localPosition = new Vector3(0, 0, 0);
                }
                else if (xLength > 1)
                {
                    blocksHolder.transform.localPosition = new Vector3((-.5f / xLength) * (xLength - 1), 0, 0);
                }
            }
        }
        if (yLength - 1 != yTiles.Count && block != null)
        {
            if (yLength - 1 > 0)
            {
                if (yTiles.Count > yLength - 1)
                {
                    for (int i = (int)yLength - 1; i < yTiles.Count; i++)
                    {
                        DestroyImmediate(yTiles[i].gameObject);
                        yTiles.RemoveAt(i);
                    }
                }
                else if (yTiles.Count < yLength - 1)
                {
                    for (int i = yTiles.Count; i < yLength; i++)
                    {
                        Vector3 pos = new Vector3(blocksHolder.transform.position.x, blocksHolder.transform.position.y + i, blocksHolder.transform.position.z);
                        GameObject newBlocksHolder = Instantiate(blocksHolder, pos, Quaternion.identity, transform);
                        yTiles.Add(newBlocksHolder);
                        ScriptedFloor newFloor = newBlocksHolder.AddComponent<ScriptedFloor>();
                        newFloor.block = block;
                        newFloor.blocksHolder = newBlocksHolder;
                    }
                }

                for (int i = 0; i < yLength - 1; i++)
                {
                    if (yTiles[i].Equals(null))
                    {
                        yTiles[i] = Instantiate(block, blocksHolder.transform, false);
                    }

                    yTiles[i].transform.localPosition = new Vector3(0, i / yLength);
                    yTiles[i].transform.localScale = new Vector3(1, 1 / yLength, 1);
                }

                if (yLength == 1)
                {
                    blocksHolder.transform.localPosition = new Vector3(0, 0, 0);
                    blocksHolder.transform.localScale = Vector3.one;
                }
                else if (yLength > 1)
                {
                    //blocksHolder.transform.localPosition = new Vector3(blocksHolder.transform.localPosition.x, (-.5f / yLength) * (yLength - 1), 0);

                    blocksHolder.transform.localScale = new Vector3(1, 1, 1);
                }
            }
            else
            {
                foreach (GameObject go in yTiles)
                {
                    DestroyImmediate(go);
                }
                yTiles.Clear();
            }
        }
        base.RemoveExtraObjects();
    }

}
