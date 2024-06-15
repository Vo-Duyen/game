using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Diagnostics.Tracing;

public class SlimeAI : MonoBehaviour
{
    private GameManager _gm;

    [Header("Config Health Bar")]
    public const float HP = 10;
    private float hp = HP;
    private Animator amin;

    public GameObject healthBar;
    private Slider sliderHealtBar;

    public enemyState state;
    //AI
    private NavMeshAgent agent;
    private Vector3 destination;
    private int idWayPoint;
    private bool isWalk;
    private bool isAlert;
    private bool isPlayerVisible;
    private bool isAttack;

    // Start is called before the first frame update
    void Start()
    {
        _gm = FindObjectOfType(typeof(GameManager)) as GameManager;
        amin = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        sliderHealtBar = healthBar.GetComponent<Slider>();
        hp = sliderHealtBar.value * HP;
        ChangeState(state);
    }

    // Update is called once per frame
    void Update()
    {
        StateManager();
        if (agent.desiredVelocity.magnitude >= 0.1f)
        {
            isWalk = true;
        }
        else
        {
            isWalk = false;
        }
        amin.SetBool("isWalk", isWalk);
        amin.SetBool("isAlert", isAlert);
    }
    IEnumerator timeIsDie()
    {
        yield return new WaitForSeconds(1.7f);
        Destroy(gameObject);
    }
    private void GetHit(int amount)
    {
        hp -= amount;
        if (hp > 0)
        {
            sliderHealtBar.value = hp / HP;
            amin.SetTrigger("GetHit");
            ChangeState(enemyState.FURY);
        }
        else
        {
            ChangeState(enemyState.DIE);
            amin.SetTrigger("Die");
            sliderHealtBar.value = 0;
            StartCoroutine(timeIsDie());
        }
        if (hp == 0)
        {
            _gm.Plus();
        }
    }
    private void StateManager()
    {
        switch(state)
        {
            case enemyState.IDLE:
                break;
            case enemyState.ALERT:
                LookAt();
                break;
            case enemyState.FOLLOW:
                LookAt();
                destination = _gm.player.position;
                agent.destination = destination;
                if (agent.remainingDistance <= _gm.distanceToAttack)
                {
                    Attack();
                }
                break;
            case enemyState.FURY:
                LookAt();
                destination = _gm.player.position;
                agent.destination = destination;
                if (agent.remainingDistance <= _gm.distanceToAttack)
                {
                    Attack();
                }
                break;
            case enemyState.PATROL:
                break;
            case enemyState.EXPLORE:
                break;
        }
    }

    private void LookAt()
    {
        Vector3 lookDirecsion = (_gm.player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirecsion);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _gm.slimeLookAtSpeed * Time.deltaTime);
    }

    private void Attack()
    {
        if (!isAttack && isPlayerVisible)
        {
            amin.SetTrigger("isAttack");
            isAttack = true;
        }
    }

    void onAttack()
    {
        StartCoroutine("ATTACK");
    }

    private void ChangeState(enemyState newState)
    {
        StopAllCoroutines();
        isAlert = false;
        isAttack = true;
        switch (newState)
        {
            case enemyState.IDLE:
                agent.stoppingDistance = 0;
                destination = transform.position;
                agent.destination = destination;
                StartCoroutine("IDLE");
                break;
            case enemyState.ALERT:
                agent.stoppingDistance = _gm.slimeStopDistance;
                destination = transform.position;
                agent.destination = destination;
                isAlert = true;
                StartCoroutine("ALERT");
                break;
            case enemyState.PATROL:
                agent.stoppingDistance = _gm.slimeStopDistance;
                idWayPoint = Random.Range(0, _gm.slimeWayPoints.Length);
                destination = _gm.slimeWayPoints[idWayPoint].position;
                agent.destination = destination;
                StartCoroutine("PATROL");
                break;
            case enemyState.FURY:
                agent.stoppingDistance = _gm.distanceToAttack;
                destination = _gm.player.position;
                agent.destination = destination;
                break;
            case enemyState.FOLLOW:
                agent.stoppingDistance = _gm.distanceToAttack;
                //isAttack = true;
                StartCoroutine("FOLLOW");
                //isAttack = false;
                break; 
        }
        StartCoroutine("ATTACK");
        state = newState;
    }
    IEnumerator IDLE()
    {
        yield return new WaitForSeconds(_gm.slimeIdleWaitTime);
        stayStill(50);
    }
    IEnumerator PATROL()
    {
        yield return new WaitUntil(() => agent.remainingDistance <= 0);
        stayStill(80);
    }
    IEnumerator ALERT()
    {
        yield return new WaitForSeconds(_gm.slimeAlertWaitTime);
        if (isPlayerVisible)
        {
            ChangeState(enemyState.FOLLOW);
        }
        else
        {
            stayStill(10);
        }
    }
    IEnumerator ATTACK()
    {
        yield return new WaitForSeconds(_gm.slimeAttackDelay);
        isAttack = false;
    }
    IEnumerator FOLLOW()
    {
        yield return new WaitUntil(() => !isPlayerVisible);
        yield return new WaitForSeconds(_gm.slimeAlertWaitTime);
        stayStill(50);
    }
    private void stayStill(int check)
    {
        if (Rand() > check)
        {
            ChangeState(enemyState.IDLE);
        }
        else
        {
            ChangeState(enemyState.PATROL);
        }
    }
    private int Rand()
    {
        return Random.Range(0, 100);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            isPlayerVisible = true;
            if (state == enemyState.IDLE || state == enemyState.PATROL)
            {
                ChangeState(enemyState.ALERT);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            isPlayerVisible = false;
        }
    }

}
