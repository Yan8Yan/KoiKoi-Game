using System.Collections;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("References")]
    [SerializeField] private RoundManager roundManager;

    [Header("Round UI")]
    [SerializeField] private GameObject roundBannerPanel;
    [SerializeField] private TextMeshProUGUI roundText;

    [Header("Match End UI")]
    [SerializeField] private GameObject matchEndPanel;
    [SerializeField] private TextMeshProUGUI matchEndText;

    [Header("KoiKoi UI")]
    [SerializeField] private GameObject koiKoiPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // стартовое состояние UI
        if (roundBannerPanel != null)
            roundBannerPanel.SetActive(false);

        if (matchEndPanel != null)
            matchEndPanel.SetActive(false);

        if (koiKoiPanel != null)
            koiKoiPanel.SetActive(false);
    }

    public void ShowKoiKoi()
    {
        StartCoroutine(ShowPanel(koiKoiPanel, 0.2f));
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
        roundManager.FinishRound();
        HideKoiKoi();
    }

    public IEnumerator ShowRoundBanner(string text, float duration = 2f)
    {
        roundText.text = text;

        yield return ShowPanel(roundBannerPanel, 0.3f);

        yield return new WaitForSecondsRealtime(duration);

        HideRoundText();
    }

    public void ShowRoundText(string text)
    {
        roundBannerPanel.SetActive(true);
        roundText.text = text;
    }

    public void HideRoundText()
    {
        roundBannerPanel.SetActive(false);
    }

    public IEnumerator ShowMatchEnd(string result)
    {
        matchEndText.text = result;

        yield return ShowPanel(matchEndPanel, 0.5f);
    }


    private IEnumerator ShowPanel(GameObject panel, float duration)
    {
        panel.SetActive(true);

        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = panel.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        panel.transform.localScale = Vector3.one * 1.1f;

        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;

            float k = t / duration;

            cg.alpha = k;

            panel.transform.localScale = Vector3.Lerp(
                Vector3.one * 1.1f,
                Vector3.one,
                k
            );

            yield return null;
        }

        cg.alpha = 1f;
        panel.transform.localScale = Vector3.one;
    }
}