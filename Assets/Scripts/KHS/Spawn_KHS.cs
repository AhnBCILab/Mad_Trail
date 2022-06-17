using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_KHS : MonoBehaviour
{
    [SerializeField]
    GameObject[] Enemy;
    [SerializeField]
    int numOfEnemies = 0;
    float GateTimer = 6f;
    private bool isEnter = false;


    private bool[] spawnEnemyIndex = { false };
    GameObject enemy;

    private void Awake()
    {
  
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (isEnter != false)
        //    GateStart();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player"){
            isEnter = true;
            SpawnEnemy(0);
            }
    }

    private void SpawnEnemy(int SpawnIndex)
    {
        if (spawnEnemyIndex[SpawnIndex] != true)    // ���� �������� �ʾҴٸ�
        {
            Debug.Log(spawnEnemyIndex[SpawnIndex]);
            spawnEnemyIndex[SpawnIndex++] = true;
            enemy = Instantiate(Enemy[0], transform.position, Quaternion.identity);
            // Instantiate(enemyPrefab[enemyType], randomPoint.position, Quaternion.identity);
            enemy.transform.SetParent(transform);
            //enemy.transform.Translate(new Vector3(0, 0, -2));
            
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
}
