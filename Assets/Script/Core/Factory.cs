using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : SingleTon<Factory>
{
    ProjectilePool projectilePool;

    HitEffectPool hitEffectPool;

    protected override void OnInitialize()
    {
        projectilePool = GetComponentInChildren<ProjectilePool>();
        projectilePool.Initialize();

        hitEffectPool = GetComponentInChildren<HitEffectPool>();
        hitEffectPool.Initialize();
    }

    public Projectile GetProjectile(Vector3 position, Vector3 rotation)
    {
        Projectile projectile = projectilePool.GetObject(position, rotation);
        return projectile;
    }

    // hitnormal => Hit 위치의 노말 벡터
    public HitEffect GetHitEffect(Vector3 position, Vector3 hitNormal)
    {
        HitEffect hitEffect = hitEffectPool.GetObject(position); 
        hitEffect.transform.forward = hitNormal;

        return hitEffect;
    }
}
