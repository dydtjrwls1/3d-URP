using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : SingleTon<Factory>
{
    ProjectilePool projectilePool;

    protected override void OnInitialize()
    {
        projectilePool = GetComponentInChildren<ProjectilePool>();
        projectilePool.Initialize();
    }

    public Projectile GetProjectile(Vector3 position, Vector3 rotation)
    {
        Projectile projectile = projectilePool.GetObject(position, rotation);
        return projectile;
    }
}
