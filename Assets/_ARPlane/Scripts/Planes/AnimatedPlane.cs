using UnityEngine;
using System.Collections;

namespace UniversoAumentado.ARPlane.Planes
{
    public class AnimatedPlane : MonoBehaviour
    {
        public GameObject propellor;
        public GameObject propellorBlur;
        public Rigidbody body;

        [HideInInspector]
        [Range(0, 1)]
        public float speed;
        [Tooltip("This value or above represents the speed at which the propellors move at max power")]
        public float maxMagnitude = 100;

        private float baseRotateModifier = 1000;

        void Update()
        {
            if(body != null)
            {
                // Calculate 'speed' based on rigidbody magnitude. A value between 0 and 1
                speed = Mathf.Min(body.velocity.magnitude / maxMagnitude, 1);
            }

            propellor.transform.Rotate(baseRotateModifier * speed * Time.deltaTime, 0, 0);
            propellorBlur.transform.Rotate(baseRotateModifier * speed * Time.deltaTime, 0, 0);

            bool showBlur = (speed > .4);
            propellor.SetActive(!showBlur);
            propellorBlur.SetActive(showBlur);
        }
    }
}