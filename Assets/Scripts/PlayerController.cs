using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private FixedJoystick fixedJoystick;

    [Header("Move Speed")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("State")]
    [SerializeField] private bool isMining, isAttack;

    [Header("Layers")]
    [SerializeField] private LayerMask isMine, isEnemy, isWall;

    [Header("Distance")]
    [SerializeField] private float miningRadius = 1f;
    [SerializeField] private float attackRadius = 13f;

    private Rigidbody playerRb;
    private List<Transform> enemiesInRange = new();

    private bool playerNearMine;
    private bool playerViewEnemy;

    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        playerNearMine = Physics.CheckSphere(transform.position, miningRadius, isMine);
        if (playerNearMine && playerRb.linearVelocity.magnitude < 0.1f)
        {
            isMining = true;
            isAttack = false;
            return;
        }
        else
        {
            isMining = false;
        }

        playerViewEnemy = Physics.CheckSphere(transform.position, attackRadius, isEnemy);
        if(playerViewEnemy && !isMining)
        {
            CheckAndAttackClosestEnemy();
        }

        Debug.Log(enemiesInRange.Count);
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        float h = fixedJoystick.Horizontal;
        float v = fixedJoystick.Vertical;

        Vector3 moveDirection = new Vector3(h, 0, v);

        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }

        Vector3 velocity = moveDirection.normalized * moveSpeed;
        playerRb.linearVelocity = new Vector3(velocity.x, playerRb.linearVelocity.y, velocity.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & isEnemy) != 0)
            enemiesInRange.Add(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & isEnemy) != 0)
            enemiesInRange.Remove(other.transform);
    }

    private void CheckAndAttackClosestEnemy()
    {
        if (enemiesInRange.Count == 0)
        {
            isAttack = false;
            return;
        }

        Transform bestTarget = null;
        float minDistance = Mathf.Infinity;

        foreach (var enemy in enemiesInRange)
        {
            if (enemy == null)
                continue;

            Vector3 direction = (enemy.position - transform.position);
            float distance = direction.magnitude;

            if (distance > attackRadius)
                continue;

            direction.Normalize();

            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, attackRadius, isEnemy | isWall))
            {
                if (hit.transform == enemy)
                {
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        bestTarget = enemy;
                    }
                }
            }
        }

        if (bestTarget != null)
        {
            isAttack = true;
            Debug.Log("Атака");
        }
        else
        {
            isAttack = false;
        }
    }
}
