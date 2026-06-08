using UnityEngine;
using UnityEngine.UI;
using Platformer.Mechanics;

namespace  Platformer.UI
{
    public class HeartsUI : MonoBehaviour
    {
        public Image heart1;
        public Image heart2;
        public Image heart3;

        public Sprite heartFull;
        public Sprite heartEmpty;

        private Health playerHealth;

        void Start()
        {
            playerHealth = FindObjectOfType<Health>();
        }

        void Update()
        {
            // récupère les HP actuels via reflection
            var currentHP = playerHealth.IsAlive ? playerHealth.CurrentHP : 0;

            heart1.sprite = currentHP >= 3 ? heartFull : heartEmpty;
            heart2.sprite = currentHP >= 2 ? heartFull : heartEmpty;
            heart3.sprite = currentHP >= 1 ? heartFull : heartEmpty;
        }
    }    
}

