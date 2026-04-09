using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private MainDoor MD;

    public void TakeDamegeDoor(float damage)
    {
        MD.TakeDamage(damage);
    }

  
}