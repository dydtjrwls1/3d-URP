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
        // �����ۿ� �ʱ�ȭ
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // ������ �߻��ϴ� ��
        rb.AddForce(Camera.main.transform.forward * force, ForceMode.Impulse);

        // ����ȸ����
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

    // ���� �� ����� �Լ�
    private void Explosion()
    {
        // ���� ����Ʈ ����
        Factory.Instance.GetExplosionEffect(transform.position);

        // ���� ������ ����
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


        // �� ���ӿ�����Ʈ ��Ȱ��ȭ
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
