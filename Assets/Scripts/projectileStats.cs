using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class projectileStats : ScriptableObject
{
    public float shootRate;
    public int proDist;
    public int shootDamage;
    public float proSpeed;
    public float arcRange;
    public float destroyTime;
    //public int proCount;
    public GameObject projectile;
    public GameObject impactEffect;
    public GameObject muzzle;
    public AudioClip shotSound;
    //public Animator anime;
}