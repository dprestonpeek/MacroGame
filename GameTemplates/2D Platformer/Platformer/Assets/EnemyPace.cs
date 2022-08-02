using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPace : MonoBehaviour
{
    private EnemyMovement movement;

    [SerializeField]
    private bool DoPace = true;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<EnemyMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (movement.moveAutomatically)
        {
            movement.Pace();
        }
    }
}
