using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damage : MonoBehaviour
{
    [SerializeField] enum damageType { bullet, stationary, melee}
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    // Start is called before the first frame update
    void Start()
    {
        if(type == damageType.bullet)
        {
            rb.linearVelocity = transform.forward * speed;
            //rb.velocity = (gameManager.instance.player.transform.position - transform.position).normalized * speed;
            Destroy(gameObject, destroyTime);
        }
        
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();

        if(dmg != null )
        {
            dmg.takeDamage(damageAmount);
        }

        if(type == damageType.bullet)
        {
            Destroy(gameObject);
        }
    }
}
