using UnityEngine;

namespace PerAsperaAdLuna
{
    public class Moon : MonoBehaviour
    {
        public float rotationSpeed = -1.5f;

        private void Update()
        {
            transform.rotation *= Quaternion.Euler(0, rotationSpeed*Time.deltaTime, 0);
        }
    }
}
