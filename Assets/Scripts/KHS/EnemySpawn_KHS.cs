using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawn_KHS : MonoBehaviour
{
    public GameObject[] enemyPrefab;
    //public PlayerDamageReceiver_KHS player;
    public GameObject player;

    //public Texture crosshairTexture;
    public float spawnInterval = 1; //Spawn new enemy each n seconds
    public int enemiesPerWave = 1; //How many enemies per wave
    public Transform[] spawnPoints; // spawnPoints를 지정한다.

    private bool isEnter = false;

    float nextSpawnTime = 0;
    int waveNumber = 1;
    bool waitingForWave = true;
    float newWaveTimer = 0;

    float GateTimer = 3f;

    int enemiesToEliminate;
    //How many enemies we already eliminated in the current wave
    int enemiesEliminated = 0;
    int totalEnemiesSpawned = 0;


    // Start is called before the first frame update
    void Start()
    {
        // 랜덤 타임 지정
        newWaveTimer = 0f;
        waitingForWave = true;
        waveNumber = 1;
        spawnInterval = Random.Range(0,1f);
        enemiesToEliminate = waveNumber * enemiesPerWave;
        totalEnemiesSpawned = 0;

    }


    // Update is called once per frame
    void Update()
    {
        if (isEnter != false){
            SpawnEnemies();
            //GateStart();
        }
                    
    }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player"){
            isEnter = true;
            }
        }
        

        private void SpawnEnemies() {

        
            if(Time.time > nextSpawnTime)
            {
                nextSpawnTime = Time.time + Random.Range(0,1f);;

                //Spawn enemy 
                if(totalEnemiesSpawned < enemiesToEliminate)
                {
                    Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length - 1)];

                    int enemyType = (int)Mathf.Round(Random.Range(0,enemyPrefab.Length));

                    GameObject enemy = Instantiate(enemyPrefab[enemyType], randomPoint.position, Quaternion.identity);
                    EnemyController_KHS npc = enemy.GetComponent<EnemyController_KHS>();
                    npc.playerTransform = player.transform;
                    npc.es = this;
                    totalEnemiesSpawned++;
                }
            }
        }

        private void GateStart(){
            if(GateTimer >= 0)
            {
                GateTimer -= Time.deltaTime;
            } else {
                GameObject SpeedGate = GameObject.Find("SpeedGate (12)");
                SpeedGate.GetComponent<SpeedGate>().ReaccelationStart();
            }
        }

    /*
        if (player.playerHP <= 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Scene scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.name);
            }
        }
        */
}