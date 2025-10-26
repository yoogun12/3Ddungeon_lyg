using UnityEngine;

public class Key : MonoBehaviour
{
    public string nextSceneName = "NextScene"; // 이동할 씬 이름

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어가 열쇠를 획득했습니다!");

            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.GoToNextScene(nextSceneName);
            }

            Destroy(gameObject);
        }
    }
}