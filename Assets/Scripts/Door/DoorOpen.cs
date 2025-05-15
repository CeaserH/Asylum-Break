using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    private Animator anim;
    private AudioSource audioSource;
    private bool isDoorOpen = false;
    private bool playerInTrigger = false;

    [SerializeField] private AudioClip doorOpen;
    [Range(0, 1)][SerializeField] private float audioDoorOpenVol = 0.9f;
    [SerializeField] private AudioClip doorClose;
    [Range(0, 1)][SerializeField] private float audioDoorCloseVol = 0.9f;

    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (playerInTrigger && Input.GetButtonDown("Interact"))
        {
            isDoorOpen = !isDoorOpen;
            anim.SetBool("Open", isDoorOpen);

            if (isDoorOpen)
            {
                if (doorOpen != null && audioSource != null)
                    audioSource.PlayOneShot(doorOpen, audioDoorOpenVol);
            }
            else
            {
                if (doorClose != null && audioSource != null)
                    audioSource.PlayOneShot(doorClose, audioDoorCloseVol);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }
}
