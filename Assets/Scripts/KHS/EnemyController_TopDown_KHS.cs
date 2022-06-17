using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController_TopDown_KHS : MonoBehaviour
{
    public Transform firePoint;                     
    //public GameObject npcDeadPrefab;                

    public Transform playerTransform;
    // Start is called before the first frame update
    public float attackDistance = 3f;              
    public float movementSpeed = 4f;                
    public int npcHP = 100;    
    public int npcDamage;
    public float attackRate = 2.617f;   
    float nextAttackTime = 0;
    Rigidbody r;

    [SerializeField]
    Animator enemyAnimator;
    [SerializeField]
    AudioSource AS;
    [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
    [SerializeField] private AudioClip m_ScreamSound;           // the sound played when character touches back on ground.


    void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void Start()
    {
        // 물리, 공격 설정
        r = GetComponent<Rigidbody>();                      // Rigidbody �߰�
        r.useGravity = false;                               // Gravity ���߿� �߰�
        r.isKinematic = true;                               // Kinematic? ���߿� ����
        npcDamage = 1;
        attackRate = 2.617f;
        nextAttackTime = 0.0f;

        AS = GetComponent<AudioSource>();
        PlayScreamSound();
                Quaternion temp=Quaternion.LookRotation(new Vector3(playerTransform.transform.position.x,0,playerTransform.transform.position.y)-new Vector3(this.transform.position.x,0,this.transform.position.x));
                Debug.Log(temp.eulerAngles);
        //transform.Rotate(new Vector3(0,180,-180));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
//       transform.LookAt(new Vector3(playerTransform.transform.position.x, transform.position.y, playerTransform.position.z));
    }

    private void PlayScreamSound()
    {
        AS.clip = m_ScreamSound;
        AS.PlayDelayed(2.32f);
    }
}
