using Preferences;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

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
            maxHealth = playerDamageableObject.health;
        }

        void Update()
        {
            text.text = playerDamageableObject.health + "/" + maxHealth;
        }
    }
}
