using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMultiple_KHS : MonoBehaviour
{
[SerializeField]
    GameObject[] Enemy;
    [SerializeField]
    int numOfEnemies = 0;


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
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enemy SpawnA");
        SpawnEnemy(0);
    }

    private void SpawnEnemy(int SpawnIndex)
    {
        if (spawnEnemyIndex[SpawnIndex] != true)    // ���� �������� �ʾҴٸ�
        {
            Debug.Log(spawnEnemyIndex[SpawnIndex]);
            spawnEnemyIndex[SpawnIndex++] = true;
            enemy = Instantiate(Enemy[0]);                  // �� ����
            enemy.transform.Translate(new Vector3(0, 0, -2));
            
        }
        
    }
}
