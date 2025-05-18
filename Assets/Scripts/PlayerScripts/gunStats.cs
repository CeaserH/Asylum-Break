using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public GameObject gunModel;
    [Range(1, 10)] public int shootDamage;
    [Range(0.1f, 10)] public float shootRate;
    [Range(15, 1000)] public int shootDist;
    [Range(3, 100)] public int ammoCur, ammoMax;
    public float reloadTime = 2.0f;
    public AudioClip reloadSound;
    public float autoFireRate = 0.08f;
    public float semiFireRate = 0.3f;

    public GameObject bulletHolePrefab;
    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    [Range(0, 1)] public float shootVol;
    public bool canSwitchFireMode;
    public bool isAutomaticDefault;
}
