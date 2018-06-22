using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

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
    public PostProcessProfile postProcessProfile;

    private Rigidbody rb;
    private LineRenderer lineRenderer;
    private List<Vector3> trajectory = new List<Vector3>();
    private bool launched;
    private bool planted;
    private bool waitingForReset;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
        ResetShuttle();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Launch();
        }

        // Повышаем зашумленность и включаем таймер сброса, если слишком далеко от Земли
        if (!waitingForReset && (transform.position - earth.position).sqrMagnitude > 100*100)
        {
            waitingForReset = true;
            StartCoroutine(ScheduleReset());
            SetNoise(true);
        }
    }

    private void FixedUpdate()
    {
        if (launched)
        {
            Vector3 toEarth = earth.position - transform.position;
            Vector3 toMoon = moon.position - transform.position;
            rb.AddForce(toEarth*rb.mass*earthMass*Time.deltaTime/toEarth.sqrMagnitude);
            rb.AddForce(toMoon*rb.mass*moonMass*Time.deltaTime/toMoon.sqrMagnitude);
            // Обновляем точки траектории, если уже летим, но ещё не поставили флаг
            if (!planted)
            {
                trajectory.Add(transform.position);
                lineRenderer.positionCount = trajectory.Count;
                lineRenderer.SetPosition(trajectory.Count - 1, trajectory[trajectory.Count - 1]);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!planted)
        {
            // Выравниваем флагшток по нормали и разворачиваем флаг в случайную сторону
            var rotation = Quaternion.FromToRotation(Vector3.up, collision.contacts[0].normal)*
                           Quaternion.Euler(0, Random.value*360, 0);
            Transform flag = Instantiate(flagPrefab, collision.contacts[0].point, rotation);
            // Прикрепляем флаг, чтобы он двигался вместе с жертвой столкновения
            flag.parent = collision.transform;
            planted = true;
        }
    }

    private void OnDestroy()
    {
        SetNoise(false);
    }

    /// <summary>
    ///  Возвращает шатл на стартовую площадку, обнуляет скорость и флаги,
    ///  чистит траекторию и уменьшает шум
    /// </summary>
    private void ResetShuttle()
    {
        waitingForReset = false;
        planted = false;
        launched = false;
        transform.parent = canaveral.parent;
        transform.localPosition = canaveral.localPosition;
        transform.localRotation = Quaternion.identity;
        rb.mass = shuttleMass;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep();
        trajectory.Clear();
        lineRenderer.positionCount = 0;
        SetNoise(false);
    }

    private IEnumerator ScheduleReset()
    {
        yield return new WaitForSeconds(2);
        ResetShuttle();
    }

    /// <summary>
    ///  Запускает шатл
    /// </summary>
    private void Launch()
    {
        ResetShuttle();
        transform.parent = null;
        rb.AddExplosionForce(velocity, earth.position, 8, 0);
        launched = true;
    }

    private void SetNoise(bool value)
    {
        var chromaticAberration = postProcessProfile.GetSetting<ChromaticAberration>();
        if (chromaticAberration != null)
        {
            chromaticAberration.enabled.value = value;
        }
        var vignette = postProcessProfile.GetSetting<Vignette>();
        if (vignette != null)
        {
            vignette.enabled.value = value;
        }
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width/2 - 30, 20, 60, 40), "Пуск"))
        {
            Launch();
        }
        shuttleMass = GUI.HorizontalSlider(new Rect(20, 20, Screen.width/2 - 60, 40), shuttleMass, 0.01f, 4.0f);
        GUI.Label(new Rect(20, 35, Screen.width/2 - 60, 20), "Масса: " + shuttleMass.ToString("F2"));
        velocity = GUI.HorizontalSlider(new Rect(Screen.width/2 + 40, 20, Screen.width/2 - 60, 40), velocity, 0, 5000);
        GUI.Label(new Rect(Screen.width/2 + 40, 35, Screen.width/2 - 60, 20),
            "Стартовая скорость: " + velocity.ToString("F0"));
    }
}
