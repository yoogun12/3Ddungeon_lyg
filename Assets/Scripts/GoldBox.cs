using UnityEngine;
using UnityEngine.SceneManagement;

public class GoldBoxTrigger : MonoBehaviour
{
    public string nextSceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어와 충돌 시
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}