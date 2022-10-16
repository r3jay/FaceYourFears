using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class baseController : MonoBehaviour , IDamageable
{
    [SerializeField] int HP;
    [SerializeField] GameObject house100;
    [SerializeField] GameObject house75;
    [SerializeField] GameObject house50;
    [SerializeField] GameObject house25;



    public List<Transform> targetPositions;
    
    public void takeDamage(int dmg)
    {
        gameManager.instance.houseCurrentHP -= dmg;
        gameManager.instance.updateHouseHP();

        float percentDamaged = gameManager.instance.houseCurrentHP / gameManager.instance.houseMaxHP * 100;
        if(percentDamaged <= 75)
        {
            house100.SetActive(false);
            house75.SetActive(true);
        }

        if (percentDamaged <= 50)
        {
            house75.SetActive(false);
            house50.SetActive(true);
        }

        if (percentDamaged <= 25)
        {
            house50.SetActive(false);
            house25.SetActive(true);
        }

        if (gameManager.instance.houseCurrentHP <= 0)
        {
            gameManager.instance.houseIsDestroyed();
        }
    }
}
