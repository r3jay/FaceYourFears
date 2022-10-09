using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class projectileStats : ScriptableObject
{
    [Header("----- Stats -----")]
    public float shootRate;
    public float proSpeed;
    public int proDistance;
    public int proDamage;
    public int proCount;

    [Header("----- Status Effects -----")]
    public float dot;
    public bool aoe;
    public float pushBackDistance;
    public float slowDown;
    public bool freezeStun;

    [Header("----- Player Feedback -----")]
    public GameObject projectile;
    public GameObject pickupModel;
    //public AudioClip sound;
    //public GameObject hitEffect;
}
