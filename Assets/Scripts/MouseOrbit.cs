using UnityEngine;

namespace PerAsperaAdLuna
{
    public class MouseOrbit : MonoBehaviour
    {
        [SerializeField]
        private Transform target;
        [SerializeField]
        private float distance = 10;
        [SerializeField]
        private float xSpeed = 250;
        [SerializeField]
        private float ySpeed = 250;
        [SerializeField]
        private float yMinLimit = -80;
        [SerializeField]
        private float yMaxLimit = 80;

        private float x;
        private float y;

        private void Start()
        {
            var angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            x += Input.GetAxis("Mouse X")*xSpeed*Time.deltaTime;
            y -= Input.GetAxis("Mouse Y")*ySpeed*Time.deltaTime;

            if (y < -360)
            {
                y += 360;
            }
            if (y > 360)
            {
                y -= 360;
            }
            y = Mathf.Clamp(y, yMinLimit, yMaxLimit);

            var rotation = Quaternion.Euler(y, x, 0);
            var position = rotation*new Vector3(0.0f, 0.0f, -distance) + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }
}
