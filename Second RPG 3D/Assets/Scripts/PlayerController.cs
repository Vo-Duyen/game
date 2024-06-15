using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private GameManager _gm;
    private CharacterController ccl;
    [Header("Config Player")]
    public float movementSpeed = 3f;
    private const float HP = 10;
    public float hp = HP;
    public Slider healthBar;

    [Header("Cameras")]
    public GameObject cam2;

    private Vector3 diracsion;
    private Animator amin;

    [Header("Config Attack")]
    public ParticleSystem attack2;
    public Transform hitBox;
    [Range(0.2f, 1f)]
    public float hitRange = 0.3f;
    private bool isAttack = false;
    public Collider[] hitInfor;
    public LayerMask hitMask;
    public float dmg = 1;

    // Start is called before the first frame update
    void Start()
    {
        _gm = FindObjectOfType(typeof(GameManager)) as GameManager;
        ccl = GetComponent<CharacterController>();   
        amin = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        if (_gm.gameState != GameState.GAMEPLAY) { return; }
        SetAminWalk();
        Inputs();
        ccl.Move(diracsion * movementSpeed * Time.deltaTime);
    }

    private void Inputs()
    {
        if (Input.GetMouseButtonDown(0) && !isAttack)
        {
            Attack();
        }
    }

    private void Attack()
    {
        attack2.Emit(1);
        isAttack = true;
        amin.SetTrigger("isAttack");
        hitInfor = Physics.OverlapSphere(hitBox.position, hitRange, hitMask);
        foreach (Collider c in hitInfor)
        {
            c.gameObject.SendMessage("GetHit", dmg, SendMessageOptions.DontRequireReceiver);
        }
    }
    void onIsAttack()
    {
        isAttack = false;
    }

    private void SetAminWalk()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        diracsion = new Vector3(horizontal, 0f, vertical).normalized;
        if (diracsion.magnitude > 0.1f)
        {
            float quay = Mathf.Atan2(diracsion.x, diracsion.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, quay, 0f);
            amin.SetBool("isWalk", true);
        }
        else
        {
            amin.SetBool("isWalk", false);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hitBox.position, hitRange);
    }
    IEnumerator timeIsDie()
    {
        yield return new WaitForSeconds(1.7f);
        print("Die");
        _gm.ChangeGameState(GameState.DIE);
    }
    void GetHit(int amount)
    {
        hp -= amount;
        if (hp > 0)
        {
            healthBar.value = (float) hp / HP;
            amin.SetTrigger("getHit");
        }
        else
        {
            healthBar.value = 0f;
        }
        if (hp == 0)
        {
            amin.SetTrigger("die");
            StartCoroutine(timeIsDie());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Slime"))
        {
            GetHit(1);
        }
        if (other.gameObject.CompareTag("cam"))
        {
            cam2.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("cam"))
        {
            cam2.SetActive(false);
        }
    }
}
