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
    [SerializeField] List<AudioClip> hitSound;
    [SerializeField] List<AudioClip> damageSound;
    [SerializeField] AudioSource aud;





    public List<Transform> targetPositions;
    
    public void takeDamage(int dmg)
    {
        System.Random rand = new System.Random();

        aud.PlayOneShot(hitSound[rand.Next(0, hitSound.Count)]);
        gameManager.instance.houseCurrentHP -= dmg;
        gameManager.instance.updateHouseHP();

        float percentDamaged = gameManager.instance.houseCurrentHP / gameManager.instance.houseMaxHP * 100;
        if(percentDamaged <= 75)
        {
            aud.PlayOneShot(damageSound[rand.Next(0, damageSound.Count)]);

            MeshRenderer meshRenderer = house100.GetComponent<MeshRenderer>();
            meshRenderer.enabled = false;
            house75.SetActive(true);
        }

        if (percentDamaged <= 50)
        {
            aud.PlayOneShot(damageSound[rand.Next(0, damageSound.Count)]);

            house75.SetActive(false);
            house50.SetActive(true);
        }

        if (percentDamaged <= 25)
        {
            aud.PlayOneShot(damageSound[rand.Next(0, damageSound.Count)]);

            house50.SetActive(false);
            house25.SetActive(true);
        }

        if (gameManager.instance.houseCurrentHP <= 0)
        {
            gameManager.instance.houseIsDestroyed();
        }
    }
}
