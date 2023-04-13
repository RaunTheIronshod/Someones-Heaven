using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent agent;

	[SerializeField]
	private Animator enemyAnimator;

    private bool isMalabranche;
    private bool isScaramoosh;

    [SerializeField]
    private static string SelectedTag = "Player";
    [SerializeField]
    private float endPushSpeed = 0.1f;

    [SerializeField]
    float radiusToTarget = 1.25f;

    [SerializeField]
    private int attackDamage = 1;

    [SerializeField] [Range(0,1)]
    private float hitChance = 0.5f;

    [SerializeField]
    private float attackRange = 2;

    [SerializeField]
    private float attackCD = 1;
    private float attackTime = 0;

    [SerializeField]
    Material material;

    [SerializeField]
    private GameObject blood1;
    [SerializeField]
    private GameObject blood2;
    [SerializeField]
    private GameObject blood3;


    [SerializeField]
    private GameObject specialFX;
    [SerializeField]
    private int specialCooldown;
    [SerializeField]
    private int specialCounter;
    [SerializeField]
    private int specialRange;
    public int specialDamage;
    public float specialAttackSpeedNerf;


    private GameObject enemySpecialFX1;
    private GameObject enemySpecialFX2;
    private GameObject enemySpecialFX3;
    private GameObject enemySpecialFX4;

    public int enemyHP;
    public int maxEnemyHp;
    public Traps currentTrap;

    private GameObject enemySpecialFX;

    GameObject[] targets;
    GameObject closest;

    Rigidbody rb;
	

    private void Start()
    {
		enemyAnimator = GetComponent<Animator>();

        rb = GetComponent<Rigidbody>();

        BloodOff();

        EnemySpecial0ff();

        IDictionary(this.gameObject == GameObject.FindGameObjectsWithTag);
    }

    

    GameObject ClosestTarget()
    {
        targets = GameObject.FindGameObjectsWithTag(SelectedTag);

        float distance = Mathf.Infinity;
        Vector3 pos = transform.position;

        foreach (GameObject target in targets)
        {
            Vector3 difference = target.transform.position - pos;
            float currentDistance = difference.sqrMagnitude;
            if (currentDistance < distance)
            {
                closest = target;
                distance = currentDistance;
            }
        }
        return closest;
    }

    bool IsPushActive()
    {
        return !agent.enabled;
    }

    void StartPush()
    {
        agent.enabled = false;
        rb.isKinematic = false;
    }

    void EndPush()
    {
        agent.enabled = true;
        rb.isKinematic = true;
    }

    void MoveToClosest()
    {
        if (enemyAnimator)
        {
            enemyAnimator.SetBool("isMoving", true);
        }
		//radius round target
		Vector3 target = transform.position - closest.transform.position;
        target = target.normalized * radiusToTarget + closest.transform.position;

        //move to target radius
        agent.SetDestination(target);

        //Debug.DrawLine(target, transform.position);
    }

    void FixedUpdate()
    {
        if (IsPushActive())
        {
            Vector3 horiz = rb.velocity;
            horiz.y = 0;

            //This function can be called before rb.AddForce() actually changes the velocity
            //hence the > 0 check
            bool slowedDown = horiz.sqrMagnitude > 0 && horiz.sqrMagnitude < endPushSpeed * endPushSpeed;
            if (slowedDown)
            {
                EndPush();
            }
        }
        else if (CanAttack())
        {
            Attack();
        }
        else
        {
            ClosestTarget();
            MoveToClosest();
        }
        if (material)
        {
            Color color = Time.time >= attackTime ? Color.red : Color.black;
            material.color = color;
        }
		//if (enemyAnimator) {
		//	enemyAnimator.SetBool("isAttacking", false);
		//}
	}

    private bool CanAttack()
    {
        if (!closest || Time.time < attackTime)
        {
            return false;
        }

        Vector3 distance = transform.position - closest.transform.position;
        return distance.magnitude <= attackRange;
    }

    private void Attack()
    {
        if (enemyAnimator)
        {
            enemyAnimator.SetBool("isAttacking", true);
        }
        bool hit = Random.value <= hitChance;
        if (hit)
        {
            Player player = closest.GetComponent<Player>();
            player.TakeDamage(attackDamage);
        }
        attackTime = Time.time + attackCD;
        Invoke("AnimatorAttackOff", 0.5f);
    }

    public void Push(Vector3 force)
    {
        StartPush();
        rb.AddForce(force);
    }

    public void EnemySpecial0ff()
    {
        enemySpecialFX1.SetActive(false);
        enemySpecialFX2.SetActive(false);
        enemySpecialFX3.SetActive(false);
        enemySpecialFX4.SetActive(false);
    }

    public void SetControllerSpeed()
    {

    }

    public void EnemySpecialOn()
    {
        enemySpecialFX1.SetActive(true);
        enemySpecialFX2.SetActive(true);
        enemySpecialFX3.SetActive(true);
        enemySpecialFX4.SetActive(true);
    }

    private void AnimatorAttackOff()
    {
        enemyAnimator.SetBool("isAttacking", false);
    }

    public void BloodOn()
    {
        blood1.SetActive(true);
        blood2.SetActive(true);
        blood3.SetActive(true);
    }

    public void BloodOff()
    {
        blood1.SetActive(false);
        blood2.SetActive(false);
        blood3.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        damage = currentTrap.damage;
        enemyHP--;
        if (enemyHP <= 0)
        {
            EnemyDeath();
        }

        BloodOn();
        Invoke("BloodOff", 0.5f);
    }

    public void EnemyDeath()
    {
        Destroy(this.gameObject);
        Debug.Log("gameobjectDestroyed");
    }
}
