using UnityEngine;

namespace PerAsperaAdLuna
{
    public class Earth : MonoBehaviour
    {
        public float rotationSpeed = -1;
        public Transform canaveral;
        public Renderer earthRenderer;
        public float cloudSpeed = 0.5f;

        private MaterialPropertyBlock propertyBlock;

        private void Awake()
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        private void Update()
        {
            transform.rotation *= Quaternion.Euler(0, rotationSpeed*Time.deltaTime, 0);

            propertyBlock.SetVector("_Clouds_ST", new Vector4(1, 1, Time.time*cloudSpeed/100, 0));
            earthRenderer.SetPropertyBlock(propertyBlock);
        }
    }
}
