using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{


    [Header("Stats")]
    [Range(1, 10)][SerializeField] int HP;


    [Header("Components")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private LayerMask ignoreLayer;
    [SerializeField] private AudioSource aud;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private int jumpMax = 2;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = 20f;

    [Header("Weapons Settings")]
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] private int shootDamage = 10;
    [SerializeField] private float shootRate = 0.25f;
    [SerializeField] private float shootDist = 100f;
    [SerializeField] private AudioClip audioShoot;
    [Range(0, 1)][SerializeField] private float audioShootVol = 1f;
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private GameObject impactEffectPrefab;
    [SerializeField] GameObject gunModel;

    [Header("Audio")]
    [SerializeField] private AudioClip[] audioSteps;
    [Range(0, 1)][SerializeField] private float audioStepsVol = 0.9f;
    [SerializeField] private AudioClip[] audioJump;
    [Range(0, 1)][SerializeField] private float audioJumpVol = 0.9f;
    [SerializeField] private AudioClip[] audioLand;
    [Range(0, 1)][SerializeField] private float audioLandVol = 0.9f;
    [SerializeField] private AudioClip audioHurt;
    [Range(0, 1)][SerializeField] private float audioHurtVol = 0.9f;


    private float stepTimer;
    private int jumpCount;
    private Vector3 moveDir;
    private Vector3 playerVel;
    private bool isSprinting;
    private bool isJumped;
    private bool isPlayingStep;
    private bool wasGrounded;
    private float shootCooldown = 0f;
    private bool isShooting;


    int HPOrig;
    public int XP;

    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
        Move();
        HandleSprint();
        HandleLanding();
        shootCooldown -= Time.deltaTime;
        HandleShooting();
        HPOrig = HP;
    }

    void HandleShooting()
    {
        if (Input.GetButton("Fire1") && shootCooldown <= 0f)
        {
            shootCooldown = shootRate;
            Shoot();
        }
    }


 
    void Shoot()
    {
        isShooting = true;


        if (gunList.Count == 0) return;
        gunStats currentGun = gunList[gunList.Count - 1]; // Last picked up, or add a selectedGun index


        // Ammo management For when Player UI is up and running
         //ammoCur--;
         //updatePlayerUI();

        if (currentGun.shootSound != null && currentGun.shootSound.Length > 0)
            aud.PlayOneShot(currentGun.shootSound[Random.Range(0, currentGun.shootSound.Length)], currentGun.shootVol);

        Vector3 origin = Camera.main.transform.position;
        Vector3 dir = Camera.main.transform.forward;

        if (muzzleFlashPrefab != null)
        {
            var m = Instantiate(muzzleFlashPrefab, origin + dir * 0.5f, Quaternion.LookRotation(dir));
            Destroy(m, 0.1f);
        }

        Ray ray = new Ray(origin, dir);
        if (Physics.Raycast(ray, out RaycastHit hit, currentGun.shootDist, ~ignoreLayer))
        {
            if (currentGun.hitEffect != null)
            {
                var i = Instantiate(currentGun.hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(i, 2f);
            }
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
                dmg.takeDamage(currentGun.shootDamage);
        }
    }


    public void getGunStats(gunStats gun)
    {
        gunList.Add(gun);
        shootDamage = gun.shootDamage;
        shootDist = gun.shootDist;
        shootRate = gun.shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void Move()
    {
        if (controller.isGrounded)
        {
            isJumped = false;
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        moveDir = (Input.GetAxis("Horizontal") * transform.right) +
                  (Input.GetAxis("Vertical") * transform.forward);
        float currentSpeed = isSprinting ? walkSpeed * sprintMultiplier : walkSpeed;
        controller.Move(moveDir * currentSpeed * Time.deltaTime);

        HandleJump();

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        if (controller.isGrounded && moveDir.magnitude > 0.3f && !isPlayingStep)
            StartCoroutine(PlaySteps());
    }

    IEnumerator PlaySteps()
    {
        isPlayingStep = true;
        aud.PlayOneShot(audioSteps[Random.Range(0, audioSteps.Length)], audioStepsVol);
        if (!isSprinting)
            yield return new WaitForSeconds(0.5f);
        else
            yield return new WaitForSeconds(0.3f);
        isPlayingStep = false;
    }

    void HandleSprint()
    {
        if (Input.GetButtonDown("Sprint"))
            isSprinting = true;
        else if (Input.GetButtonUp("Sprint"))
            isSprinting = false;
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpForce;
            isJumped = true;
            aud.PlayOneShot(audioJump[Random.Range(0, audioJump.Length)], audioJumpVol);
        }
    }

    void HandleLanding()
    {
        if (isJumped && controller.isGrounded && audioLand.Length > 0)
            aud.PlayOneShot(audioLand[Random.Range(0, audioLand.Length)], audioLandVol);

        if (controller.isGrounded)
            isJumped = false;

        wasGrounded = controller.isGrounded;
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
    }


    //public void updatePlayerUI()
    //{
        

    //}

}
