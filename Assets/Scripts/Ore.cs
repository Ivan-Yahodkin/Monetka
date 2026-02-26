using UnityEngine;

public class Ore : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private float CurrentHP = 100f;
    [SerializeField] private float HPmax = 100f;
    [SerializeField] private float HPmin = 0f;

    private void Start()
    {
        CurrentHP = HPmax;
    }

    public void HpDamage(float minus)
    {
        CurrentHP -= minus;
        CurrentHP = Mathf.Clamp(CurrentHP, HPmin, HPmax);

        if (CurrentHP <= HPmin)
        {
            Destruction();
        }
    }

    private void Destruction()
    {
        Destroy(gameObject);
    }
}
