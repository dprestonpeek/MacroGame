using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScriptedWall : ScriptedObject
{
    public override void UseMoveTool()
    {
        base.UseMoveTool();
    }

    public override void UseRectTool()
    {
        base.UseRectTool();

        transform.localScale = new Vector2(transform.localScale.x, yLength);
        transform.position = currPos;
        //currScale = transform.localScale;
    }

    public override void UpdateTiles()
    {
        base.UpdateTiles();

        if (yLength != tiles.Count && block != null)
        {
            if (yLength > 0)
            {
                if (tiles.Count > yLength)
                {
                    for (int i = (int)yLength - 1; i < tiles.Count; i++)
                    {
                        DestroyImmediate(tiles[i].gameObject);
                        tiles.RemoveAt(i);
                    }
                }
                else if (tiles.Count < yLength)
                {
                    for (int i = tiles.Count - 1; i < yLength; i++)
                    {
                        GameObject newTile = Instantiate(block, blocksHolder.transform);
                        tiles.Add(newTile);
                    }
                }

                for (int i = 0; i < yLength; i++)
                {
                    if (tiles[i].Equals(null))
                    {
                        tiles[i] = Instantiate(block, blocksHolder.transform, false);
                    }

                    float index = i;
                    tiles[i].transform.localPosition = new Vector2(0, (index / (yLength)));
                    tiles[i].transform.localScale = new Vector2(1, 1 / yLength);
                }

                if (yLength == 1)
                {
                    blocksHolder.transform.localPosition = new Vector2(0, 0);
                }
                else if (yLength > 1)
                {
                    blocksHolder.transform.localPosition = new Vector2(0, (-.5f / yLength) * (yLength - 1));
                }
            }
        }
    }
}
