using RPG.Attributes;
using RPG.Combat;
using UnityEngine;

[CreateAssetMenu(fileName = "New weapon", menuName = "Weapons/Create new weapon", order = 0)]
public class Weapon : ScriptableObject
{
    [SerializeField] GameObject weaponPrefab = null;
    [SerializeField] AnimatorOverrideController weaponOverrideController = null;

    [SerializeField] float weaponRange = 2f;
    [SerializeField] int weaponDamage = 5;
    [SerializeField] bool isRightHand = true;

    [SerializeField] Projectile projectile = null;

    const string weaponName = "Weapon";
    public void SpawnWeapon(Transform rightHandWeaponPosition, Transform leftHandWeaponPosition, Animator animator)
    {
        DestroyPrevWeapon(rightHandWeaponPosition, leftHandWeaponPosition);
        if (weaponPrefab != null)
        {
            Transform handPosition = GetHandPosition(rightHandWeaponPosition, leftHandWeaponPosition);

            GameObject weapon = Instantiate(weaponPrefab, handPosition);
            weapon.name = weaponName;
        }
        var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
        if (weaponOverrideController != null)
        {
            animator.runtimeAnimatorController = weaponOverrideController;
        }
        else if (overrideController != null)
        {
            animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
        }
    }

    private void DestroyPrevWeapon(Transform rightHand, Transform leftHand)
    {
        Transform prevWeapon = rightHand.Find(weaponName);
        if (prevWeapon == null)
        {
            prevWeapon = leftHand.Find(weaponName);
        }
        if (prevWeapon == null)
        {
            return;
        }
        prevWeapon.name = "destroyMe";
        Destroy(prevWeapon.gameObject);
    }

    Transform GetHandPosition(Transform rightHandWeaponPosition, Transform leftHandWeaponPosition)
    {
        Transform handPosition;
        if (isRightHand) handPosition = rightHandWeaponPosition;
        else handPosition = leftHandWeaponPosition;
        return handPosition;
    }

    public float GetWeaponRange()
    {
        return weaponRange;
    }
    public int GetWeaponDamage()
    {
        return weaponDamage;
    }

    public bool HasProjectile()
    {
        return projectile != null;
    }

    public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, int calculatedDamage)
    {
        Projectile projectileInst = Instantiate(projectile, GetHandPosition(rightHand, leftHand).position, Quaternion.identity);
        projectileInst.SetTarget(target, instigator, calculatedDamage);
    }
}
