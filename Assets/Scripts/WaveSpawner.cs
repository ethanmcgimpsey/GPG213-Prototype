using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Xml.Serialization;
using System.IO;

public class WaveSpawner : MonoBehaviour
{
    public enum SpawnState
    {
        Spawning,
        Waiting,
        Counting
    };

    [System.Serializable]
    public class Wave
    {
        public string name;
        public int index;
        [XmlIgnore] public Transform enemy;
        public int count;
        public float rate;
    }

    public Wave[] waves;
    public int nextWave = 0;
    public Transform waypointParent;
    public GameObject completeLevelUI, finalWaveUI;
    public Text waveTextRound;
    // public Text score;
    public AudioSource nextWaveAudio;
    public AudioSource gameOverAudio;
    public float timeBetweenWaves = 5f;
    public float waveCountdown;
    public SpawnState state = SpawnState.Counting;

    [Header("Saving")]
    public string fileName = "GameSave";

    private string fullPath;

    private float searchCountdown = 1f;


    void Start()
    {
        /* fullPath = Application.persistentDataPath + "/" + fileName + ".xml";

        if(File.Exists(fullPath))
        {
            Wave currentWave = Load(fullPath);
            SetWave(currentWave.index);
        }*/

        Enemy enemy = GetComponent<Enemy>();
        waveCountdown = timeBetweenWaves;
        finalWaveUI.SetActive(false);
    }

    void SetWave(int waveIndex)
    {
        nextWave = waveIndex;
        nextWaveAudio.Play();
        state = SpawnState.Counting;
        waveCountdown = timeBetweenWaves;
        waveTextRound.text = "Wave: " + (nextWave + 1).ToString();
    }

    void Update()
    {
        // score.text = PlayerStats.Score.ToString() + " :Points";
        if (state == SpawnState.Waiting)
        {
            if (EnemyIsAlive())
            {
                return;
            }
            else
            {
                Debug.Log("Wave Completed");
                nextWave++;
                SetWave(nextWave);
                waves[nextWave].index = nextWave;
                // Save(fullPath, waves[nextWave]);
            }
        }

        if (waveCountdown <= 0)
        {
            if(state != SpawnState.Spawning)
            {
                StartCoroutine(SpawnWave(waves[nextWave]));
            }
        }
        else
        {
            waveCountdown -= Time.deltaTime;
        }

        /*
        switch (nextWave)
        {
            case 4:
                finalWaveUI.gameObject.SetActive(true);
                if (state == SpawnState.Spawning)
                {
                    finalWaveUI.gameObject.SetActive(false);
                }
                break;
            case 5:
                completeLevelUI.SetActive(true);
                break;
        }
        */

        if (nextWave == 4)
        {
            finalWaveUI.gameObject.SetActive(true);
            if (state == SpawnState.Spawning)
            {
                finalWaveUI.gameObject.SetActive(false);
            }
        }

        if (nextWave == 5)
        {
            completeLevelUI.SetActive(true);
        }

        if(PlayerStats.Lives <= 0)
        {
            if (!gameOverAudio.isPlaying)
            {
                Debug.Log("Play this audio source");
                gameOverAudio.Play();
            }
        }
    }

    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;
        if(searchCountdown <= 0f)
        {
            searchCountdown = 1f;
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator SpawnWave(Wave _wave)
    {
        Debug.Log("Spawning Enemy" + _wave.name);
        state = SpawnState.Spawning;
        // Spawn
        for (int i = 0; i < _wave.count; i++)
        {
            SpawnEnemy(_wave.enemy);
            yield return new WaitForSeconds(1f/_wave.rate);
        }

        state = SpawnState.Waiting;

        yield break;
    }

    void SpawnEnemy(Transform _enemy)
    {
        Debug.Log("Spawning Enemy: " + _enemy.name);
        Transform clone = Instantiate(_enemy, transform.position, transform.rotation);
        Enemy enemy = clone.GetComponent<Enemy>();
        enemy.waypointParent = waypointParent;
    }


    /* public void Save(string path, Wave wave)
    {
        var serializer = new XmlSerializer(typeof(Wave));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, wave);
        }
    }*/

    public static Wave Load(string path)
    {
        var serializer = new XmlSerializer(typeof(Wave));
        using (var stream = new FileStream(path, FileMode.Open))
        {
            return serializer.Deserialize(stream) as Wave;
        }
    }
}