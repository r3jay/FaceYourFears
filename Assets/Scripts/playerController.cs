using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour , IDamageable
{
    [SerializeField] CharacterController controller;

    [SerializeField] int currentHP;
    [SerializeField] int maxHP;

    [SerializeField] float playerSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravityValue;

    int timesJumped;
    [SerializeField] int jumpsMax;

    [SerializeField] float fireRate;
    //[SerializeField] GameObject cube;
    [SerializeField] int shootDistance;
    [SerializeField] int playerDamage;
    bool isShooting;

    private Vector3 playerVelocity;
    private bool groundedPlayer;
    Vector3 move;


    private void Start()
    {
        Respawn();
    }

    void Update()
    {
        if (!gameManager.instance.isPaused)
        {
            MovePlayer();

            StartCoroutine(Shoot());
            //StartCoroutine(Build());
        }
    }

    void MovePlayer()
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
        }


        playerVelocity.y -= gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void takeDamage(int _damage)
    {
        currentHP -= _damage;
        UpdatePlayerHPBar();

        if (currentHP <= 0)
        {
            gameManager.instance.PlayerIsDead();
        }
        else
            StartCoroutine(DamageFlash());
    }

    IEnumerator DamageFlash()
    {
        gameManager.instance.playerDamage.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamage.SetActive(false);

    }

    IEnumerator Shoot()
    {
        if (!isShooting && Input.GetButtonDown("Shoot"))
        {
            isShooting = true;

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(.5f, .5f)), out hit, shootDistance))
            {
                if (hit.collider.GetComponent<IDamageable>() != null)
                {
                    hit.collider.GetComponent<IDamageable>().takeDamage(playerDamage);
                }
            }


            yield return new WaitForSeconds(fireRate);
            isShooting = false;
        }
    }
    //IEnumerator Build()
    //{
    //    if (!isShooting && Input.GetButtonDown("Fire2"))
    //    {
    //        isShooting = true;

    //        RaycastHit hit;

    //        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(.5f, .5f)), out hit, shootDistance))
    //        {
    //            Instantiate(cube, hit.point, cube.transform.rotation);
    //        }

    //        yield return new WaitForSeconds(fireRate);
    //        isShooting = false;
    //    }
    //}

    public void Respawn()
    {
        controller.enabled = false;
        currentHP = maxHP;
        UpdatePlayerHPBar();
        transform.position = gameManager.instance.playerSpawnPosition.transform.position;
        gameManager.instance.CursorUnlockUnpause();
        controller.enabled = true;
    }

    public void UpdatePlayerHPBar()
    {
        gameManager.instance.HPBar.fillAmount = (float)currentHP / (float)maxHP;
    }

    public void GiveHP(int _amount)
    {
        currentHP += _amount;
    }
    public void GiveJump(int _amount)
    {
        jumpsMax += _amount;
    }
    public void GiveSpeed(int _amount)
    {
        playerSpeed += _amount;
    }
}
