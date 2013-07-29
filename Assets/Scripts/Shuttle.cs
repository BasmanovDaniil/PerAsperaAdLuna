using System.Collections.Generic;
using UnityEngine;

// ����, ������� ��������� �� ���������� ����� � ����,
// ���������� ��������� ��� ������� � ��������� ����� � ��������,
// ������ ���������� ����� � ������ ����� ��� ������������
public class Shuttle : MonoBehaviour
{
    public Transform flagPrefab;
    // ���������� � �������� ��������
    public Transform canaveral;
    // ���������� ���� � �����
    public Transform moon;
    public Transform earth;
    // ��������� �������� � ����� �����, ���� � �����
    public float velocity = 1800;
    public float shuttleMass = 1;
    public float moonMass = 1000;
    public float earthMass = 1000;
    // ������ �� ������ � ����� �� ������� ������
    public NoiseEffect noise;

    private Transform tr;
    private Rigidbody rb;
    private LineRenderer lineRenderer;
    private List<Vector3> trajectory;
    private bool launched;
    private bool planted;
    private Vector3 toMoon;
    private Vector3 toEarth;

    void Awake()
    {
        tr = transform;
        rb = rigidbody;
        trajectory = new List<Vector3>();
        lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Reset();
            Launch();
        }

        // �������� ������������� � �������� ������ ������, ���� ������� ������ �� �����
        if ((tr.position - earth.position).sqrMagnitude > 100*100)
        {
            Invoke("Reset", 2);
            noise.grainIntensityMin = 1;
            noise.grainIntensityMax = 1;
        }
	}

    void FixedUpdate()
    {
        if (launched)
        {
            toEarth = earth.position - tr.position;
            toMoon = moon.position - tr.position;
            rb.AddForce(toEarth * rb.mass * earthMass * Time.deltaTime / toEarth.sqrMagnitude);
            rb.AddForce(toMoon * rb.mass * moonMass * Time.deltaTime / toMoon.sqrMagnitude);
            // ��������� ����� ����������, ���� ��� �����, �� ��� �� ��������� ����
            if (!planted)
            {
                trajectory.Add(tr.position);
                lineRenderer.SetVertexCount(trajectory.Count);
                lineRenderer.SetPosition(trajectory.Count - 1, trajectory[trajectory.Count - 1]);
            }
        }
    }


    /// <summary>
    ///  ���������� ���� �� ��������� ��������, �������� �������� � �����,
    ///  ������ ���������� � ��������� ���
    /// </summary>
    void Reset()
    {
        tr.position = canaveral.position;
        tr.rotation = canaveral.rotation;
        rb.mass = shuttleMass;
        rb.velocity = Vector3.zero;
        planted = false;
        launched = false;
        trajectory.Clear();
        lineRenderer.SetVertexCount(0);
        noise.grainIntensityMin = 0.2f;
        noise.grainIntensityMax = 0.2f;
    }

    /// <summary>
    ///  ��������� ����
    /// </summary>
    void Launch()
    {
        rb.AddExplosionForce(velocity, earth.position, 8, 0);
        launched = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!planted)
        {
            // ����������� ������� �� ������� � ������������� ���� � ��������� �������
            var rotation = Quaternion.FromToRotation(Vector3.up, collision.contacts[0].normal)*
                           Quaternion.Euler(0, Random.value*360, 0);
            var flag = Instantiate(flagPrefab, collision.contacts[0].point, rotation) as Transform;
            // ����������� ����, ����� �� �������� ������ � ������� ������������
            flag.parent = collision.transform;
            planted = true;
        }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width/2 - 30, 20, 60, 40), "����"))
        {
            Reset();
            Launch();
        }
        shuttleMass = GUI.HorizontalSlider(new Rect(20, 20, Screen.width / 2 - 60, 40), shuttleMass, 0.01f, 4.0f);
        GUI.Label(new Rect(20, 35, Screen.width / 2 - 60, 20), "�����: " + shuttleMass.ToString("F2"));
        velocity = GUI.HorizontalSlider(new Rect(Screen.width / 2 + 40, 20, Screen.width / 2 - 60, 40), velocity, 0, 5000);
        GUI.Label(new Rect(Screen.width / 2 + 40, 35, Screen.width / 2 - 60, 20), "��������� ��������: " + velocity.ToString("F0"));
    }
}


