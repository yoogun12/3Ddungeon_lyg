using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject projectilePrefab; // Projectile 프리팹

    public Transform firePoint;         // 발사 위치 (총구)

    Camera cam;
    void Start()
    {
        cam = Camera.main;  //메인 카메라 가져오기
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) // 좌클릭 발사
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // 화면에서 마우스 -> 광선(Ray) 쏘기
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPoint;
        targetPoint = ray.GetPoint(50f);
        Vector3 direction = (targetPoint - firePoint.position).normalized;  //방향 벡터

        // Projectile 생성
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));
    }
}
