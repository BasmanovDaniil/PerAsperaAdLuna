using UnityEngine;
using UnityEngine.UI;

public class LaunchingUI : MonoBehaviour
{
    [SerializeField]
    private Shuttle shuttle;
    [SerializeField]
    private Button launchButton;
    [SerializeField]
    private Slider massSlider;
    [SerializeField]
    private Text massText;
    [SerializeField]
    private Slider velocitySlider;
    [SerializeField]
    private Text velocityText;

    private void Awake()
    {
        launchButton.onClick.AddListener(() => shuttle.Launch());
        massSlider.onValueChanged.AddListener(SetMass);
        velocitySlider.onValueChanged.AddListener(SetVelocity);
    }

    private void Start()
    {
        massSlider.value = shuttle.normalizedMass;
        UpdateMassText();
        velocitySlider.value = shuttle.normalizedVelocity;
        UpdateVelocityText();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            shuttle.Launch();
        }
    }

    private void SetMass(float value)
    {
        shuttle.SetMass(value);
        UpdateMassText();
    }

    private void UpdateMassText()
    {
        massText.text = string.Format("Mass: {0:F2}", shuttle.mass);
    }

    private void SetVelocity(float value)
    {
        shuttle.SetVelocity(value);
        UpdateVelocityText();
    }

    private void UpdateVelocityText()
    {
        velocityText.text = string.Format("Velocity: {0:F0}", shuttle.velocity);
    }
}
