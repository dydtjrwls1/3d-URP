using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitHandler : MonoBehaviour
{
    public Transform testSphere;

    CinemachineVirtualCamera m_PlayerCamera;

    PlayerMovementContoller Player { get; set; }

    int enemyLayerMask; 
    int groundLayerMask; 
    int WallLayerMask;

    const float HitOffset = 0.01f; // ��Ʈ ������ ������

    private void Awake()
    {
        m_PlayerCamera = GetComponentInChildren<CinemachineVirtualCamera>();

        enemyLayerMask = 1 << LayerMask.NameToLayer("Enemy");
        groundLayerMask = 1 << LayerMask.NameToLayer("Ground");
        WallLayerMask = 1 << LayerMask.NameToLayer("Wall");
    }

    private void Start()
    {
        Player = GameManager.Instance.Player;

        Player.onBulletFire += HitHandler;
    }

    private void HitHandler(Weapon weapon)
    {
        RaycastHit hit;
        if (Physics.Raycast(
            m_PlayerCamera.transform.position,
            m_PlayerCamera.transform.forward,
            out hit,
            1000f,
            enemyLayerMask | groundLayerMask | WallLayerMask))
        {
            int hitLayerNum = 1 << hit.collider.gameObject.layer;
            if (hitLayerNum == enemyLayerMask)
            {
                // �ѿ� ���� ����� ���� ��� ����Ʈ ���� �� ����� ü���� ��´�
                Factory.Instance.GetFlashHitEffect(hit.point + hit.normal * HitOffset, hit.normal);

                Health health = hit.collider.GetComponentInParent<Health>();

                // �� ü�� ���ⵥ���� ��ŭ ����
                if (health != null)
                {
                    health.OnDamage(Player.CurrentWeapon.defaultDamage);
                }

            }
            else
            {
                Factory.Instance.GetHitEffect(hit.point + hit.normal * HitOffset, hit.normal);
            }
        }
    }
}
