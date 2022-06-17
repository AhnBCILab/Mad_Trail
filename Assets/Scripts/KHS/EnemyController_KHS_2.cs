using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]

public class EnemyController_KHS_2 : MonoBehaviour
{
    public float attackDistance = 3f;              
    public float movementSpeed = 4f;                
    public int npcHP = 100;                       
    //How much damage will npc deal to the player
    public int npcDamage;                     
    public float attackRate = 2.617f;                
    public Transform firePoint;                     
    //public GameObject npcDeadPrefab;                

    public Transform playerTransform;
    // [HideInInspector]   // Makes a variable not show up in the inspector but be serialized.
    public EnemySpawn_KHS es;   // Enemy Spawn Scipt
    NavMeshAgent agent;         // AI �̿��� ���� agent
    float nextAttackTime = 0;   // ���� �ð� ����
    Rigidbody r;                // ���� ��Ģ ����



    [SerializeField]
    Animator enemyAnimator;
    [SerializeField]
    AudioSource AS;
    //[SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.
    [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
    [SerializeField] private AudioClip m_ScreamSound;           // the sound played when character touches back on ground.


    [SerializeField] private float m_StepInterval;


    private float m_StepCycle;
    private float m_NextStep;

    private float count= 0;


    // Start is called before the first frame update
    void Start() { 
        // AI ����
        agent = GetComponent<NavMeshAgent>();               // AI ��� �߰�
        agent.stoppingDistance = attackDistance;            // ���� �Ÿ� ����
        agent.speed = movementSpeed;                        // Enemy Speed ����

        // 물리, 공격 설정
        r = GetComponent<Rigidbody>();                      // Rigidbody �߰�
        r.useGravity = false;                               // Gravity ���߿� �߰�
        r.isKinematic = true;                               // Kinematic? ���߿� ����
        npcDamage = 1;
        attackRate = 2.617f;
        nextAttackTime = 0.0f;

        m_StepCycle = 0f;
        m_NextStep = m_StepCycle/2f;
            
        AS = GetComponent<AudioSource>();

        
    }

    void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }


    void Update(){
        if (enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("standUp")){
            Debug.Log("Scream.!!");
            PlayScreamSound();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    

        Attack();

        if (agent.remainingDistance - attackDistance < 0.01f)   
        {
            if (Time.time >= nextAttackTime)                     
            {
                nextAttackTime = Time.time + attackRate;
                //AS.clip = m_LandSound;        
                //AS.PlayDelayed(0.2f);
                EnemyAttack();
            }
        }
    
        //Move towardst he player
        agent.destination = playerTransform.position;
        //Always look at player
        transform.LookAt(new Vector3(playerTransform.transform.position.x, transform.position.y, playerTransform.position.z));
        //Gradually reduce rigidbody velocity if the force was applied by the bullet
        r.velocity *= 0.99f;
    }

    public void EnemyAttack()
    {
        //Attack
        RaycastHit hit;                                
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, attackDistance))
        {
            if (hit.transform.CompareTag("Player"))     
            {
                Debug.DrawLine(firePoint.position, firePoint.position + firePoint.forward * attackDistance, Color.cyan);

                //IEntity player = hit.transform.GetComponent<IEntity>();
                //player.ApplyDamage(npcDamage);          
            }
        }
    }

    public void Attack()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) > 4.0f)
        {
            enemyAnimator.SetBool("isAttack", false);
            enemyAnimator.SetBool("isRun", true);

            // isRun Sound
            if (enemyAnimator.GetBool("isRun")){
                if (!AS.isPlaying)
                    PlayFootStepAudio();
            } else 
                AS.Stop();
        }
        else
        {
            enemyAnimator.SetBool("isAttack", true);
            enemyAnimator.SetBool("isRun", false);
        }
    }

    private void PlayScreamSound()
    {
        AS.clip = m_ScreamSound;
        AS.Play();
    }


    private void PlayFootStepAudio()
    {
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            //int n = Random.Range(1, m_FootstepSounds.Length);
            AS.clip = m_FootstepSounds[0];
            AS.PlayOneShot(AS.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[0] = m_FootstepSounds[0];
            m_FootstepSounds[0] = AS.clip;
    }

    public void ApplyDamage(int points)
    {
        npcHP -= points;
        if (npcHP <= 0)  // npCHP�� 0�� �Ǹ� �״´�.
        {
            agent.enabled = false;
            enemyAnimator.SetBool("isDamage", true);
            Destroy(gameObject, 2.0f);
        }

        else
        {
            enemyAnimator.SetBool("isDamage", false);
        }

        Debug.Log("Enemy's HP:" + npcHP);
    }
}