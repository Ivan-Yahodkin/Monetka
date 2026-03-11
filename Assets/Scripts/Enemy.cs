using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    [Header("Stats")]
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float damage = 10f;

    [Header("Targets")]
    [SerializeField] private GameObject firtsTarget;
    [SerializeField] private Transform player;

    [Header("Layers")]
    [SerializeField] private LayerMask layerPL;
    [SerializeField] private LayerMask layerDoor;

    [Header("Combat")]
    [SerializeField] private float cheaseRadius = 5f;
    [SerializeField] private float attackRadius = 5f;       // зона обнаружения контакта
    [SerializeField] private float contactRadius = 1.2f;    // контакт для дверей
    [SerializeField] private float baseAttackDistance = 2f; // дистанция атаки базы
    [SerializeField] private float doorDetectDistance = 1.5f;
    [SerializeField] private float attackCooldown = 1f;

    private Transform currentTarget;
    private HP hpTarget;
    private Door doorTarget;
    private Collider targetCollider;

    private float lastAttackTime;
    private bool baseFound = true;



    private void Start()
    {
        agent.speed = speed;
        agent.autoBraking = false;


        firtsTarget = GameObject.FindGameObjectWithTag("Base");

        if (firtsTarget == null)
        {
            baseFound = false;
            agent.isStopped = true;
            Debug.Log("Enemy: база не найдена");
            return;
        }

        SetTarget(firtsTarget.transform);
    }

    private void Update()
    {
        if (!baseFound) return;

        ValidateTarget();

        // Сначала проверяем игрока
        CheckPlayerAndPath();

        // Потом проверяем дверь только если нет игрока или путь свободен
        CheckDoorOnPath();

        Move();
        AttackIfContact();
    }

    // ===== ПРОВЕРКА ЦЕЛИ =====
    private void ValidateTarget()
    {
        if (currentTarget == null)
            SetTarget(firtsTarget.transform);
    }

    // ===== ПРИОРИТЕТ ИГРОКА =====
    private void CheckPlayerAndPath()
    {
        if (player != null)
        {
            float distToPlayer = Vector3.Distance(transform.position, player.position);

            if (distToPlayer <= cheaseRadius)
            {
                // проверяем дверь на пути к игроку
                Vector3 dir = (player.position - transform.position).normalized;

                if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, out RaycastHit hit, distToPlayer, layerDoor))
                {
                    SetTarget(hit.transform); // дверь мешает
                }
                else
                {
                    SetTarget(player); // путь чист ? игрок
                }
            }
        }
    }

    // ===== ДВЕРЬ НА ПУТИ =====
    private void CheckDoorOnPath()
    {
        if (currentTarget == null || doorTarget != null) return;
        if (hpTarget != null && ((1 << hpTarget.gameObject.layer) & layerPL) != 0) return; // если цель игрок ? не ищем дверь

        Vector3 dir = (currentTarget.position - transform.position).normalized;
        float dist = Vector3.Distance(transform.position, currentTarget.position);

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, out RaycastHit hit, dist, layerDoor))
        {
            SetTarget(hit.transform); // дверь на пути к цели
        }
    }

    // ===== УСТАНОВКА ЦЕЛИ =====
    private void SetTarget(Transform target)
    {
        currentTarget = target;
        hpTarget = target.GetComponent<HP>();
        doorTarget = target.GetComponent<Door>();
        targetCollider = target.GetComponent<Collider>();

        if (doorTarget == null && hpTarget != null)
            agent.stoppingDistance = baseAttackDistance;
        else
            agent.stoppingDistance = contactRadius;
    }

    // ===== ДВИЖЕНИЕ =====
    private void Move()
    {
        if (currentTarget == null) return;

        bool inContact = CheckContact();

        if (!inContact)
        {
            agent.isStopped = false;
            agent.updatePosition = true;
            agent.updateRotation = true;
            agent.SetDestination(currentTarget.position);
        }
        else
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.updatePosition = false;
            agent.updateRotation = false;
        }
    }

    // ===== ПРОВЕРКА КОНТАКТА =====
    private bool CheckContact()
    {
        if (currentTarget == null || targetCollider == null)
            return false;

        Vector3 closest = targetCollider.ClosestPoint(transform.position);
        float dist = Vector3.Distance(transform.position, closest);

        if (doorTarget != null)
            return dist <= contactRadius;

        return dist <= baseAttackDistance;
    }

    // ===== АТАКА =====
    private void AttackIfContact()
    {
        if (currentTarget == null) return;
        if (!CheckContact()) return;
        if (Time.time < lastAttackTime + attackCooldown) return;

        if (hpTarget != null)
            hpTarget.HpDamage(damage);

        if (doorTarget != null)
        {
            doorTarget.TakeDamage(damage);
            if (doorTarget == null)
                SetTarget(firtsTarget.transform);
        }

        lastAttackTime = Time.time;
    }

    // ===== GIZMOS =====
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, contactRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, baseAttackDistance);

        Gizmos.color = Color.magenta;
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawLine(origin, origin + transform.forward * doorDetectDistance);
    }
}
