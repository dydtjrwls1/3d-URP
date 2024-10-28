using UnityEngine;

public class EnemyAttack : MonoBehaviour, IInitialize
{
    [SerializeField]
    float damage = 10.0f;

    Collider hitBox;

    private void Awake()
    {
        Transform child = transform.GetChild(3);
        hitBox = child.GetComponent<SphereCollider>();
    }

    private void Start()
    {
        EnemyHitBox hitBox = GetComponentInChildren<EnemyHitBox>();
        hitBox.onHit += (health) => 
        { 
            health.OnDamage(damage);
            Debug.Log(damage);
        };
    }

    public void EnableHitBox()
    {
        hitBox.enabled = true;
    }

    public void DisalbeHitBox()
    {
        hitBox.enabled = false;
    }

    public void Initialize()
    {
        if (hitBox == null)
        {
            Transform child = transform.GetChild(3);
            hitBox = child.GetComponent<SphereCollider>();
        }
        hitBox.enabled = false;
    }
}
