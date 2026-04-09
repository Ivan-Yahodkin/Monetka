using UnityEngine;

public class MainDoor : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private float hp = 100f;
    [SerializeField] private Transform firstRotPivot;
    [SerializeField] private Transform secondRotPivot;
    [SerializeField] private Transform player;

    [Header("Open Settings")]
    [SerializeField] private float openRadius = 3f;
    [SerializeField] private float openSpeed = 2f;

    private Quaternion firstClosedRot;
    private Quaternion secondClosedRot;

    private Quaternion firstOpenRot;
    private Quaternion secondOpenRot;

    private bool isOpen;

    private void Start()
    {
        firstClosedRot = firstRotPivot.localRotation;
        secondClosedRot = secondRotPivot.localRotation;

        firstOpenRot = Quaternion.Euler(0f, -90f, 0f);
        secondOpenRot = Quaternion.Euler(0f, 90f, 0f);
    }

    private void Update()
    {
        CheckPlayerDistance();
        AnimateDoor();
    }

    private void CheckPlayerDistance()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        isOpen = distance <= openRadius;
    }

    private void AnimateDoor()
    {
        if (isOpen)
        {
            firstRotPivot.localRotation = Quaternion.Lerp(
                firstRotPivot.localRotation,
                firstOpenRot,
                Time.deltaTime * openSpeed);

            secondRotPivot.localRotation = Quaternion.Lerp(
                secondRotPivot.localRotation,
                secondOpenRot,
                Time.deltaTime * openSpeed);
        }
        else
        {
            firstRotPivot.localRotation = Quaternion.Lerp(
                firstRotPivot.localRotation,
                firstClosedRot,
                Time.deltaTime * openSpeed);

            secondRotPivot.localRotation = Quaternion.Lerp(
                secondRotPivot.localRotation,
                secondClosedRot,
                Time.deltaTime * openSpeed);
        }
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, openRadius);
    }
}