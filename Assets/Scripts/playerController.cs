using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour , IDamageable
{
    [Header("----- Components ------ ")]
    [SerializeField] CharacterController controller;

    [Header("------ Player Attributes -----")]
    [SerializeField] int currentHP;
    [SerializeField] int minHP;
    [SerializeField] int maxHP;
    [SerializeField] float playerSpeed;
    [SerializeField] float sprintModifier;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravityValue;
    [Range(0, 10)] public int boostTime;
    int timesJumped;
    [SerializeField] int jumpsMax;

    [Header("------ Gun Stats -----")]
    [SerializeField] float fireRate;
    [SerializeField] int shootDistance;
    [SerializeField] int playerDamage;
    [SerializeField] List<weaponStats> weaponStat = new List<weaponStats>();
    int selectedWeapon;
    [SerializeField] GameObject weaponModel;
    bool isShooting;


    [Header("----- Audio -----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] damagedSound;
    [Range(0, 1)] [SerializeField] float damagedSoundVolume;
    [SerializeField] AudioClip[] jumpSound;
    [Range(0, 1)] [SerializeField] float jumpSoundVolume;
    [SerializeField] AudioClip[] footstepSound;
    [Range(0, 1)] [SerializeField] float footstepSoundVolume;
    [Range(0, 1)] [SerializeField] float gunShotSoundVolume;
    [Range(0, 1)] [SerializeField] float pickUpSoundVolume;

    private Vector3 playerVelocity;
    private bool groundedPlayer;
    Vector3 move;

    int HPOrig;
    float playerSpeedOriginal;
    int playerDamageOrig;
    bool isSprinting;
    bool playingFootsteps;
    bool usingSpeedBoost;
    bool usingDamageBoost;
    float speedTimeLeft;
    float damageTimeLeft;

    private void Start()
    {
        HPOrig = currentHP;
        respawn();
        playerSpeedOriginal = playerSpeed;
    }

    void Update()
    {
        if (!gameManager.instance.isPaused)
        {
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);
            movePlayer();
            sprint();
            StartCoroutine(footSteps());
            StartCoroutine(shoot());
            weaponSelect();

            if (usingSpeedBoost)
            {
                speedTimeLeft -= Time.deltaTime;
                gameManager.instance.SpeedBoostBar.fillAmount = (speedTimeLeft / (float)boostTime);

            }

            if (usingDamageBoost)
            {
                damageTimeLeft -= Time.deltaTime;
                gameManager.instance.DamageBoostBar.fillAmount = (damageTimeLeft / (float)boostTime);

            }
        }
    }

    void movePlayer()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            timesJumped = 0;
        }

        move = (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * playerSpeed);

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && timesJumped < jumpsMax)
        {
            playerVelocity.y = jumpHeight;
            timesJumped++;
            aud.PlayOneShot(jumpSound[Random.Range(0, jumpSound.Length)], jumpSoundVolume);
        }


        playerVelocity.y -= gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            playerSpeed *= sprintModifier;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            playerSpeed = playerSpeedOriginal;
            isSprinting = false;
        }
    }

    IEnumerator footSteps()
    {
        if (!playingFootsteps && controller.isGrounded && move.normalized.magnitude > 0.3f)
        {
            playingFootsteps = true;
            aud.PlayOneShot(footstepSound[Random.Range(0, footstepSound.Length)], footstepSoundVolume);

            if (isSprinting)
            {
                yield return new WaitForSeconds(0.3f);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }

            playingFootsteps = false;
        }
    }

    //============================================================================================================
    //    UNCOMMENT WHEN GUNSTAT IS ADDED
    //============================================================================================================
    void weaponSelect()
    {
        if (weaponStat.Count > 1)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedWeapon < weaponStat.Count - 1)
            {
                selectedWeapon++;
                fireRate = weaponStat[selectedWeapon].shootRate;
                shootDistance = weaponStat[selectedWeapon].shootDist;
                playerDamage = weaponStat[selectedWeapon].shootDamage;

                weaponModel.GetComponent<MeshFilter>().sharedMesh = weaponStat[selectedWeapon].model.GetComponent<MeshFilter>().sharedMesh;
                weaponModel.GetComponent<MeshRenderer>().sharedMaterial = weaponStat[selectedWeapon].model.GetComponent<MeshRenderer>().sharedMaterial;
                //aud.PlayOneShot(gunStat[selectedWeapon].pickupSound, pickUpSoundVolume);

            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedWeapon > 0)
            {
                selectedWeapon--;
                fireRate = weaponStat[selectedWeapon].shootRate;
                shootDistance = weaponStat[selectedWeapon].shootDist;
                playerDamage = weaponStat[selectedWeapon].shootDamage;

                weaponModel.GetComponent<MeshFilter>().sharedMesh = weaponStat[selectedWeapon].model.GetComponent<MeshFilter>().sharedMesh;
                weaponModel.GetComponent<MeshRenderer>().sharedMaterial = weaponStat[selectedWeapon].model.GetComponent<MeshRenderer>().sharedMaterial;
                //aud.PlayOneShot(gunStat[selectedWeapon].pickupSound, pickUpSoundVolume);

            }
        }
    }
    //============================================================================================================
    //    UNCOMMENT WHEN GUNSTAT IS ADDED
    //============================================================================================================

    public void takeDamage(int _damage)
    {
        currentHP -= _damage;
        aud.PlayOneShot(damagedSound[Random.Range(0, damagedSound.Length)], damagedSoundVolume);
        updatePlayerHPBar();

        if (currentHP <= 0)
        {
            gameManager.instance.PlayerIsDead();
        }
        else
            StartCoroutine(damageFlash());
    }

    IEnumerator damageFlash()
    {
        gameManager.instance.playerDamage.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamage.SetActive(false);

    }

    IEnumerator shoot()
    {
        if (!isShooting && Input.GetButtonDown("Fire1"))   // ADD "&& gunStat.Count > 0 when gunstat added
        {
            isShooting = true;
            //aud.PlayOneShot(gunStat[selectedGun].sound, gunShotSoundVolume);

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(.5f, .5f)), out hit, shootDistance))
            {
                if (hit.collider.GetComponent<IDamageable>() != null)
                {
                    hit.collider.GetComponent<IDamageable>().takeDamage(playerDamage);
                }

                //Instantiate(gunStat[selectedWeapon].hitEffect, hit.point, transform.rotation);
            }


            yield return new WaitForSeconds(fireRate);
            isShooting = false;
        }
    }

    public void respawn()
    {
        controller.enabled = false;
        currentHP = maxHP;
        updatePlayerHPBar();
        transform.position = gameManager.instance.playerSpawnPosition.transform.position;
        gameManager.instance.CursorUnlockUnpause();
        controller.enabled = true;
    }


    public void updatePlayerHPBar()
    {
        gameManager.instance.HPBar.fillAmount = (float)currentHP / (float)maxHP;
    }

    //============================================================================================================
    //    UNCOMMENT WHEN GUNSTAT IS ADDED
    //============================================================================================================
    public void weaponPickup(weaponStats stats)
    {
        fireRate = stats.shootRate;
        shootDistance = stats.shootDist;
        playerDamage = stats.shootDamage;

        weaponModel.GetComponent<MeshFilter>().sharedMesh = stats.model.GetComponent<MeshFilter>().sharedMesh;
        weaponModel.GetComponent<MeshRenderer>().sharedMaterial = stats.model.GetComponent<MeshRenderer>().sharedMaterial;

        weaponStat.Add(stats);
        selectedWeapon = weaponStat.Count - 1;

        //aud.PlayOneShot(weaponStat[selectedWeapon].pickupSound, pickUpSoundVolume);
    }
    //============================================================================================================
    //    UNCOMMENT WHEN GUNSTAT IS ADDED
    //============================================================================================================

    public void boostPickUp(boostStats stats)
    {
        if (stats.boostType.name == "SpeedBoost")
        {
            StartCoroutine(giveSpeed(stats.boostMultiplier));

        }
        else if (stats.boostType.name == "HealthBoost")
        {
            if (currentHP < 100)
            {
                giveHP(stats.boostMultiplier);
            }

        }
        else if (stats.boostType.name == "DamageBoost")
        {
            StartCoroutine(giveDamage(2));
        }
    }

    public void giveHP(int amount)
    {
        currentHP *= amount;
        updatePlayerHPBar();
    }

    public void giveJump(int _amount)
    {
        jumpsMax += _amount;
    }
    public IEnumerator giveSpeed(int amount)
    {
        if (!usingSpeedBoost)
        {
            
            usingSpeedBoost = true;
            playerSpeed *= amount;
            speedTimeLeft = boostTime;

            gameManager.instance.SpeedBoostBar.fillAmount = 1;


            yield return new WaitForSeconds(boostTime);

            playerSpeed = playerSpeedOriginal;
        }
        usingSpeedBoost = false;
    }

    IEnumerator giveDamage(int amount)
    {
        if (!usingDamageBoost)
        {
            usingDamageBoost = true;
            playerDamageOrig = playerDamage;
            playerDamage *= amount;
            damageTimeLeft = boostTime;

            gameManager.instance.DamageBoostBar.fillAmount = 1;
            yield return new WaitForSeconds(boostTime);
            playerDamage = playerDamageOrig;
            usingDamageBoost = false;
        }

    }
}
