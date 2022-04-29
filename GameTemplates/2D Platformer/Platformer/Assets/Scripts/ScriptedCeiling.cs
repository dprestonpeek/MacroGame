using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptedCeiling : ScriptedObject
{
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

        transform.localScale = new Vector2(xLength, transform.localScale.y);
        transform.position = currPos;
        //currScale = transform.localScale;
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
    }

}
