using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIGlobal : MonoBehaviour
{
    public Vector3 lastPlayerPosition;
    PlayerMovementController player;
    public float memoryTime;
    float counter;
    public bool isPlayerDetected;
    void Start()
    {
        player = FindObjectOfType<PlayerMovementController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerDetected)
        {
            if (counter <= 0)
            {
                counter = memoryTime;
                lastPlayerPosition = Vector3.zero;
                isPlayerDetected = false;
            }
            else 
            {
                counter -= Time.deltaTime;
                lastPlayerPosition = player.transform.position;
            }
        }
        
    }
}
