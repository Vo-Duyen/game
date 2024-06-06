using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private CharacterController ccl;
    [Header("Config Player")]
    public float movementSpeed = 3f;
    private const float HP = 10;
    public float hp = HP;
    public Slider healthBar;

    [Header("Cameras")]
    public GameObject cam2;
    private int cnt = 0;

    private Vector3 diracsion;
    private Animator amin;

    [Header("Config Attack")]
    public ParticleSystem attack2;
    public Transform hitBox;
    [Range(0.2f, 1f)]
    public float hitRange = 0.3f;
    private bool isAttack2 = false;
    public Collider[] hitInfor;
    public LayerMask hitMask;
    public float dmg = 1;

    // Start is called before the first frame update
    void Start()
    {
        ccl = GetComponent<CharacterController>();   
        amin = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ++cnt;
            cnt %= 2;
            if (cnt == 1)
            {
                cam2.SetActive(true);
            }
            else
            {
                cam2.SetActive(false);
            }
        }
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
        if (Input.GetMouseButtonDown(0))
        {
            if (isAttack2 == false)
            {
                attack2.Emit(1);
                isAttack2 = true;
            }
            amin.SetTrigger("isAttack");
            hitInfor = Physics.OverlapSphere(hitBox.position, hitRange, hitMask);
            foreach (Collider c in hitInfor)
            {
                c.gameObject.SendMessage("GetHit", dmg, SendMessageOptions.DontRequireReceiver);
            }
        }
        ccl.Move(diracsion * movementSpeed * Time.deltaTime);
    }
    void onIsAttack2()
    {
        isAttack2 = false;
    }
    private void OnDrawGizmosSelected()
    {
        if (hitBox != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(hitBox.position, hitRange);
        }
    }
    IEnumerator timeIsDie()
    {
        yield return new WaitForSeconds(1.7f);
        //Destroy(gameObject);
        gameObject.SetActive(false);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Slime"))
        {
            hp -= 1;
            if (hp <= 0)
            {
                healthBar.value = 0;
                amin.SetTrigger("die");
                StartCoroutine(timeIsDie());
            }
            else
            {
                healthBar.value = hp / HP;
                amin.SetTrigger("isAttack");
            }
        }
    }
}
