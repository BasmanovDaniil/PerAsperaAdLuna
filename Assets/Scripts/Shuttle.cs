using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Random = UnityEngine.Random;

namespace PerAsperaAdLuna
{
    public class Shuttle : MonoBehaviour
    {
        public event Action onReset = () => { };

        [SerializeField]
        private Transform flagPrefab;
        [SerializeField]
        private Transform moon;
        [SerializeField]
        private Earth earth;

        public float mass { get; private set; }
        public float normalizedMass { get { return Mathf.InverseLerp(minShuttleMass, maxShuttleMass, mass); } }
        public float velocity { get; private set; }
        public float normalizedVelocity { get { return Mathf.InverseLerp(minShuttleVelocity, maxShuttleVelocity, velocity); } }
        public bool launched { get; private set; }

        private const float defaultShuttleMass = 1;
        private const float minShuttleMass = 0.01f;
        private const float maxShuttleMass = 4;
        private const float defaultShuttleVelocity = 1800;
        private const float minShuttleVelocity = 0;
        private const float maxShuttleVelocity = 5000;
        private const float moonMass = 1000;
        private const float earthMass = 1000;
        private const float homeSpaceDistance = 100;
        private const float darkSpaceDistance = 120;
        private const float resetInterval = 2;

        private Rigidbody rb;
        private LineRenderer lineRenderer;
        private PostProcessVolume postProcessVolume;
        private List<Vector3> trajectory = new List<Vector3>();
        private bool planted;
        private float? resetTime;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            lineRenderer = GetComponent<LineRenderer>();
            postProcessVolume = GetComponent<PostProcessVolume>();

            mass = defaultShuttleMass;
            velocity = defaultShuttleVelocity;
            ResetShuttle();
        }

        private void Update()
        {
            float distance = (transform.position - earth.transform.position).magnitude;
            if (distance > homeSpaceDistance)
            {
                float percent = Mathf.Clamp01(Mathf.InverseLerp(homeSpaceDistance, darkSpaceDistance, distance));
                SetNoise(percent);

                if (!resetTime.HasValue)
                {
                    resetTime = Time.time + resetInterval;
                    SetNoise(1);
                }
            }

            if (resetTime.HasValue && Time.time > resetTime)
            {
                ResetShuttle();
            }
        }

        private void FixedUpdate()
        {
            if (launched)
            {
                Vector3 toEarth = earth.transform.position - transform.position;
                Vector3 toMoon = moon.position - transform.position;
                rb.AddForce(toEarth*rb.mass*earthMass*Time.deltaTime/toEarth.sqrMagnitude);
                rb.AddForce(toMoon*rb.mass*moonMass*Time.deltaTime/toMoon.sqrMagnitude);

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
                var rotation = Quaternion.FromToRotation(Vector3.up, collision.contacts[0].normal)*
                               Quaternion.Euler(0, Random.value*360, 0);
                Transform flag = Instantiate(flagPrefab, collision.contacts[0].point, rotation);
                flag.parent = collision.transform;
                planted = true;
            }
        }

        public void ResetShuttle()
        {
            launched = false;
            planted = false;
            resetTime = null;

            transform.parent = earth.canaveral;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            rb.mass = mass;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep();

            trajectory.Clear();
            lineRenderer.positionCount = 0;
            SetNoise(0);

            onReset();
        }

        public void Launch()
        {
            launched = true;
            planted = false;
            resetTime = null;

            transform.parent = null;
            rb.AddExplosionForce(velocity, earth.transform.position, 8, 0);
        }

        public void SetMass(float value)
        {
            mass = Mathf.Lerp(minShuttleMass, maxShuttleMass, value);
        }

        public void SetVelocity(float value)
        {
            velocity = Mathf.Lerp(minShuttleVelocity, maxShuttleVelocity, value);
        }

        private void SetNoise(float value)
        {
            postProcessVolume.weight = value;
        }
    }
}
