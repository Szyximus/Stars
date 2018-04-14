using UnityEngine;

namespace Enviroment.Border
{
    public class StarRotation : MonoBehaviour {
        public float Speed = 5;
        void Update()
        {
            // Rotate the object around its local X axis at 1 degree per second * speed variable
            transform.Rotate(-1 * Vector3.up *Speed * Time.deltaTime);
            transform.Rotate(Vector3.forward * Speed * Time.deltaTime * 10 * Mathf.Sin(Time.deltaTime/10));
        }
    }
}