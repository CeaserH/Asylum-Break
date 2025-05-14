using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
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

    [Header("Shooting Settings")]
    [SerializeField] private int shootDamage = 10;
    [SerializeField] private float shootRate = 0.25f;
    [SerializeField] private int shootDist = 100;

    [Header("Audio")]
    [SerializeField] private AudioClip[] audioSteps;
    [Range(0, 1)][SerializeField] private float audioStepsVol = 0.9f;
    [SerializeField] private AudioClip[] audioJump;
    [Range(0, 1)][SerializeField] private float audioJumpVol = 0.9f;
    [SerializeField] private AudioClip[] audioLand;
    [Range(0, 1)][SerializeField] private float audioLandVol = 0.9f;

    private float stepTimer;
    private int jumpCount;
    private Vector3 moveDir;
    private Vector3 playerVel;
    private bool isSprinting;
    private bool isJumped;
    private bool isPlayingStep;
    private bool wasGrounded;

    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
        Move();
        HandleSprint();
        HandleLanding();
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
        {
            aud.PlayOneShot(audioLand[Random.Range(0, audioLand.Length)], audioLandVol);
            isJumped = false;
        }
        wasGrounded = controller.isGrounded;
    }
}
