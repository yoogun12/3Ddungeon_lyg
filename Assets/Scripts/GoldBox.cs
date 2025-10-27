using UnityEngine;
using UnityEngine.SceneManagement;

public class GoldBoxTrigger : MonoBehaviour
{
    public string nextSceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // �÷��̾�� �浹 ��
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}