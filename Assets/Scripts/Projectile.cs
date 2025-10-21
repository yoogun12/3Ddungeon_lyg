using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;       //�̵� �ӵ�

    public float lifeTime = 2f;     // ���� �ð� (��)

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
        if(other.CompareTag("Enemy"))
        {
            // �� �浹 �� �� ����
            Destroy(other.gameObject);
            // Projectile ����
            Destroy(gameObject);
        }
    }
}
