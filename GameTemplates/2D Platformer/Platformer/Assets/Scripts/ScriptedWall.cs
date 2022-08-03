using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScriptedWall : ScriptedObject
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

        transform.localScale = new Vector2(transform.localScale.x, yLength);
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

        if (yLength != tiles.Count && block != null) //if the tiles have been adjusted
        {
            if (yLength > 0) //don't allow negative expansion
            {
                //before updating the tiles, compare the list of virtual tiles to the tangible objects.
                if (tiles.Count > yLength) //if there are more virtual tiles than tangible objects, remove the extras
                {
                    for (int i = (int)yLength - 1; i < tiles.Count; i++)
                    {
                        DestroyImmediate(tiles[i].gameObject);
                        tiles.RemoveAt(i);
                    }
                }
                else if (tiles.Count < yLength) //if there are less virtual tiles than tangible objects, create the missing objects
                {
                    for (int i = tiles.Count - 1; i < yLength; i++)
                    {
                        GameObject newTile = Instantiate(block, blocksHolder.transform);
                        newTile.tag = "Block";
                        tiles.Add(newTile);
                    }
                }

                //now that the list matches the object, create new tiles according to new adjustments
                for (int i = 0; i < yLength; i++)
                {
                    if (tiles[i].Equals(null)) //if the tile became null, reinstantiate it
                    {
                        tiles[i] = Instantiate(block, blocksHolder.transform, false);
                    }

                    //adjust position and scale to accommodate any new or removed tiles
                    float index = i;
                    tiles[i].transform.localPosition = new Vector2(0, index / yLength);
                    tiles[i].transform.localScale = new Vector2(1, 1 / yLength);
                }

                //center the tile if only 1 exists, otherwise align to grid
                blocksHolder.transform.localPosition = new Vector2(0, (-.5f / yLength) * (yLength - 1));
            }
        }
    }
}
