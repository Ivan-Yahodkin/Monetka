using UnityEngine;
using System.Collections;

public class Bow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject ArrowInBow;
    [SerializeField] private GameObject arrowPrefab;

    [Header("Settings")]
    [SerializeField] private float flightTime = 1.5f;
    [SerializeField] private GameObject bloodDecal;

    private GameObject currentArrow;
    private GameObject targetPoint;

    private bool isFollowingString = false;

    private static readonly int ShootTrigger = Animator.StringToHash("Shoot");

    public void Shoot(GameObject target)
    {
        targetPoint = target;
        animator.SetTrigger(ShootTrigger);
    }

    // 🏹 Спавн стрелы (чисто визуальной)
    public void SpawnArrow()
    {
        ArrowInBow.SetActive(true);
        isFollowingString = true;
    }

    private void Update()
    {
        if (isFollowingString && currentArrow != null)
        {
            // 👉 просто повторяем тетиву
            currentArrow.transform.position = muzzle.position;
            currentArrow.transform.rotation = muzzle.rotation;
        }
    }

    // 💥 момент выстрела (конец анимации)
    public void OnAnimationFinished()
    {
        ArrowInBow.SetActive(false);
        currentArrow = Instantiate(arrowPrefab, muzzle.position, muzzle.rotation);
        if (currentArrow != null)
        {
            isFollowingString = false;

            StartCoroutine(FlyToTarget(currentArrow, targetPoint.transform.position));

            currentArrow = null;
        }
    }

    private IEnumerator FlyToTarget(GameObject arrow, Vector3 target)
    {
        Vector3 startPos = arrow.transform.position;
        float time = 0f;

        while (time < flightTime && arrow != null)
        {
            time += Time.deltaTime;
            float t = time / flightTime;

            arrow.transform.position = Vector3.Lerp(startPos, target, t);
            arrow.transform.LookAt(target);

            yield return null;
        }

        while (arrow != null)
        {
            arrow.transform.position = Vector3.MoveTowards(
                arrow.transform.position,
                target,
                (Vector3.Distance(startPos, target) / flightTime) * Time.deltaTime
            );

            arrow.transform.LookAt(target);

            // 👉 проверка попадания
            if (Vector3.Distance(arrow.transform.position, target) < 0.05f)
            {
                arrow.transform.position = target;
               
                if (bloodDecal != null)
                {
                    Quaternion rot = Quaternion.LookRotation((target - startPos).normalized);
                    GameObject bloodInstance = Instantiate(bloodDecal, target, rot);
                    Destroy(bloodInstance, 1f);
                }
                
                Destroy(arrow); // 💥 уничтожаем сразу при попадании
                yield break;
            }

            yield return null;
        }
    }
}