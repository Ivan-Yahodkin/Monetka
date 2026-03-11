using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private float hp = 100f;

    public void TakeDamage(float damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
