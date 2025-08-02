using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager instance;

    [SerializeField] private float delayBeforeShowing = .3f;

    [SerializeField] private float textPaddingSizeX = 10f;
    [SerializeField] private float textPaddingSizeY = 10f;

    [SerializeField] private float tooltipDownVerticalSpacing = 3f;

    private TextMeshProUGUI tooltipText;
    private RectTransform backgroundTransform;

    private Coroutine coroutine;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);

        tooltipText = GetComponentInChildren<TextMeshProUGUI>();
        backgroundTransform = GetComponentInChildren<Image>().GetComponent<RectTransform>();
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    private void Update()
    {
        CalculateTooltipPosition();
        if (InputManager.Instance.rightClick.RELEASE || InputManager.Instance.click.RELEASE) _HideTooltip();
    }

    private void CalculateTooltipPosition()
    {
        Vector2 tooltipPosition = InputManager.Instance.point;

        if (InputManager.Instance.point.x > Screen.width * 2 / 3)
        {
            tooltipPosition.x -= backgroundTransform.sizeDelta.x;
        }

        if (InputManager.Instance.point.y > Screen.height * 2 / 3)
        {
            tooltipPosition.y -= backgroundTransform.sizeDelta.y + tooltipDownVerticalSpacing;
        }

        transform.position = tooltipPosition;
    }

    private void _ShowTooltip(string text)
    {
        coroutine = StartCoroutine(ShowTooltipAfterDelay(text));
    }

    private void ActualShowTooltip(string text)
    {
        tooltipText.text = text;
        var preferredSize = tooltipText.GetPreferredValues();
        backgroundTransform.sizeDelta = new Vector2(Mathf.Min(preferredSize.x, tooltipText.rectTransform.sizeDelta.x) + textPaddingSizeX * 2, preferredSize.y + textPaddingSizeY * 2);
        CalculateTooltipPosition();
    }

    private void _HideTooltip()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        tooltipText.text = "";
        backgroundTransform.sizeDelta = Vector2.zero;
    }

    private IEnumerator ShowTooltipAfterDelay(string text)
    {
        yield return new WaitForSeconds(delayBeforeShowing);

        ActualShowTooltip(text);
    }

    public static void ShowTooltip(string text)
    {
        instance._ShowTooltip(text);
    }

    public static void HideTooltip()
    {
        instance._HideTooltip();
    }
}
