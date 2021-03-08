using FMODUnity;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NavMeshAgent))]
public class babyAI : NPC
{
    [SerializeField] StudioEventEmitter eventEmitter = null;

    const float WantsToMom = 0.0f;
    const float Walking = 1.0f;
    const float Relax = 2.0f;

    public Transform[] wayPoints = null;
    public float eyeAngle = 45f;
    public float moveSpeed = 3.5f;
    public float runSpeed = 8f;
    public float searchTime = 8f;
    public float playerCatchDist = 2f;
    public float stepLength = 3f;

    Animator animator;

    private float timer;
    float searchTimer = 0f;
    bool playerSpotted;
    NavMeshAgent agent;
    RaycastHit hit;
    int i = 0;

    private FMOD.Studio.EventInstance foosteps;

    public float VoiceCondition { get; private set; }

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        ChangeCondition(Walking);
    }
    private void Update()
    {
        RefreshStates();
        if (!playerSpotted)
        {
            searchTimer -= Time.deltaTime;
            if(searchTimer <= 0)
                Patrol();
            else
            {
                AttackPlayer();
            }
        }
        else
        {
            AttackPlayer();
        }
    }
    private void TryPlayFootstep(float speed)
    {
        if (timer < Time.time)
        {
            print("BabyStep");
            PlayFootstep(speed);
            timer = Time.time + stepLength / moveSpeed;
        }
    }
    private void PlayFootstep(float speed)
    {
        foosteps = FMODUnity.RuntimeManager.CreateInstance("event:/BabySteps");
        foosteps.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position + Vector3.down));
        foosteps.start();
        foosteps.release();
    }
    void RefreshStates()
    {
        SeePlayer();
    }
    void SeePlayer()
    {
        var dirOnPlayer = Player.instance.transform.position - transform.position;
        var angle = Vector3.Angle(transform.forward, dirOnPlayer);
        Physics.Raycast(transform.position, Player.instance.transform.position - transform.position, out hit);
        playerSpotted = hit.transform != null && /*hit.transform.tag == "Player" &&*/
            angle <= eyeAngle;
    }
    void Patrol()
    {
        if (state != NPCState.Patrol)
        {
            ControllerNPC.OnRelax(this);
            agent.speed = moveSpeed;
            state = NPCState.Patrol;
            animator.SetBool("IsAgr", false);
            ChangeCondition(Walking);
        }
        if (Vector3.Distance(transform.position, wayPoints[i].position) < 3f)
        {
            i++;
            if (i >= wayPoints.Length)
                i = 0;
        }
        TryPlayFootstep(moveSpeed);
        agent.SetDestination(wayPoints[i].position);
    }
    void AttackPlayer()
    {
        if (state != NPCState.Attack)
        {
            ControllerNPC.OnAgression(this);
            agent.speed = runSpeed;
            state = NPCState.Attack;
            searchTimer = searchTime;
            animator.SetBool("IsAgr", true);
            ChangeCondition(WantsToMom);
        }
        if (Vector3.Distance(Player.instance.transform.position, transform.position) <= playerCatchDist)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        TryPlayFootstep(runSpeed);
        agent.SetDestination(Player.instance.transform.position);
    }
    public void ChangeCondition(float condition)
    {
        if (condition == VoiceCondition)
            return;
        VoiceCondition = condition;
        eventEmitter.SetParameter(eventEmitter.Params[0].Name, condition, true);
    }
    private void OnDrawGizmos()
    {
        if(Player.instance)
            Gizmos.DrawRay(transform.position, Player.instance.transform.position - transform.position);
    }
}
