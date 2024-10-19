using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : SingleTon<Factory>
{
    ProjectilePool projectilePool;

    HitEffectPool hitEffectPool;

    HitEffectPool flashHitEffectPool;

    ZombiePool zombiePool;

    protected override void OnInitialize()
    {
        projectilePool = GetComponentInChildren<ProjectilePool>();
        projectilePool?.Initialize();

        Transform child = transform.GetChild(1);
        hitEffectPool = child.GetComponent<HitEffectPool>();
        hitEffectPool?.Initialize();

        child = transform.GetChild(2);
        flashHitEffectPool = child.GetComponent<HitEffectPool>();
        flashHitEffectPool?.Initialize();

        zombiePool = GetComponentInChildren<ZombiePool>();
        zombiePool?.Initialize();
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

    public HitEffect GetFlashHitEffect(Vector3 position, Vector3 hitNormal)
    {
        HitEffect hitEffect = flashHitEffectPool.GetObject(position);
        hitEffect.transform.forward = hitNormal;

        return hitEffect;
    }

    public Zombie GetZombie(Vector3 position)
    {
        Zombie zombie = zombiePool.GetObject(position);
        return zombie;
    }
}
