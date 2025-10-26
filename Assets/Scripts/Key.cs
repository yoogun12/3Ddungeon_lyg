using UnityEngine;

public class Key : MonoBehaviour
{
    public string nextSceneName = "NextScene"; // �̵��� �� �̸�

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("�÷��̾ ���踦 ȹ���߽��ϴ�!");

            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.GoToNextScene(nextSceneName);
            }

            Destroy(gameObject);
        }
    }
}