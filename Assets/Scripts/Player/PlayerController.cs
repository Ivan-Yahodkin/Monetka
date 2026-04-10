using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class PlayerController : MonoBehaviour
{
    [SerializeField] private DynamicJoystick fixedJoystick;

    [Header("Move Speed")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("State")]
    [SerializeField] private bool isMining;

    [Header("Layers")]
    [SerializeField] private LayerMask isMine, isEnemy, isWall;

    [Header("Distance")]
    [SerializeField] private float miningRadius = 3f;
    [SerializeField] private float attackRadius = 13f;

    [Header("Balance")]
    [SerializeField] private int countMoney = 0;
    [SerializeField] private float miningInterval = 1.5f;

    [Header("Attack")]
    [SerializeField] private float attackInterval = 1f;
    [SerializeField] private float attackDamage = 20f;

    [Header("Enemy Target")]
    [SerializeField] private Transform enemyPoint;

    private float attackTimer;
    private float miningTimer;

    private Rigidbody playerRb;

    [SerializeField] private List<Transform> enemiesInRange = new();

    private Ore currentMine;
    [SerializeField] private Transform currentEnemyTarget;

    private bool playerNearMine;
    [SerializeField] private bool nearEnemy;

    [SerializeField] private AnimationController Anim;



    private Transform aimPos;

    [Header("Items")]
    [SerializeField] private GameObject BowBack;
    [SerializeField] private GameObject PickaxeBack;
    [SerializeField] private GameObject Bow;
    [SerializeField] private GameObject Pickaxe;

    [SerializeField] private Animator animPickaxe;
    [SerializeField] private Animator animBow;
    [SerializeField] private Item CurrentItem;
    [SerializeField] private Bow BowSrcipt;

    

    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();

    }
    private void FixedUpdate()
    {
        Move();
    }
    private void Update()
    {
        // Поиск ближайшей шахты
        FindNearestMine();
        UpdateAimTorso();
       
        AnimationHand();
        UpdateEnemiesInRange();

        if (playerNearMine)
        {
            isMining = true;
            currentEnemyTarget = null;

            Bow.SetActive(false);
            BowBack.SetActive(true);

            PickaxeBack.SetActive(false);
            Pickaxe.SetActive(true);

            // Включаем анимацию сразу
            animPickaxe.SetBool("Mine", true);

            miningTimer += Time.deltaTime;
            if (miningTimer >= miningInterval)
            {
                if (currentMine != null)
                {
                    currentMine.HpDamage(30f);
                    countMoney += 30;
                }
                miningTimer = 0f;
            }

            return;
        }
        else
        {
            isMining = false;
            animPickaxe.SetBool("Mine", false);
            PickaxeBack.SetActive(true);
            Pickaxe.SetActive(false);
        }
    
        
        if (enemiesInRange.Count > 0) 
        {
            nearEnemy = true;
        }
        else
        {
            nearEnemy = false;
        }
        
        CheckAndAttackClosestEnemy();
        

    }
    private void UpdateAimTorso()
    {

        bool hasTarget = false;

        // 1️⃣ Приоритет: враг
        if (currentEnemyTarget !=null)
        {
         
            aimPos = currentEnemyTarget;
            hasTarget = true;
        }
        // 2️⃣ Шахта
        else if (currentMine != null)
        {
            
            aimPos = currentMine.transform;
            hasTarget = true;
        }

        if (hasTarget)
        {

            Anim.AimingOn(aimPos);
        }
        else
        {
            Anim.AimingOff();
        }

    }
    private void AnimationHand()
    {
       
        if (isMining)
        {  
        
            CurrentItem = Pickaxe.GetComponent<Item>();
            Anim.RigOnOff(1);
            Anim.moveHand(CurrentItem.TargetRight, CurrentItem.TargetLeft);
        }
        if (nearEnemy && !isMining)
        {
            BowBack.SetActive(false);
            Bow.SetActive(true);
            CurrentItem = Bow.GetComponent<Item>();
            Anim.RigOnOff(1);
            Anim.moveHand(CurrentItem.TargetRight, CurrentItem.TargetLeft);
        }
        else if(!isMining && !nearEnemy)
        {
            Bow.SetActive(false);
            BowBack.SetActive(true);
            Anim.RigOnOff(0);
        }
    }
    private void Move()
    {
        float h = fixedJoystick.Horizontal;
        float v = fixedJoystick.Vertical;

        Vector3 moveDirection = new Vector3(h, 0, v);

        // ДВИЖЕНИЕ — ТОЛЬКО ОТ ДЖОЙСТИКА
        Vector3 velocity = moveDirection.normalized * moveSpeed;

        playerRb.linearVelocity = new Vector3(
            velocity.x,
            playerRb.linearVelocity.y,
            velocity.z
        );
       
        //анимация
        if (velocity.magnitude > 0.1f)
        { Anim.isMove(true); }
        else
        { Anim.isMove(false);}
       
        // -------- ВРАЩЕНИЕ --------

        // Если есть враг — смотрим на врага
        if (currentEnemyTarget != null)
        {
            Vector3 lookDir = currentEnemyTarget.position - transform.position;
            lookDir.y = 0;

            if (lookDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDir);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * 100 * Time.deltaTime
                );
            }

            if (enemyPoint != null)
                enemyPoint.position = currentEnemyTarget.position;
        }
        else if(currentMine != null)
        {
            Vector3 lookDir = currentMine.transform.position - transform.position;
            lookDir.y = 0;

            if (lookDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDir);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * 100 * Time.deltaTime
                );
            }

         
        }
        else
        {
            // Если врага нет — смотрим по движению
            if (moveDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * 100 * Time.deltaTime
                );
            }
        }
    }

    private void UpdateEnemiesInRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius, isEnemy);

        enemiesInRange.Clear();

        foreach (var hit in hits)
        {
            enemiesInRange.Add(hit.transform);
        }
    }
    private void FindNearestMine()
    {
        Collider[] mines = Physics.OverlapSphere(transform.position, miningRadius, isMine);

        float minDistance = Mathf.Infinity;
        Ore nearestMine = null;

        foreach (var mineCol in mines)
        {
            Ore ore = mineCol.GetComponent<Ore>();

            if (ore == null) continue;

            float distance = Vector3.Distance(transform.position, mineCol.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestMine = ore;
            }
        }

        currentMine = nearestMine;
        playerNearMine = currentMine != null;
    }

    private void CheckAndAttackClosestEnemy()
    {
        
        attackTimer += Time.deltaTime;

        currentEnemyTarget = null;

        foreach (var enemy in enemiesInRange)
        {
            if (enemy == null) continue;

            // ❗ ВАЖНО: проверка слоя
            if (((1 << enemy.gameObject.layer) & isEnemy) == 0)
                continue;

            float distance = Vector3.Distance(transform.position, enemy.position);
            if (distance > attackRadius) continue;

            Vector3 dir = (enemy.position - transform.position).normalized;

            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, attackRadius, isEnemy | isWall))
            {
                if (hit.transform != enemy)
                    continue;
            }

            currentEnemyTarget = enemy;
            break;
        }

        // 👉 стрельба
        if (currentEnemyTarget != null && attackTimer >= attackInterval)
        {
        
          
            BowSrcipt.Shoot(currentEnemyTarget.gameObject);

            HP hp = currentEnemyTarget.GetComponent<HP>();
            if (hp != null)
                hp.HpDamage(attackDamage);
           
            attackTimer = 0f;
        }
       
    }
    private void OnDrawGizmosSelected()
    {
        // Радиус поиска шахт
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, miningRadius);

        // Радиус атаки врагов
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        // Линия к текущему врагу
        if (currentEnemyTarget != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, currentEnemyTarget.position);
            Gizmos.DrawWireSphere(currentEnemyTarget.position, 0.3f);
        }

        // Линия к текущей шахте
        if (currentMine != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, currentMine.transform.position);
            Gizmos.DrawWireSphere(currentMine.transform.position, 0.3f);
        }
    }

}