using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFOV : MonoBehaviour
{
    public float detectionTime;
    public EnemyAI enemyAI;
    public PlayerMovementController playerController;
    private void OnTriggerStay(Collider other)
    {
        StartCoroutine(Recognise(detectionTime,other));
    }

    private void OnTriggerExit(Collider other)
    {
        
    }

    IEnumerator Recognise(float time, Collider other) 
    {
        yield return new WaitForSeconds(time);
        
    }
}
