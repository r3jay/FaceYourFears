using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class houseController : MonoBehaviour, IDamageable
{
    [Header("----- House Stats -----")]
    [SerializeField] int HP;
    [SerializeField] int minHP;
    [SerializeField] int maxHP;
    public List<Transform> targetPositions;

    int HPOrig;
    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.instance.isPaused)
        {
            HP = Mathf.Clamp(HP, 0, maxHP);
        }
    }

    public void takeDamage(int dmg)
    {
        HP -= dmg;
        
        if(HP <= HPOrig / 2)
        {
            //maybe warn player?
        }

        if (HP <= 0)
        {
            gameManager.instance.houseIsDestroyed();
        }
    }

    public void updateHouseHP()
    {
        gameManager.instance.houseHPBar.fillAmount = (float)HP / (float)HPOrig;
    }
}
