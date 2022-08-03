//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[ExecuteInEditMode]
//public class CameraBuffer : ScriptedObject
//{
//    public Camera cam;
//    public GameObject bufferParent;
//    [SerializeField]
//    public Vector3 bufferOffset;
//    public PlayerMovement playerObj;

//    GameObject leftBoundary;
//    GameObject rightBoundary;
//    GameObject topBoundary;
//    GameObject botBoundary;

//    [SerializeField]
//    [Range(0, 1)]
//    public float bufferSpeed = .5f;

//    bool initialized = false;
//    bool tilesUpdated = false;

//    public void Initialize()
//    {
//        if (playerObj == null)
//        {
//            playerObj = FindObjectOfType<PlayerMovement>();
//        }
//        if (leftBoundary == null)       //assume all boundaries are null
//        {
//            foreach (BufferBoundary boundary in GetComponentsInChildren<BufferBoundary>())
//            {
//                if (boundary.name == "Left")
//                {
//                    leftBoundary = boundary.gameObject;
//                }
//                if (boundary.name == "Right")
//                {
//                    rightBoundary = boundary.gameObject;
//                }
//                if (boundary.name == "Top")
//                {
//                    topBoundary = boundary.gameObject;
//                }
//                if (boundary.name == "Bottom")
//                {
//                    botBoundary = boundary.gameObject;
//                }
//            }
//        }
//        initialized = true;
//    }

//    public override void UseMoveTool()
//    {
//        if (!Application.isPlaying)
//        {
//            transform.position = new Vector3(Mathf.RoundToInt(transform.position.x) + .5f, Mathf.RoundToInt(transform.position.y) - .5f, 0);
//            currPos = transform.position;
//        }
//    }

//    public override void UseRectTool()
//    {
//        if (!Application.isPlaying)
//        {
//            base.UseRectTool();
//            transform.localScale = new Vector2(xLength, yLength);
//            transform.position = currPos;

//            UpdateTiles();
//            if (!tilesUpdated)
//            {
//                FixBlockSize();
//            }
//        }
//    }

//    public override void Update()
//    {
//        if (!Application.isPlaying)
//        {
//            base.Update();
//            if (!CheckTiles())
//            {
//                UpdateTiles();
//            }

//            Vector3 newPos = new Vector3(cam.transform.position.x + bufferOffset.x, cam.transform.position.y + bufferOffset.y, -10);
//            transform.position = Vector3.Lerp(transform.position, newPos, bufferSpeed);
//        }
//    }

//    public override void UpdateTiles()
//    {
//        if (tiles.Count != 4)
//        {
//            for (int i = 0; i < tiles.Count; i++)
//            {
//                DestroyImmediate(tiles[i].gameObject);
//            }
//            tiles.Clear();

//            leftBoundary = GameObject.CreatePrimitive(PrimitiveType.Cube);
//            rightBoundary = GameObject.CreatePrimitive(PrimitiveType.Cube);
//            topBoundary = GameObject.CreatePrimitive(PrimitiveType.Cube);
//            botBoundary = GameObject.CreatePrimitive(PrimitiveType.Cube);

//            leftBoundary.name = "Left";
//            rightBoundary.name = "Right";
//            topBoundary.name = "Top";
//            botBoundary.name = "Bottom";

//            leftBoundary.transform.localScale = new Vector3(1, 1, 1);
//            rightBoundary.transform.localScale = new Vector3(1, 1, 1);
//            topBoundary.transform.localScale = new Vector3(1, 1, 1);
//            botBoundary.transform.localScale = new Vector3(1, 1, 1);

//            leftBoundary.transform.parent = transform;
//            rightBoundary.transform.parent = transform;
//            topBoundary.transform.parent = transform;
//            botBoundary.transform.parent = transform;

