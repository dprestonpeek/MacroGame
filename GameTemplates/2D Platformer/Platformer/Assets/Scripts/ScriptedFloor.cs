using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScriptedFloor : ScriptedObject
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

        if (xLength != tiles.Count && block != null) //if the tiles have been adjusted
        {
            if (xLength > 0) //don't allow negative expansion
            {
                //before updating the tiles, compare the list of virtual tiles to the tangible objects.
                if (tiles.Count > xLength) //if there are more virtual tiles than tangible objects, remove the extras
                {
                    for (int i = (int)xLength - 1; i < tiles.Count; i++)
                    {
                        DestroyImmediate(tiles[i].gameObject);
                        tiles.RemoveAt(i);
                    }
                }
                else if (tiles.Count < xLength) //if there are less virtual tiles than tangible objects, create the missing objects
                {
                    for (int i = tiles.Count - 1; i < xLength; i++)
                    {
                        GameObject newTile = Instantiate(block, blocksHolder.transform);
                        newTile.tag = "Block";
                        tiles.Add(newTile);
                    }
                }

                //now that the list matches the object, create new tiles according to new adjustments
                for (int i = 0; i < xLength; i++)
                {
                    if (tiles[i].Equals(null)) //if the tile became null, reinstantiate it
                    {
                        tiles[i] = Instantiate(block, blocksHolder.transform, false);
                    }

                    //adjust position and scale to accommodate any new or removed tiles

                    float index = i;
                    tiles[i].transform.localPosition = new Vector3(index / xLength, 0); //ensures tiles are equidistant
                    tiles[i].transform.localScale = new Vector2(1 / xLength, 1); //ensures parent size matches tile size
                }

                //center the tile if only 1 exists, otherwise align to grid
                //if (xLength == 1)
                //{
                //    blocksHolder.transform.localPosition = new Vector3(0, 0, 0);
                //}
                //else if (xLength > 1)
                //{
                //    blocksHolder.transform.localPosition = new Vector3((-.5f / xLength) * (xLength - 1) /* + 1/xLength*/, 0, 0);
                //}
                blocksHolder.transform.localPosition = new Vector3((-.5f / xLength) * (xLength - 1) /* + 1/xLength*/, 0, 0);
            }
        }
    }

}