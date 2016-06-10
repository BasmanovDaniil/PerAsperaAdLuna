using UnityEngine;

public class Earth : MonoBehaviour
{
    public float cloudSpeed = -0.5f;
    public float rotationSpeed = 0.5f;

    private Transform tr;
    private Material mat;

    void Awake()
    {
        tr = transform;
        mat = GetComponent<Renderer>().material;
    }

	void Update ()
    {
        tr.rotation *= Quaternion.Euler(0, 0, rotationSpeed * Time.deltaTime);
        mat.SetTextureOffset("_Clouds", new Vector2(Time.time * cloudSpeed / 100, 0));
	}
}
