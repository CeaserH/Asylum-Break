using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerWeaponManager weaponManager;
    [SerializeField] private BreathingSystem gunBreathingSim;
    [SerializeField] private BreathingSystem playerBreathingSim;
    [SerializeField] private GameObject armsModel;

    void Update()
    {
        playerMovement.HandleMove();
        playerMovement.HandleSprint();
        playerMovement.HandleLanding();
        weaponManager.HandleShooting();
        playerHealth.UpdateOriginalHP();

        if (gunBreathingSim != null) gunBreathingSim.HandleBreathing();
        if (playerBreathingSim != null) playerBreathingSim.HandleBreathing();

        if (armsModel != null)
        {
            armsModel.SetActive(weaponManager.HasGun());
        }

        if (Input.GetButtonDown("FireMode"))
        {
            weaponManager.ToggleFireMode();
        }

        weaponManager.SetAiming(Input.GetButton("Fire2"));
        weaponManager.HandleADS();
    }
}
