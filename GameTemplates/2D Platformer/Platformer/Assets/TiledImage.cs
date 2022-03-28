using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TiledImage : MonoBehaviour
{
    [SerializeField]
    Sprite texture;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (texture.texture != null)
        {
        }
    }

    [ContextMenu("Generate Texture")]
    void GenerateTexture()
    {
        for (int i = 0; i < transform.localScale.x; i++)
        {
            Image image = gameObject.AddComponent<Image>();
            image.transform.position = new Vector2(i, 0);
        }
    }
}
