using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int HP;
    public int HPOrig;
    public int XP;
    public AudioSource aud;
    public AudioClip audioHurt;
    [Range(0, 1)] public float hurtVol = 0.9f;

    public void TakeDamage(int amount)
    {
        HP -= amount;

        if (audioHurt != null)
            aud.PlayOneShot(audioHurt, hurtVol);
    }

    public void UpdateOriginalHP()
    {
        HPOrig = HP;
    }
}
