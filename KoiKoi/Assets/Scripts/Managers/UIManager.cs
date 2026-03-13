using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private RoundManager roundManager;

    public GameObject koiKoiPanel;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowKoiKoi()
    {
        koiKoiPanel.SetActive(true);
    }

    public void HideKoiKoi()
    {
        koiKoiPanel.SetActive(false);
    }

    public void OnKoiKoiPressed()
    {
        Debug.Log("Player chose KOI KOI");
       
        HideKoiKoi();
    }

    public void OnStopPressed()
    {
        Debug.Log("Player chose STOP");
        roundManager.RestartRound();
        HideKoiKoi();
    }
}