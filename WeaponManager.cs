using UnityEngine;
using UnityEngine.Events;

public class WeaponManager : MonoBehaviour
{
    public UnityEvent onWeaponFire;
    public float weaponCooldown;
    public bool automatic;
    private float currentCooldown;
    private void Start()
    {
        currentCooldown = weaponCooldown;
    }
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (currentCooldown <= 0f)
            {
                onWeaponFire?.Invoke();
                currentCooldown = weaponCooldown;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (currentCooldown <= 0f)
                {
                    onWeaponFire?.Invoke();
                    currentCooldown = weaponCooldown;
                }
            }   
        }
        currentCooldown -= Time.deltaTime; 
    }
}