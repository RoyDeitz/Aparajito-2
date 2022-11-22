using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum GunType 
    {
        SemiAuto,
        FullAuto
    }
    public int maxAmmo;
    public int currentAmmo;
    public int magCapacity;
    public int currentMag;
    public float firingRate;
    public float burstInterval;
    public float reloadTime;
    public float range;
    public int bulletDamage;
    public Transform shootingPoint;
    public GameObject muzzleFlash;

    public AudioClip fireSound;
    public GameObject GunModel;

    public void Shoot() 
    {
    
    }

    public void Reload() 
    {
    
    }

}
