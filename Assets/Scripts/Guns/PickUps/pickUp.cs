using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup : MonoBehaviour
{
    [SerializeField] gunStats gun;

    private void Start()
    {
        gun.ammoCur = gun.ammoMax;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.playerScript.GetGunStats(gun);
            Destroy(gameObject);
        }
    }
}
