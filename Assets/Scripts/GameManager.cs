using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private int enemyKillCount = 0;

    [Header("���� ����")]
    public GameObject keyPrefab;
    public Transform keySpawnPoint;
    public int requiredKills = 20;
    private bool keySpawned = false;

    [Header("UI")]
    public TextMeshProUGUI goblinKillText; // �����Ϳ��� ���� ����

    void Start()
    {
        UpdateKillText(); // �� ���� �� UI ����
    }

    public void AddKill()
    {
        enemyKillCount++;
        Debug.Log($"���� óġ ��: {enemyKillCount}");
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
            Debug.Log("���谡 �����Ǿ����ϴ�!");
            StopAllSpawners();
        }
    }

    void StopAllSpawners()
    {
        EnemySpawner[] spawners = FindObjectsOfType<EnemySpawner>();
        foreach (var spawner in spawners)
            spawner.enabled = false;

        Debug.Log($"�� {spawners.Length}���� �����ʰ� �����Ǿ����ϴ�.");
    }

    void UpdateKillText()
    {
        if (goblinKillText != null)
        {
            goblinKillText.text =
                $"óġ�ؾ� �ϴ� ��� �� : {requiredKills}\n���� óġ�� ��� �� : {enemyKillCount}";
        }
    }

    public void GoToNextScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}