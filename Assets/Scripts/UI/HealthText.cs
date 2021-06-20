using Damages;
using Preferences;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthText : MonoBehaviour
    {

        private DamageableObject playerDamageableObject;
        private Text text;
        private int maxHealth;
        void Start()
        {
            playerDamageableObject = GameObject.FindGameObjectWithTag(Settings.Tags.Player).GetComponent<DamageableObject>();
            text = GetComponent<Text>();
            maxHealth = playerDamageableObject.GetHealth();
        }

        void Update()
        {
            if(!playerDamageableObject)
                return;
            text.text = playerDamageableObject.health + "/" + maxHealth;
        }
    }
}
