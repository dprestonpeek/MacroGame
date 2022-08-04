using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EnemyTrigger : MonoBehaviour
{
    [Tooltip("This should be set to the script on the root enemy object.")]
    [SerializeField]
    private EnemyMovement enemy;

    public enum TriggerType { INTERRUPTION, PROVOCATION }
    [Tooltip("Determines what this trigger makes the enemy do.")]
    [SerializeField]
    public TriggerType triggerType = TriggerType.INTERRUPTION;

    //[Tooltip("Determines the square size of this trigger.")]
    //[SerializeField]
    //[Range(5, 30)]
    private Vector2 triggerSize = new Vector2(15, 15);

    private void Update()
    {
        if (!Application.isPlaying)
        {
            switch (triggerType)
            {
                case TriggerType.INTERRUPTION:
                    triggerSize = new Vector2(enemy.interruptionRangeX, enemy.interruptionRangeY);
                    break;
                case TriggerType.PROVOCATION:
                    triggerSize = new Vector2(enemy.provocationRangeX, enemy.provocationRangeY);
                    break;
            }
            if (triggerSize.x > 0 && triggerSize.y > 0)
            {
                transform.localScale = new Vector3(triggerSize.x, triggerSize.y);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Application.isPlaying)
        {
            PlayerMovement player;
            if (player = other.GetComponentInParent<PlayerMovement>())
            {
                enemy.SetTarget(player.transform);
                if (triggerType == TriggerType.INTERRUPTION)
                {
                    enemy.SetInterrupted(true);
                }
                else if (triggerType == TriggerType.PROVOCATION)
                {
                    enemy.SetProvoked(true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Application.isPlaying)
        {
            PlayerMovement player;
            if (player = other.GetComponentInParent<PlayerMovement>())
            {
                enemy.SetTarget(player.transform);
                if (triggerType == TriggerType.INTERRUPTION)
                {
                    enemy.SetInterrupted(false);
                }
                else if (triggerType == TriggerType.PROVOCATION)
                {
                    enemy.SetProvoked(false);
                }
            }
        }
    }
}
