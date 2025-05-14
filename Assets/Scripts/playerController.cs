using System.Collections;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header("----Components----")]
    [SerializeField] CharacterController controller;

    [Header("----Stats----")]
    [Range(1, 10)][SerializeField] int HP = 10;
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpSpeed = 10f;
    [SerializeField] float gravity = 20f;

    Vector3 moveDir;
    Vector3 playerVel;
    int jumpCount;

    void Start()
    {
        // optional: lock cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Movement();
    }

    void Movement()
    {
        if (controller.isGrounded)
        {
            playerVel = Vector3.zero;
            jumpCount = 0;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        moveDir = transform.right * h + transform.forward * v;
        controller.Move(moveDir * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && jumpCount < 1)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }

        playerVel.y -= gravity * Time.deltaTime;
        controller.Move(playerVel * Time.deltaTime);
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        Debug.Log("Player took damage. HP: " + HP);

        if (HP <= 0)
        {
            Debug.Log("Player died!");
            // You can disable controls or respawn here
        }
    }
}