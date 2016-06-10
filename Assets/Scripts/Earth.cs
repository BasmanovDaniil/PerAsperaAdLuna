using UnityEngine;

public class Earth : MonoBehaviour
{
    public float cloudSpeed = -0.5f;
    public float rotationSpeed = 0.5f;

    private Transform tr;
    private Material material;

    private void Awake()
    {
        tr = transform;
        material = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        tr.rotation *= Quaternion.Euler(0, 0, rotationSpeed*Time.deltaTime);
        material.SetTextureOffset("_Clouds", new Vector2(Time.time*cloudSpeed/100, 0));
    }
}