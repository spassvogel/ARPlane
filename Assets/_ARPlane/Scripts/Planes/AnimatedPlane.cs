using UnityEngine;
using System.Collections;

namespace UniversoAumentado.ARPlane.Planes
{
    public class AnimatedPlane : MonoBehaviour
    {
        public GameObject propellor;
        public GameObject propellorBlur;

        public bool engineOn;

        void Update()
        {
            if (engineOn)
            {
                propellor.SetActive(false);
                propellorBlur.SetActive(true);
                propellorBlur.transform.Rotate(1000 * Time.deltaTime, 0, 0);
            }
            else
            {
                propellor.SetActive(true);
                propellorBlur.SetActive(false);
            }
        }
    }
}