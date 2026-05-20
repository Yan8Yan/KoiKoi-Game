using UnityEngine;
using TMPro;

namespace KoiKoiProject
{
    public class VisualCounter 
    {
        public TextMeshProUGUI scoreText;
        private int count = 0;

        public void AddScore(int amount)
        {
            count += amount;
            UpdateScoreText();
        }

        void UpdateScoreText()
        {
            scoreText.text = "Score: " + count.ToString();
        }
    }

}