using UnityEngine;
using UnityEngine.UI;

namespace PerAsperaAdLuna
{
    public class UI : MonoBehaviour
    {
        [SerializeField]
        private Shuttle shuttle;
        [SerializeField]
        private Button launchResetButton;
        [SerializeField]
        private Text launchResetText;
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
            launchResetButton.onClick.AddListener(() =>
            {
                if (shuttle.launched)
                {
                    ResetShuttle();
                }
                else
                {
                    Launch();
                }
            });
            massSlider.onValueChanged.AddListener(SetMass);
            velocitySlider.onValueChanged.AddListener(SetVelocity);

            shuttle.onReset += SetLaunchText;
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
                if (shuttle.launched)
                {
                    ResetShuttle();
                }
                else
                {
                    Launch();
                }
            }
        }

        private void ResetShuttle()
        {
            shuttle.ResetShuttle();
            SetLaunchText();
        }

        private void Launch()
        {
            shuttle.Launch();
            SetResetText();
        }

        private void SetLaunchText()
        {
            launchResetText.text = "Launch";
        }

        private void SetResetText()
        {
            launchResetText.text = "Reset";
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
}
