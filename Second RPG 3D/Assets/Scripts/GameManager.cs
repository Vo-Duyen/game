using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum enemyState
{
    IDLE, ALERT, PATROL, FOLLOW, FURY, EXPLORE
}
public class GameManager : MonoBehaviour
{
    [Header("Slime AI")]
    public Transform[] slimeWayPoints;
    public float slimeIdleWaitTime;
    public Transform player;
    public float distanceToAttack;
    public float slimeAlertWaitTime;
    public float slimeAttackDelay;
    public float slimeLookAtSpeed;

    public TMP_Text tx;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Plus()
    {
        int tam = int.Parse(tx.text);
        ++tam;
        tx.text = tam.ToString();
    }
}
