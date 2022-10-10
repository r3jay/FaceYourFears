using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class baseController : MonoBehaviour , IDamageable
{
    [SerializeField] int HP;

    public List<Transform> targetPositions;
    
    public void takeDamage(int dmg)
    {
        gameManager.instance.houseCurrentHP -= dmg;
        gameManager.instance.updateHouseHP();

        if(gameManager.instance.houseCurrentHP <= 0)
        {
            gameManager.instance.PlayerIsDead();
        }
    }
}
