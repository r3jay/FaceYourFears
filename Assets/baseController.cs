using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class baseController : MonoBehaviour , IDamageable
{
    [SerializeField] int HP;
    public List<Transform> targetPositions;
    
    public void takeDamage(int dmg)
    {
        HP -= dmg;

        if(HP <= 0)
        {
            gameManager.instance.PlayerIsDead();
        }
    }
}
