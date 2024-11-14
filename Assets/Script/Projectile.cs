using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : RecycleObject
{
    [SerializeField]
    float force = 5.0f;

    [SerializeField]
    float duration = 2.0f;

    [SerializeField]
    [Range(0f, 1f)]
    float drag = 1.0f;

    [SerializeField]
    float radius = 5.0f;

    [SerializeField]
    float damage = 100.0f;

    Rigidbody rb;

    float elapsedTime = 0.0f;

    bool isCollided = false;

    int Enemy_LayerNumber;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Enemy_LayerNumber = LayerMask.GetMask("Enemy");
    }

    protected override void OnReset()
    {
        // 물리작용 초기화
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 앞으로 발사하는 힘
        rb.AddForce(Camera.main.transform.forward * force, ForceMode.Impulse);

        // 랜덤회전값
        Vector2 randomForce = Random.insideUnitSphere * 3.0f;
        rb.AddTorque(new Vector3(randomForce.x, randomForce.y, 0f));

        isCollided = false;
        elapsedTime = 0.0f;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (isCollided)
        {
            Vector3 nextVelocity = rb.velocity * drag;
            // rb.velocity = Vector3.Max(Vector3.zero, nextVelocity);
            rb.velocity = nextVelocity;

            rb.angularVelocity = Vector3.right * rb.angularVelocity.x;
        }

        if (elapsedTime > duration)
        {
            Explosion();
        }
    }

    // 폭발 시 실행될 함수
    private void Explosion()
    {
        // 폭발 이펙트 생성
        Factory.Instance.GetExplosionEffect(transform.position);

        // 폭발 데미지 적용
        Collider[] collders = Physics.OverlapSphere(transform.position, radius, Enemy_LayerNumber);
        if (collders.Length > 0)
        {
            foreach (Collider collider in collders)
            {
                Health health = collider.GetComponentInParent<Health>();
                Debug.Log(health.gameObject.name);
                health?.OnDamage(damage);
            }
        }


        // 이 게임오브젝트 비활성화
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        isCollided = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isCollided = false;
    }
}
