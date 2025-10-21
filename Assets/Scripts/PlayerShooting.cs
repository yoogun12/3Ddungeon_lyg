using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject projectilePrefab; // Projectile ������

    public Transform firePoint;         // �߻� ��ġ (�ѱ�)

    Camera cam;
    void Start()
    {
        cam = Camera.main;  //���� ī�޶� ��������
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) // ��Ŭ�� �߻�
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // ȭ�鿡�� ���콺 -> ����(Ray) ���
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPoint;
        targetPoint = ray.GetPoint(50f);
        Vector3 direction = (targetPoint - firePoint.position).normalized;  //���� ����

        // Projectile ����
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));
    }
}