//            leftBoundary.transform.position = new Vector3((transform.localScale.x / -2) - .5f, transform.localPosition.y, 0);
//            rightBoundary.transform.position = new Vector3((transform.localScale.x / 2) + .5f, transform.localPosition.y, 0);
//            topBoundary.transform.position = new Vector3(transform.localPosition.x, transform.localScale.y / 2, 0);
//            botBoundary.transform.position = new Vector3(transform.localPosition.x, -transform.localScale.y / 2, 0);

//            leftBoundary.GetComponent<MeshRenderer>().enabled = false;
//            rightBoundary.GetComponent<MeshRenderer>().enabled = false;
//            topBoundary.GetComponent<MeshRenderer>().enabled = false;
//            botBoundary.GetComponent<MeshRenderer>().enabled = false;

//            leftBoundary.GetComponent<BoxCollider>().isTrigger = true;
//            rightBoundary.GetComponent<BoxCollider>().isTrigger = true;
//            topBoundary.GetComponent<BoxCollider>().isTrigger = true;
//            botBoundary.GetComponent<BoxCollider>().isTrigger = true;

//            leftBoundary.AddComponent<BufferBoundary>().camBuffer = this;
//            rightBoundary.AddComponent<BufferBoundary>().camBuffer = this;
//            topBoundary.AddComponent<BufferBoundary>().camBuffer = this;
//            botBoundary.AddComponent<BufferBoundary>().camBuffer = this;

//            //Instantiate(block, blocksHolder.transform);
//            tiles.Add(leftBoundary);
//            tiles.Add(rightBoundary);
//            tiles.Add(topBoundary);
//            tiles.Add(botBoundary);

//            tilesUpdated = true;
//            return;
//        }
//        tilesUpdated = false;
//    }

//    private bool CheckTiles()
//    {
//        if (tiles.Count == 4)
//        {
//            foreach (GameObject tile in tiles)
//            {
//                if (tile.name == "Left")
//                {
//                    if (tile.transform.localPosition != new Vector3((transform.localScale.x / -2) - .5f, transform.localPosition.y, 0))
//                    {
//                        return false;
//                    }
//                    if (tile.transform.localPosition != new Vector3(1, 10, 1))
//                    {
//                        return false;
//                    }
//                }
//                else if (tile.name == "Right")
//                {
//                    if (tile.transform.position != new Vector3((transform.localScale.x / 2) + .5f, transform.localPosition.y, 0))
//                    {
//                        return false;
//                    }
//                    if (tile.transform.localScale != new Vector3(1, 10, 1)) 
//                    {
//                        return false;
//                    }
//                }
//                else if (tile.name == "Top")
//                {
//                    if (tile.transform.position != new Vector3(transform.localPosition.x, transform.localScale.y, 0))
//                    {
//                        return false;
//                    }
//                    if (tile.transform.localScale != new Vector3(15, 1, 1))
//                    {
//                        return false;
//                    }
//                }
//                else if (tile.name == "Bottom")
//                {
//                    if (tile.transform.position != new Vector3(transform.localPosition.x, -transform.localScale.y, 0))
//                    {
//                        return false;
//                    }
//                    if (tile.transform.localScale != new Vector3(15, 1, 1))
//                    {
//                        return false;
//                    }
//                }
//            }
//            return true;
//        }
//        return false;
//    }

//    private void FixBlockSize()
//    {
//        //start with x
//        Vector3 newPos = Vector3.zero;
//        Vector3 newScale = Vector3.zero;

//        foreach (GameObject tile in tiles)
//        {
//            for (int i = 0; i < transform.localScale.x; i++)
//            {
//                float index = i;
//                newPos.x = index / transform.localScale.x;
//                newScale.x = 1 / transform.localScale.x;
//            }

//            for (int i = 0; i < transform.localScale.y; i++)
//            {
//                float index = i;
//                newPos.y = index / transform.localScale.y;
//                newScale.y = 1 / transform.localScale.y;
//            }


//            //tile.transform.localPosition = newPos;
//            tile.transform.localScale = newScale;
//        }

//    }
//}
