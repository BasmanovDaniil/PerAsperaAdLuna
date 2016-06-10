using System.Collections.Generic;
using UnityEngine;

// Шатл, который реагирует на притяжение Земли и Луны,
// отображает интерфейс для запуска и настройки массы и скорости,
// рисует траекторию полёта и ставит флаги при столкновении
public class Shuttle : MonoBehaviour
{
    public Transform flagPrefab;
    // Координаты и вращение респавна
    public Transform canaveral;
    // Координаты Луны и Земли
    public Transform moon;
    public Transform earth;
    // Стартовая скорость и массы шатла, Луны и Земли
    public float velocity = 1800;
    public float shuttleMass = 1;
    public float moonMass = 1000;
    public float earthMass = 1000;
    // Ссылка на скрипт с шумом на главной камере
    public NoiseEffect noise;

    private Transform tr;
    private Rigidbody rb;
    private LineRenderer lineRenderer;
    private List<Vector3> trajectory;
    private bool launched;
    private bool planted;
    private Vector3 toMoon;
    private Vector3 toEarth;

    private void Awake()
    {
        tr = transform;
        rb = GetComponent<Rigidbody>();
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

        // Повышаем зашумленность и включаем таймер сброса, если слишком далеко от Земли
        if ((tr.position - earth.position).sqrMagnitude > 100*100)
        {
            Invoke("Reset", 2);
            noise.grainIntensityMin = 1;
            noise.grainIntensityMax = 1;
        }
    }

    private void FixedUpdate()
    {
        if (launched)
        {
            toEarth = earth.position - tr.position;
            toMoon = moon.position - tr.position;
            rb.AddForce(toEarth*rb.mass*earthMass*Time.deltaTime/toEarth.sqrMagnitude);
            rb.AddForce(toMoon*rb.mass*moonMass*Time.deltaTime/toMoon.sqrMagnitude);
            // Обновляем точки траектории, если уже летим, но ещё не поставили флаг
            if (!planted)
            {
                trajectory.Add(tr.position);
                lineRenderer.SetVertexCount(trajectory.Count);
                lineRenderer.SetPosition(trajectory.Count - 1, trajectory[trajectory.Count - 1]);
            }
        }
    }


    /// <summary>
    ///  Возвращает шатл на стартовую площадку, обнуляет скорость и флаги,
    ///  чистит траекторию и уменьшает шум
    /// </summary>
    private void Reset()
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
    ///  Запускает шатл
    /// </summary>
    private void Launch()
    {
        rb.AddExplosionForce(velocity, earth.position, 8, 0);
        launched = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!planted)
        {
            // Выравниваем флагшто по нормали и разворачиваем флаг в случайную сторону
            var rotation = Quaternion.FromToRotation(Vector3.up, collision.contacts[0].normal)*
                           Quaternion.Euler(0, Random.value*360, 0);
            var flag = (Transform) Instantiate(flagPrefab, collision.contacts[0].point, rotation);
            // Прикрепляем флаг, чтобы он двигался вместе с жертвой столкновения
            flag.parent = collision.transform;
            planted = true;
        }
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width/2 - 30, 20, 60, 40), "Пуск"))
        {
            Reset();
            Launch();
        }
        shuttleMass = GUI.HorizontalSlider(new Rect(20, 20, Screen.width/2 - 60, 40), shuttleMass, 0.01f, 4.0f);
        GUI.Label(new Rect(20, 35, Screen.width/2 - 60, 20), "Масса: " + shuttleMass.ToString("F2"));
        velocity = GUI.HorizontalSlider(new Rect(Screen.width/2 + 40, 20, Screen.width/2 - 60, 40), velocity, 0, 5000);
        GUI.Label(new Rect(Screen.width/2 + 40, 35, Screen.width/2 - 60, 20), "Стартовая скорость: " + velocity.ToString("F0"));
    }
}