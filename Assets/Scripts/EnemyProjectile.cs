using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{

    public int damage = 2;

    public float speed = 8f;

    public float lifeTime = 3f;

    private Vector3 moveDir;

    public void SetDirection(Vector3 dir)
    {
        moveDir = dir.normalized;

        // Rigidbody를 가져와서 중력 끄고 속도 적용
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.velocity = moveDir * speed;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,lifeTime);   
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += moveDir * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if(pc != null) pc.TakeDamage(damage);

            Destroy(gameObject);
        }
    }    
}
