using System;
using Unity.Mathematics;
using UnityEngine;

public class Factory : SingleTon<Factory>
{
    ProjectilePool projectilePool;

    ParticleEffectPool hitEffectPool;

    ParticleEffectPool flashHitEffectPool;

    ParticleEffectPool explosionEffectPool;

    EnemyPool[] enemyPools;

    EnemyWeaponPool[] enemyWeaponPools;

    EnemyEquipmentPool[] enemyEquipmentPools;

    PickUpItemPool[] pickUpItemPools;

    

    protected override void OnInitialize()
    {
        projectilePool = GetComponentInChildren<ProjectilePool>();
        projectilePool?.Initialize();

        Transform child = transform.GetChild(0);
        hitEffectPool = child.GetComponent<ParticleEffectPool>();
        hitEffectPool?.Initialize();

        child = transform.GetChild(1);
        flashHitEffectPool = child.GetComponent<ParticleEffectPool>();
        flashHitEffectPool?.Initialize();

        enemyWeaponPools = GetComponentsInChildren<EnemyWeaponPool>();
        if (enemyWeaponPools.Length > 0)
        {
            foreach (var pool in enemyWeaponPools)
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

        enemyPools = GetComponentsInChildren<EnemyPool>();
        if (enemyPools.Length > 0)
        {
            foreach (var pool in enemyPools)
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

        child = transform.GetChild(7);
        explosionEffectPool = child.GetComponent<ParticleEffectPool>();
        explosionEffectPool?.Initialize();
    }

    public Projectile GetProjectile(Vector3 position)
    {
        Projectile projectile = projectilePool.GetObject(position);
        return projectile;
    }

    // hitnormal => Hit ��ġ�� �븻 ����
    public ParticleEffect GetHitEffect(Vector3 position, Vector3 hitNormal)
    {
        ParticleEffect hitEffect = hitEffectPool.GetObject(position); 
        hitEffect.transform.forward = hitNormal;

        return hitEffect;
    }

    public ParticleEffect GetFlashHitEffect(Vector3 position, Vector3 hitNormal)
    {
        ParticleEffect hitEffect = flashHitEffectPool.GetObject(position);
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

            // ���� ��� ������ �� �ִ°�� ��� ����
            //IEquipable equipable = enemy as IEquipable;

            //if(equipable != null)
            //{
            //    equipable.Equip();
            //}
        }
        
        return enemy;
    }

    // Ǯ���ִ� ���� ���� �� �ƹ��ų� �������� �����´�.
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

        // �Ű������� ���� ������ �ڵ�� ���� pool �� ã�´�
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
            // ���� �ڵ��� pool �� ������� item ��ȯ
            PickUpItem item = itemPool.GetObject(position);
            return item;
        }
        else
        {
            // ������� null
            Debug.Log("�ش� �ڵ��� ������ Ǯ�� �������� �ʽ��ϴ�.");
            return null;
        }
    }

    public PickUpItem GetPickUpItem(Vector3 position, int code)
    {
        PickUpItem item = null;
        item = GetPickUpItem(position, (ItemCode)code);
        return item;
    }

    public ParticleEffect GetExplosionEffect(Vector3 position)
    {
        ParticleEffect hitEffect = explosionEffectPool.GetObject(position);

        return hitEffect;
    }
}
