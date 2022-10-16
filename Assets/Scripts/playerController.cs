using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamageable
{
    [Header("----- Components ------ ")]
    [SerializeField] CharacterController controller;
    [SerializeField] Animator anime;

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

    [HideInInspector] public bool stunStatusEffectActive;
    [HideInInspector] public float stunTime;
    bool stunTimerRunning;


    [Header("------ Projectile Stats -----")]
    [SerializeField] float fireRate;
    [SerializeField] int shootDistance;
    [SerializeField] public int playerDamage;
    [SerializeField] List<weaponStats> weaponStat = new List<weaponStats>();
    int selectedWeapon;
    [SerializeField] GameObject weaponModel;
    [SerializeField] List<projectileStats> proList = new List<projectileStats>();
    int selectedPro;
    public Transform shootPos;
    private Vector3 destination;
    public GameObject projectile;
    public GameObject impactE;
    public GameObject muzzle;
    public float proSpeed;
    public float arcRange;
    public float destroyTime;
    public int DOTdamage;
    public float DOTtime;
    public bool stun;
    public bool isAoe;
    public float slowDown;
    


    public float statusEffectTime_stun;
    public float statusEffectTime_slow;
    public float statusEffectTime_poison;
    public float timeBetweenTicks;
    [HideInInspector] public bool isTakingPoisonDamage;
    bool isShooting;


    [Header("----- Audio -----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] damagedSound;
    [Range(0, 1)] [SerializeField] float damagedSoundVolume;
    [SerializeField] AudioClip[] jumpSound;
    [Range(0, 1)] [SerializeField] float jumpSoundVolume;
    [SerializeField] AudioClip[] footstepSound;
    [Range(0, 1)] [SerializeField] float footstepSoundVolume;
    [Range(0, 1)] [SerializeField] float weaponShotSoundVolume;

    [SerializeField] AudioClip pickupSound;
    [Range(0, 1)] [SerializeField] float pickUpSoundVolume;

    [SerializeField] AudioClip projectileShotSound;
    [Range(0, 1)] [SerializeField] float proShotSoundVolume;


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

    [Header("----- Respawn Info -----")]
    [SerializeField] int livesLeft;
    int livesLeftOrig;

    private void Start()
    {
        livesLeftOrig = livesLeft;
        HPOrig = currentHP;
        playerSpeedOriginal = playerSpeed;
        respawn();
    }

    void Update()
    {
        if (!gameManager.instance.isPaused)
        {
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);
            if (!stunStatusEffectActive)
            {
                movePlayer();
                sprint();
                StartCoroutine(footSteps());
            }
            else if(!stunTimerRunning)
            {
                StartCoroutine(stunTimer());
            }
            StartCoroutine(shoot());
            projectileSelect();
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
        anime.SetFloat("Speed", move.magnitude);
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
                //aud.PlayOneShot(weaponStat[selectedWeapon].pickupSound, pickUpSoundVolume);

            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedWeapon > 0)
            {
                selectedWeapon--;
                fireRate = weaponStat[selectedWeapon].shootRate;
                shootDistance = weaponStat[selectedWeapon].shootDist;
                playerDamage = weaponStat[selectedWeapon].shootDamage;

                weaponModel.GetComponent<MeshFilter>().sharedMesh = weaponStat[selectedWeapon].model.GetComponent<MeshFilter>().sharedMesh;
                weaponModel.GetComponent<MeshRenderer>().sharedMaterial = weaponStat[selectedWeapon].model.GetComponent<MeshRenderer>().sharedMaterial;
                aud.PlayOneShot(pickupSound, pickUpSoundVolume);

            }
        }
    }
    void projectileSelect()
    {
        if (proList.Count > 1)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedPro < proList.Count - 1)
            {
                
                selectedPro++;
                fireRate = proList[selectedPro].shootRate;
                projectile = proList[selectedPro].projectile;
                playerDamage = proList[selectedPro].shootDamage;
                impactE = proList[selectedPro].impactEffect;
                arcRange = proList[selectedPro].arcRange;
                muzzle = proList[selectedPro].muzzle;
                

                //weaponModel.GetComponent<MeshFilter>().sharedMesh = weaponStat[selectedWeapon].model.GetComponent<MeshFilter>().sharedMesh;
                //weaponModel.GetComponent<MeshRenderer>().sharedMaterial = weaponStat[selectedWeapon].model.GetComponent<MeshRenderer>().sharedMaterial;
                aud.PlayOneShot(pickupSound, pickUpSoundVolume);

            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedPro > 0)
            {
               
                selectedPro--;
                fireRate = proList[selectedPro].shootRate;
                projectile = proList[selectedPro].projectile;
                playerDamage = proList[selectedPro].shootDamage;
                impactE = proList[selectedPro].impactEffect;
                arcRange = proList[selectedPro].arcRange;
                muzzle = proList[selectedPro].muzzle;
               
               

                //weaponModel.GetComponent<MeshFilter>().sharedMesh = weaponStat[selectedWeapon].model.GetComponent<MeshFilter>().sharedMesh;
                //weaponModel.GetComponent<MeshRenderer>().sharedMaterial = weaponStat[selectedWeapon].model.GetComponent<MeshRenderer>().sharedMaterial;
                aud.PlayOneShot(pickupSound, pickUpSoundVolume);

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
            livesLeft--;
            if (livesLeft > 0)
            {
                gameManager.instance.PlayerCanRespawn();
            }
            else
            {
                livesLeft = livesLeftOrig;
                gameManager.instance.PlayerIsDead();
            }
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
        if (!isShooting && Input.GetButtonDown("Shoot") && proList.Count > 0)
        {
            isShooting = true;

            aud.PlayOneShot(proList[selectedPro].shotSound, proShotSoundVolume);

            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
                destination = hit.point;
            else
                destination = ray.GetPoint(proList[selectedPro].proDist);

            //Instantiate(projectile, shootPos.transform.position, transform.rotation);
            anime.SetTrigger("Attack");
            yield return new WaitForSeconds(0.3f);
            instantiateProjectile(Camera.main.transform);


            //if (hit.collider.GetComponent<IDamageable>() != null)
            //    hit.collider.GetComponent<IDamageable>().takeDamage(playerDamage);


            //if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(.5f, .5f)), out hit, shootDistance))
            //{

            //    if (hit.collider.GetComponent<IDamageable>() != null)
            //    {
            //        hit.collider.GetComponent<IDamageable>().takeDamage(playerDamage);
            //    }
            //    Instantiate(weaponStat[selectedWeapon].hitEffect, hit.point, transform.rotation);
            //}
            yield return new WaitForSeconds(fireRate);
            isShooting = false;
        }
    }
    void instantiateProjectile(Transform shotPoint)
    {
        Instantiate(proList[selectedPro].projectile, shotPoint.transform.position, transform.rotation);
        //var projectileObj = Instantiate(proList[selectedPro].projectile, shotPoint.position, Quaternion.identity);
        //projectileObj.GetComponent<Rigidbody>().velocity = (destination - shotPoint.position).normalized * proList[selectedPro].proSpeed;
        //iTween.PunchPosition(projectileObj, new Vector3(Random.Range(-proList[selectedPro].arcRange, proList[selectedPro].arcRange), Random.Range(-proList[selectedPro].arcRange, proList[selectedPro].arcRange), 0), Random.Range(0.5f, 2));
        //Instantiate(proList[selectedPro].muzzle, shotPoint.transform.position, transform.rotation); - problem destroying after instantiating
    }

    public void respawn()
    {
            controller.enabled = false;
            currentHP = maxHP;
            updatePlayerHPBar();
            //gameManager.instance.houseCurrentHP = gameManager.instance.houseMaxHP;
            //gameManager.instance.updateHouseHP();
            transform.position = gameManager.instance.playerSpawnPosition.transform.position;
            controller.enabled = true;
            gameManager.instance.CursorUnlockUnpause();
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
        aud.PlayOneShot(pickupSound, pickUpSoundVolume);
    }
    public void projectilePickup(projectileStats stats)
    {
        fireRate = stats.shootRate;
        playerDamage = stats.shootDamage;
        projectile = stats.projectile;
        impactE = stats.impactEffect;
        arcRange = stats.arcRange;
        muzzle = stats.muzzle;
        proSpeed = stats.proSpeed;
        destroyTime = stats.destroyTime;
        DOTdamage = stats.DOTdamage;
        DOTtime = stats.DOTtime;
        timeBetweenTicks = stats.timeBetweenTicks;
        statusEffectTime_poison = stats.statusEffectTime_poison;
        statusEffectTime_slow = stats.statusEffectTime_slow;
        statusEffectTime_stun = stats.statusEffectTime_stun;
        stun = stats.stun;
        slowDown = stats.slowDown;
        
        isAoe = stats.isAoe;
        

        proList.Add(stats);
        selectedPro = proList.Count - 1;
        aud.PlayOneShot(pickupSound, pickUpSoundVolume);
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

    IEnumerator stunTimer()
    {
        stunTimerRunning = true;
        yield return new WaitForSeconds(stunTime);
        stunStatusEffectActive = false;
        stunTimerRunning = false;
        stunTime = 0;
    }
}