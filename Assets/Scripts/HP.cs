using UnityEngine;
using System.Collections;

public class HP : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private float CurrentHP = 100f;
    [SerializeField] private float HPmax = 100f;
    [SerializeField] private float HPmin = 0f;

    [Header("Type")]
    [SerializeField] private bool isPl;
    [SerializeField] private bool isEnemy;
    [SerializeField] private bool isBase;

    [Header("Damage Control")]
    [SerializeField] private bool passDamager;
    [SerializeField] private float damageCooldown = 3f;

    [Header("Player Regeneration")]
    [SerializeField] private float healDelay = 5f;
    [SerializeField] private float healAmount = 2f;
    [SerializeField] private float healTick = 1f;

    private Coroutine regenCoroutine;
    private Coroutine damageCoroutine;

    private void Start()
    {
        CurrentHP = HPmax;
    }

    public void HpHealth(float plus)
    {
        CurrentHP += plus;
        CurrentHP = Mathf.Clamp(CurrentHP, HPmin, HPmax);
    }

    public void HpDamage(float minus)
    {
        CurrentHP -= minus;
        CurrentHP = Mathf.Clamp(CurrentHP, HPmin, HPmax);

        passDamager = true;

        if (damageCoroutine != null)
            StopCoroutine(damageCoroutine);

        damageCoroutine = StartCoroutine(DamageOff());

        if (isPl)
        {
            if (regenCoroutine != null)
                StopCoroutine(regenCoroutine);

            regenCoroutine = StartCoroutine(PlayerRegen());
        }

        if (CurrentHP <= HPmin)
        {
            Dead();
        }
    }

    private IEnumerator DamageOff()
    {
        yield return new WaitForSeconds(damageCooldown);
        passDamager = false;
    }

    private IEnumerator PlayerRegen()
    {
        yield return new WaitForSeconds(healDelay);

        while (!passDamager && CurrentHP < HPmax)
        {
            HpHealth(healAmount);
            yield return new WaitForSeconds(healTick);
        }
    }

    private void Dead()
    {
        if (isPl) PlayerDead();
        if (isEnemy) EnemyDead();
        if (isBase) BaseDead();
    }

    private void PlayerDead()
    {
        Debug.Log("Player dead");
        Destroy(gameObject);
    }

    private void EnemyDead()
    {
        Debug.Log("Enemy dead");
        Destroy(gameObject);
    }

    private void BaseDead()
    {
        Debug.Log("Base destroyed");
        Destroy(gameObject);
    }
}
