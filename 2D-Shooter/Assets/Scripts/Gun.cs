using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunType
{
    Static,
    FollowPointer,
}
public class Gun : MonoBehaviour
{
    private PlayerCombatController combatController;

    [SerializeField]
    private GameObject bulletPrefab = null;
    [SerializeField]
    private GameObject shotSparksEffect = null;
    [SerializeField]
    private Transform bulletSpawnPoint = null;

    [SerializeField]
    private float shotCoolDownTime = 1f;

    private float _shotCoolDownTime;

    private bool canShoot;

    public GunType gunType = GunType.FollowPointer;
    // Start is called before the first frame update
    void Start()
    {
        combatController = GetComponent<PlayerCombatController>();
        _shotCoolDownTime = shotCoolDownTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(gunType == GunType.FollowPointer)
            LookAtPointer(transform);

        
    }
    private void FixedUpdate()
    {
        HandleShotCooldown();
    }
    private void LookAtPointer(Transform transform)
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        var distance = mousePos - transform.position;

        float angle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    private void Shoot()
    {
        if (canShoot)
        {
            var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            var sparks = Instantiate(shotSparksEffect, bulletSpawnPoint.position, Quaternion.identity);
            LookAtPointer(sparks.transform);
            var bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.SetVariables(combatController, bulletSpawnPoint.right);
            canShoot = false;
        }
    }
    private void HandleShotCooldown()
    {
        if (!canShoot)
        {
            if (_shotCoolDownTime <= 0)
            {
                canShoot = true;
                _shotCoolDownTime = shotCoolDownTime;
            }
            else
            {
                _shotCoolDownTime -= Time.deltaTime;
            }
        }
    }
}
