using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : SingleTon<Factory>
{
    ProjectilePool projectilePool;

    HitEffectPool hitEffectPool;

    HitEffectPool flashHitEffectPool;

    protected override void OnInitialize()
    {
        projectilePool = GetComponentInChildren<ProjectilePool>();
        projectilePool.Initialize();

        Transform child = transform.GetChild(1);
        hitEffectPool = child.GetComponent<HitEffectPool>();
        hitEffectPool.Initialize();

        child = transform.GetChild(2);
        flashHitEffectPool = child.GetComponent<HitEffectPool>();
        flashHitEffectPool.Initialize();
    }

    public Projectile GetProjectile(Vector3 position, Vector3 rotation)
    {
        Projectile projectile = projectilePool.GetObject(position, rotation);
        return projectile;
    }

    // hitnormal => Hit ��ġ�� �븻 ����
    public HitEffect GetHitEffect(Vector3 position, Vector3 hitNormal)
    {
        HitEffect hitEffect = hitEffectPool.GetObject(position); 
        hitEffect.transform.forward = hitNormal;

        return hitEffect;
    }

    public HitEffect GetFlashHitEffect(Vector3 position, Vector3 hitNormal)
    {
        HitEffect hitEffect = flashHitEffectPool.GetObject(position);
        hitEffect.transform.forward = hitNormal;

        return hitEffect;
    }
}
