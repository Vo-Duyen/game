using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public enum enemyState
{
    IDLE, ALERT, PATROL, FOLLOW, FURY, EXPLORE, DIE, 
}
public class GameManager : MonoBehaviour
{
    [Header("Slime AI")]
    public Transform[] slimeWayPoints;
    public float slimeIdleWaitTime;
    public Transform player;
    public float slimeStopDistance;
    public float distanceToAttack;
    public float slimeAlertWaitTime;
    public float slimeAttackDelay;
    public float slimeLookAtSpeed;

    public TMP_Text tx;

    [Header("Rain Manager")]
    public PostProcessVolume postB;
    public ParticleSystem rainParticle;
    private ParticleSystem.EmissionModule rainModule;
    public int rainRateOverTime;
    public int rainIncrement;
    public float rainIncrementDelay;

    // Start is called before the first frame update
    void Start()
    {
        rainModule = rainParticle.emission;
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
    public void OnOffRain(bool isRain)
    {
        StopCoroutine("RainManager");
        StopCoroutine("PostBManager");
        StartCoroutine("RainManager", isRain);
        StartCoroutine("PostBManager", isRain);
    }
    IEnumerator RainManager(bool isRain)
    {
        switch (isRain)
        {
            case true:
                for (float i = rainModule.rateOverTime.constant; i < rainRateOverTime; i += rainIncrement)
                {
                    rainModule.rateOverTime = i;
                    yield return new WaitForSeconds(rainIncrementDelay);
                }
                rainModule.rateOverTime = rainRateOverTime;
                break;
            case false:
                for (float i = rainModule.rateOverTime.constant; i > 0; i -= rainIncrement)
                {
                    rainModule.rateOverTime = i;
                    yield return new WaitForSeconds(rainIncrementDelay);
                }
                rainModule.rateOverTime = 0;
                break;
        }
    }
    IEnumerator PostBManager(bool isRain)
    {
        switch (isRain)
        {
            case true:
                for (float i = postB.weight; i < 1; i += Time.deltaTime)
                {
                    postB.weight = i;
                    yield return new WaitForEndOfFrame();
                }
                postB.weight = 1;
                break;
            case false:
                for (float i = postB.weight; i > 0; i -= Time.deltaTime)
                {
                    postB.weight = i;
                    yield return new WaitForEndOfFrame();
                }
                postB.weight = 0;
                break;
        }
    }
}
