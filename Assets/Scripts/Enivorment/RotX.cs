using UnityEngine;
public class RotX : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 180f;

    private void Update()
    {
        transform.Rotate(-rotationSpeed * Time.deltaTime, 0f, 0f);
    }
}
