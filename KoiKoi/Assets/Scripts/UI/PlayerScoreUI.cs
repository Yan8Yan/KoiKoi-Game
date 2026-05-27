using UnityEngine;
using TMPro;

public class PlayerScoreUI : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI hikariText;
    [SerializeField] private TextMeshProUGUI kasuText;
    [SerializeField] private TextMeshProUGUI taneText;
    [SerializeField] private TextMeshProUGUI tanzakuText;

    public void UpdateUI(int hikari, int kasu, int tane, int tanzaku)
    {
        hikariText.text = "Hikari: " + hikari;
        kasuText.text = "Kasu: " + kasu;
        taneText.text = "Tane: " + tane;
        tanzakuText.text = "Tanzaku: " + tanzaku;
    }
}