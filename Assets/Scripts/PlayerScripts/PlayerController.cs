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
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintMultiplier;
    [SerializeField] private int jumpMax;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity;

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
    private bool isAutomaticMode;

    private bool isReloading = false;
    private Coroutine reloadCoroutine;

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

        if (Input.GetButtonDown("FireMode"))
        {
            ToggleFireMode();
        }
    }

    void HandleShooting()
    {
        if (gunList.Count == 0) return;
        gunStats currentGun = gunList[gunList.Count - 1];

        if (isReloading) return;

        bool fireInput = isAutomaticMode ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");

        if (fireInput && shootCooldown <= 0f)
        {

            if (currentGun.ammoCur > 0)
            {
                shootCooldown = isAutomaticMode ? currentGun.autoFireRate : currentGun.semiFireRate;
                Shoot();
                currentGun.ammoCur--;

                if (currentGun.ammoCur <= 0)
                {

                    reloadCoroutine = StartCoroutine(ReloadRoutine(currentGun));
                }
            }
            else
            {
                if (reloadCoroutine == null) reloadCoroutine = StartCoroutine(ReloadRoutine(currentGun));
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && currentGun.ammoCur < currentGun.ammoMax && !isReloading)
        {
            reloadCoroutine = StartCoroutine(ReloadRoutine(currentGun));
        }
    }


    void Shoot()
    {
        isShooting = true;

        if (gunList.Count == 0) return;
        gunStats currentGun = gunList[gunList.Count - 1];

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

    public void GetGunStats(gunStats gun)
    {
        gunList.Add(gun);
        shootDamage = gun.shootDamage;
        shootDist = gun.shootDist;
        shootRate = gun.shootRate;

        isAutomaticMode = gun.isAutomaticDefault;

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

    IEnumerator ReloadRoutine(gunStats gun)
    {
        isReloading = true;
        if (gun.reloadSound != null) aud.PlayOneShot(gun.reloadSound, 0.8f);
        yield return new WaitForSeconds(gun.reloadTime);
        gun.ammoCur = gun.ammoMax;
        isReloading = false;
        reloadCoroutine = null;
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

    void ToggleFireMode()
    {
        if (gunList.Count == 0) return;
        gunStats currentGun = gunList[gunList.Count - 1];

        // Only allow switching if enabled
        if (currentGun.canSwitchFireMode)
            isAutomaticMode = !isAutomaticMode;
    }

    public void TakeDamage(int amount)// Waiting on UI implementation
    {
        HP -= amount;
    }
}
