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

    EnemyEquipmentPool[] enemyEquipmentPools;

    PickUpItemPool[] pickUpItemPools;

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

        enemyEquipmentPools = GetComponentsInChildren<EnemyEquipmentPool>();
        if (enemyEquipmentPools.Length > 0)
        {
            foreach (var pool in enemyEquipmentPools)
            {
                pool.Initialize();
            }
        }

        pickUpItemPools = GetComponentsInChildren<PickUpItemPool>();
        if (pickUpItemPools.Length > 0)
        {
            foreach (var pool in pickUpItemPools)
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

            // 적이 장비를 장착할 수 있는경우 장비 장착
            IEquipable equipable = enemy as IEquipable;

            if(equipable != null)
            {
                equipable.Equip();
            }
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

    public EnemyEquipment GetRandomEnemyEquipment(Vector3 position)
    {
        EnemyEquipment equipment = null;
        int index = UnityEngine.Random.Range(0, enemyEquipmentPools.Length);

        equipment = enemyEquipmentPools[index].GetObject(position);

        return equipment;
    }

    public PickUpItem GetPickUpItem(Vector3 position, ItemCode code)
    {
        PickUpItemPool itemPool = null;

        // 매개변수로 받은 아이템 코드와 같은 pool 을 찾는다
        foreach (var pool in pickUpItemPools) 
        {
            if(pool.code == code)
            {
                itemPool = pool;
                break;
            }
        }
        
        if(itemPool != null)
        {
            // 같은 코드의 pool 이 있을경우 item 반환
            PickUpItem item = itemPool.GetObject(position);
            return item;
        }
        else
        {
            // 없을경우 null
            Debug.Log("해당 코드의 아이템 풀이 존재하지 않습니다.");
            return null;
        }
    }
}
