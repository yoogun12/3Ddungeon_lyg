using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private int enemyKillCount = 0;

    [Header("열쇠 설정")]
    public GameObject keyPrefab;
    public Transform keySpawnPoint;
    public int requiredKills = 20;
    private bool keySpawned = false;

    [Header("UI")]
    public TextMeshProUGUI goblinKillText; // 에디터에서 직접 연결

    void Start()
    {
        UpdateKillText(); // 씬 시작 시 UI 갱신
    }

    public void AddKill()
    {
        enemyKillCount++;
        Debug.Log($"현재 처치 수: {enemyKillCount}");
        UpdateKillText();

        if (enemyKillCount >= requiredKills && !keySpawned)
        {
            SpawnKey();
        }
    }

    void SpawnKey()
    {
        if (keyPrefab != null && keySpawnPoint != null)
        {
            Instantiate(keyPrefab, keySpawnPoint.position, Quaternion.identity);
            keySpawned = true;
            Debug.Log("열쇠가 생성되었습니다!");
            StopAllSpawners();
        }
    }

    void StopAllSpawners()
    {
        EnemySpawner[] spawners = FindObjectsOfType<EnemySpawner>();
        foreach (var spawner in spawners)
            spawner.enabled = false;

        Debug.Log($"총 {spawners.Length}개의 스포너가 중지되었습니다.");
    }

    void UpdateKillText()
    {
        if (goblinKillText != null)
        {
            goblinKillText.text =
                $"처치해야 하는 고블린 수 : {requiredKills}\n현재 처치한 고블린 수 : {enemyKillCount}";
        }
    }

    public void GoToNextScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}