using Character;
using UnityEngine;
using UnityEngine.UI;

namespace Debug
{
    public class VelocityText : MonoBehaviour
    {
        
        public Controller controller;

        private Text text;
        void Start()
        {
            text = GetComponent<Text>();
        }
        
        void Update()
        {
            Vector3 velocity = controller.GetVelocity();
            text.text = $"VEL: {velocity.magnitude}, {velocity} ";
        }
    }
}
