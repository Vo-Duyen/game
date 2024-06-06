using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class SlimeAI : MonoBehaviour
{
    private GameManager _gm;

    [Header("Config Health Bar")]
    public const float HP = 20;
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
        //if (!isWalk)
        //    amin.SetTrigger("Attack");
    }
    IEnumerator timeIsDie()
    {
        yield return new WaitForSeconds(1.7f);
        Destroy(gameObject);
    }
    private void GetHit(float n)
    {
        hp -= n;
        if (hp <= 0 )
        {
            amin.SetTrigger("Die");
            _gm.Plus();
            sliderHealtBar.value = 0;
            StartCoroutine(timeIsDie());
        }
        else
        {
            sliderHealtBar.value = hp / HP;
            amin.SetTrigger("GetHit");
            ChangeState(enemyState.FURY);
        }
    }
    private void StateManager()
    {
        switch(state)
        {
            case enemyState.IDLE:
                break;
            case enemyState.ALERT:
                break;
            case enemyState.FOLLOW:
                break;
            case enemyState.FURY:
                destination = _gm.player.position;
                agent.destination = destination;
                break;
            case enemyState.PATROL:
                break;
            case enemyState.EXPLORE:
                break;
        }
    }
    private void ChangeState(enemyState newState)
    {
        StopAllCoroutines();
        state = newState;
        //print(state);
        switch (state)
        {
            case enemyState.IDLE:
                destination = transform.position;
                agent.destination = destination;
                StartCoroutine("IDLE");
                break;
            case enemyState.ALERT:
                break;
            case enemyState.PATROL:
                agent.stoppingDistance = 0;
                idWayPoint = Random.Range(0, _gm.slimeWayPoints.Length);
                destination = _gm.slimeWayPoints[idWayPoint].position;
                agent.destination = destination;
                StartCoroutine("PATROL");
                break;
            case enemyState.FURY:
                agent.stoppingDistance = _gm.distanceToAttack;
                agent.stoppingDistance = 0.3f;
                destination = _gm.player.position;
                agent.destination = destination;
                //print(destination);
                break;
        }
    }
    IEnumerator IDLE()
    {
        yield return new WaitForSeconds(_gm.slimeIdleWaitTime);
        stayStill(50);
    }
    IEnumerator PATROL()
    {
        yield return new WaitUntil(() => agent.remainingDistance <= 0);
        //yield return new WaitForSeconds(patrolWaitTime);
        stayStill(30);
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
}
