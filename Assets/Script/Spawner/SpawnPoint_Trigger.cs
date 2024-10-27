using UnityEngine;

public class SpawnPoint_Trigger : SpawnPoint_Base
{
    public ItemCode[] spawnItems;

    PickUpItem m_Item = null;

    protected override void Awake()
    {
        base.Awake();
        m_CanSpawn = true;
    }

    protected override void Spawn()
    {
        // 아이템 배열중 하나를 랜덤하게 선택
        int randIndex = Random.Range(0, spawnItems.Length);
        m_Item = Factory.Instance.GetPickUpItem(transform.position, spawnItems[randIndex]);


        // 스폰 불가능한 상태로 전환
        m_CanSpawn = false;

        // 아이템이 비활성화 (플레이어가 먹었을 때) 스폰가능한 상태로 전환한다.
        m_Item.onDisable += () => { m_CanSpawn = true; };
    }
}
