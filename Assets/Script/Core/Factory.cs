using System;
using Unity.Mathematics;
using UnityEngine;

public class Factory : SingleTon<Factory>
{
    ProjectilePool projectilePool;

    HitEffectPool hitEffectPool;

    HitEffectPool flashHitEffectPool;

    EnemyPool[] enemyPools;

    EnemyWeaponPool[] enemyWeaponPools;

    protected override void OnInitialize()
    {
        //projectilePool = GetComponentInChildren<ProjectilePool>();
        //projectilePool?.Initialize();

        Transform child = transform.GetChild(0);
        hitEffectPool = child.GetComponent<HitEffectPool>();
        hitEffectPool?.Initialize();

        child = transform.GetChild(1);
        flashHitEffectPool = child.GetComponent<HitEffectPool>();
        flashHitEffectPool?.Initialize();

        enemyWeaponPools = GetComponentsInChildren<EnemyWeaponPool>();
        if (enemyWeaponPools.Length > 0)
        {
            foreach (var pool in enemyWeaponPools)
            {
                pool.Initialize();
            }
        }

        enemyPools = GetComponentsInChildren<EnemyPool>();
        if (enemyPools.Length > 0)
        {
            foreach (var pool in enemyPools)
            {
                pool.Initialize();
            }
        }
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

    public EnemyBase GetEnemy(Vector3 position, EnemyType type)
    {
        EnemyBase enemy = null;
        int index = (int)type;

        if(index < enemyPools.Length)
        {
            enemy = enemyPools[index].GetObject(position);
        }
        
        return enemy;
    }

    // 풀에있는 무기 종류 중 아무거나 랜덤으로 가져온다.
    public EnemyWeapon GetRandomEnemyWeapon(Vector3 position) 
    {
        EnemyWeapon weapon = null;
        int index = UnityEngine.Random.Range(0, enemyWeaponPools.Length);

        weapon = enemyWeaponPools[index].GetObject(position);

        return weapon;
    }
}
