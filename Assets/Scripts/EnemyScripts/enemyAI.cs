using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    private Animator anim;

    public int HP = 100;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = true;

            FacePlayer();

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }

        if (anim != null)
            anim.SetBool("isWalking", agent.velocity.magnitude > 0.1f);
    }

    void Attack()
    {
        if (anim != null)
            anim.SetTrigger("attack");

        // Here you can apply damage to the player if a health script is available
        Debug.Log("Zombie is attacking");
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (anim != null)
            anim.SetTrigger("die");

        // Disable NavMeshAgent to stop movement after death
        agent.enabled = false;
        Destroy(gameObject, 2f); // Destroy the zombie after 2 seconds
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
}
