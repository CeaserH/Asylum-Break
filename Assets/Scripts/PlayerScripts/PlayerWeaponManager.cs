using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerWeaponManager : MonoBehaviour
{
    public List<gunStats> gunList = new List<gunStats>();
    public AudioSource aud;
    public GameObject muzzleFlashPrefab;
    public float muzzleFlashTime = 0.05f;
    public LayerMask ignoreLayer;
    public GameObject gunModel;
    public GameObject armsModel;
    public float adsSpeed;
    public GameObject tracerPrefab;

    Transform currentHipPosition;
    Transform currentAdsPosition;
    bool isAutomaticMode;
    bool isReloading;
    Coroutine reloadCoroutine;
    float shootCooldown = 0f;
    bool isAiming;

    public void HandleShooting()
    {
        shootCooldown -= Time.deltaTime;
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
                    reloadCoroutine = StartCoroutine(ReloadRoutine(currentGun));
            }
            else
            {
                if (reloadCoroutine == null) reloadCoroutine = StartCoroutine(ReloadRoutine(currentGun));
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && currentGun.ammoCur < currentGun.ammoMax && !isReloading)
            reloadCoroutine = StartCoroutine(ReloadRoutine(currentGun));
    }

    void Shoot()
    {
        if (gunList.Count == 0) return;
        gunStats currentGun = gunList[gunList.Count - 1];

        if (currentGun.shootSound != null && currentGun.shootSound.Length > 0)
            aud.PlayOneShot(currentGun.shootSound[Random.Range(0, currentGun.shootSound.Length)], currentGun.shootVol);

        Vector3 origin = Camera.main.transform.position;
        Vector3 dir = Camera.main.transform.forward;

        if (muzzleFlashPrefab != null)
            StartCoroutine(MuzzleFlashRoutine());

        Debug.DrawRay(origin, dir * currentGun.shootDist, Color.red, 1f);

        Ray ray = new Ray(origin, dir);
        if (Physics.Raycast(ray, out RaycastHit hit, currentGun.shootDist, ~ignoreLayer))
        {
            if (currentGun.hitEffect != null)
            {
                var i = Instantiate(currentGun.hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(i, 2f);
            }

            if (currentGun.bulletHolePrefab != null)
            {
                var bulletHole = Instantiate(
                    currentGun.bulletHolePrefab,
                    hit.point + hit.normal * 0.01f,
                    Quaternion.LookRotation(hit.normal)
                );
                bulletHole.transform.SetParent(hit.transform);
                Destroy(bulletHole, 10f);
            }

            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
                dmg.takeDamage(currentGun.shootDamage);
        }
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

    public void GetGunStats(gunStats gun)
    {
        gunList.Add(gun);
        isAutomaticMode = gun.isAutomaticDefault;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;

        currentHipPosition = gunModel.transform.Find("HipPosition");
        currentAdsPosition = gunModel.transform.Find("ADSPosition");

        if (armsModel != null)
            armsModel.SetActive(true);
    }

    public void SetAiming(bool aim)
    {
        isAiming = aim;
    }

    public void HandleADS()
    {
        if (currentHipPosition == null || currentAdsPosition == null)
            return;

        Transform target = isAiming ? currentAdsPosition : currentHipPosition;

        gunModel.transform.localPosition = Vector3.Lerp(
            gunModel.transform.localPosition,
            target.localPosition,
            Time.deltaTime * adsSpeed
        );
        gunModel.transform.localRotation = Quaternion.Slerp(
            gunModel.transform.localRotation,
            target.localRotation,
            Time.deltaTime * adsSpeed
        );
    }

    public void ToggleFireMode()
    {
        if (gunList.Count == 0) return;
        gunStats currentGun = gunList[gunList.Count - 1];

        if (currentGun.canSwitchFireMode)
            isAutomaticMode = !isAutomaticMode;
    }

    public bool HasGun()
    {
        return gunList.Count > 0;
    }

    IEnumerator MuzzleFlashRoutine()
    {
        muzzleFlashPrefab.SetActive(true);
        yield return new WaitForSeconds(muzzleFlashTime);
        muzzleFlashPrefab.SetActive(false);
    }
}
