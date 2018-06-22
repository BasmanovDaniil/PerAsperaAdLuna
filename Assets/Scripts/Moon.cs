using UnityEngine;

public class Moon : MonoBehaviour
{
    public float rotationSpeed = 1f;

    private Transform tr;

    private void Awake()
    {
        tr = transform;
    }

    private void Update()
    {
        tr.rotation *= Quaternion.Euler(0, rotationSpeed*Time.deltaTime, 0);
    }
}
