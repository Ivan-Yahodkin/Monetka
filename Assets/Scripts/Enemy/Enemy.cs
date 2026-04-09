using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    [Header("Stats")]
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float rotationSpeed = 360f;

    [Header("Targets")]
    [SerializeField] private GameObject firstTarget;
    [SerializeField] private Transform player;

    [Header("Layers")]
    [SerializeField] private LayerMask layerPL;
    [SerializeField] private LayerMask layerDoor;

    [Header("Combat")]
    [SerializeField] private float chaseRadius = 5f;
    [SerializeField] private float doorContactRadius = 1.2f;
    [SerializeField] private float baseAttackDistance = 2f;
    [SerializeField] private float doorDetectDistance = 1.5f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Animation")]
    [SerializeField] private AnimationController animContr;

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

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
            Debug.LogWarning("Enemy: игрок не найден по тегу Player");

        firstTarget = GameObject.FindGameObjectWithTag("Base");

        if (firstTarget == null)
        {
            baseFound = false;
            agent.isStopped = true;
            Debug.Log("Enemy: база не найдена");
            return;
        }

        SetTarget(firstTarget.transform);
    }

    private void Update()
    {
        if (!baseFound) return;

        ValidateTarget();
        CheckPlayerAndPath();
        CheckDoorOnPath();
        Move();
        AttackIfContact();
    }

    // ===== ПРОВЕРКА ЦЕЛИ =====
    private void ValidateTarget()
    {
        // Повторный поиск игрока если он заспавнился позже
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (currentTarget == null)
            SetTarget(firstTarget.transform);
    }

    // ===== ПРИОРИТЕТ ИГРОКА =====
    private void CheckPlayerAndPath()
    {
        if (player == null) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (distToPlayer <= chaseRadius)
        {
            Vector3 dir = (player.position - transform.position).normalized;

            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, out RaycastHit hit, distToPlayer, layerDoor))
            {
                SetTarget(hit.transform); // дверь мешает
            }
            else
            {
                SetTarget(player); // путь чист → игрок
            }
        }
        else if (currentTarget == player) // игрок убежал → возврат к базе
        {
            SetTarget(firstTarget.transform);
        }
    }

    // ===== ДВЕРЬ НА ПУТИ =====
    private void CheckDoorOnPath()
    {
        if (currentTarget == null || doorTarget != null) return;
        if (hpTarget != null && ((1 << hpTarget.gameObject.layer) & layerPL) != 0) return; // цель игрок → не ищем дверь

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
        if (target == null) return;

        currentTarget = target;
        hpTarget = target.GetComponent<HP>();
        doorTarget = target.GetComponent<Door>();
        target.TryGetComponent(out targetCollider);

        if (doorTarget != null)
            agent.stoppingDistance = doorContactRadius;
        else
            agent.stoppingDistance = baseAttackDistance;
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
            animContr.isMove(true);
        }
        else
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.updatePosition = false;
            agent.updateRotation = false; // агент не крутит — крутим вручную
            RotateToTarget();
            animContr.isMove(false);
        }
    }

    // ===== ПОВОРОТ К ЦЕЛИ =====
    private void RotateToTarget()
    {
        if (currentTarget == null) return;

        Vector3 dir = currentTarget.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );
    }

    // ===== ПРОВЕРКА КОНТАКТА =====
    private bool CheckContact()
    {
        if (currentTarget == null || targetCollider == null)
            return false;

        Vector3 closest = targetCollider.ClosestPoint(transform.position);
        float dist = Vector3.Distance(transform.position, closest);

        if (doorTarget != null)
            return dist <= doorContactRadius;

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
            GameObject doorObj = doorTarget.gameObject;
            doorTarget.TakeDamegeDoor(damage);

            // Проверяем реальное состояние объекта, а не закэшированный компонент
            if (doorObj == null || !doorObj.activeInHierarchy)
                SetTarget(firstTarget.transform);
        }

        lastAttackTime = Time.time;
    }

    public void DeadStop()
    {
        gameObject.layer = 0;
        agent.SetDestination(gameObject.transform.position);
        agent.isStopped = true;
    }

    // ===== GIZMOS =====
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, doorContactRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, baseAttackDistance);

        Gizmos.color = Color.magenta;
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawLine(origin, origin + transform.forward * doorDetectDistance);
    }
}