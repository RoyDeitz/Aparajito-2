using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableGun : MonoBehaviour
{
    public enum CollectableType 
    {
        RifleCollectible,
        SMGCollectible,
        AmmoCollectible,
        GrenadeCollectible
    }

    public CollectableType collectableType;

    public int ammoRifle;
    public int ammoSMG;
    public int numberOfGrenades; 


   
}
