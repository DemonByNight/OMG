using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OMG
{
    public class testAspect : MonoBehaviour
    {
        [SerializeField] private Camera Camera;
        [SerializeField] private float aspect;
        [SerializeField] bool isOn;

        private void Update()
        {
            if (!isOn)
                return;

            Camera.aspect = aspect;
        }

    }
}
