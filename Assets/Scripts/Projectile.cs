using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;       //�̵� �ӵ�

    public float lifeTime = 2f;     // ���� �ð� (��)

    public int damage = 1;

    void Start()
    {
        //���� �ð� �� �ڵ� ���� (�޸� ����)
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        // �Ǥ����� forward ����(��)���� �̵�
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            FireEnemy fireEnemy = other.GetComponent<FireEnemy>();
            BossEnemy bossEnemy = other.GetComponent<BossEnemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
              
            }
            if(fireEnemy != null)
            {
                fireEnemy.TakeDamage(damage);
            }

            if(bossEnemy != null)
            {
                bossEnemy.TakeDamage(damage);
            }

            // Projectile ����
            Destroy(gameObject);
        }
    }
}
