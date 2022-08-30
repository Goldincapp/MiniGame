using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] private float _cooldown;

     [Header("Projectile")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _projectileSpawnTransform;
    [SerializeField] private float _projectileDamage;
    [SerializeField] private float _projectileSpeed;

    private GameObject _projectileParent;

    private ObjectsPool _objectsPool;

    private SpaceshipController _spaceship;

    public void ActivateWeapon(GameObject projectileParent, SpaceshipController spaceship)
    {
        _projectileParent = projectileParent;
        _spaceship = spaceship;

        _objectsPool = new ObjectsPool(10, _projectilePrefab, _projectileParent);

        StartCoroutine(WaitingForShoot());
    }

    public void DeactivateWeapon()
    {
        foreach (var projectile in _projectileSpawnTransform.GetComponentsInChildren<ProjectileController>())
            Destroy(projectile.gameObject);

        StopAllCoroutines();
    }

    private void OnDisable() => DeactivateWeapon();

    private IEnumerator WaitingForShoot()
    {
        while(true)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Shoot();
                yield return new WaitForSeconds(_cooldown);
            }

            yield return null;
        }
    }

    private void Shoot()
    {
        var projectile = _objectsPool.GetObject();
        
        projectile.GetComponent<ProjectileController>()?.ActivateProjectile(_projectileDamage, _projectileSpeed, _spaceship.Direction,
        _projectileSpawnTransform.position, _spaceship.transform.rotation, delegate(GameObject projectile) { _objectsPool.AddObject(projectile); });

        SoundManager.Instance.PlayLaserSound();
    }
}
