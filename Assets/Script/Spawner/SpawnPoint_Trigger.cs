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
        // ������ �迭�� �ϳ��� �����ϰ� ����
        int randIndex = Random.Range(0, spawnItems.Length);
        m_Item = Factory.Instance.GetPickUpItem(transform.position, spawnItems[randIndex]);


        // ���� �Ұ����� ���·� ��ȯ
        m_CanSpawn = false;

        // �������� ��Ȱ��ȭ (�÷��̾ �Ծ��� ��) ���������� ���·� ��ȯ�Ѵ�.
        m_Item.onDisable += () => { m_CanSpawn = true; };
    }
}
