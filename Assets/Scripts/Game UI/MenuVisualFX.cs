using UnityEngine;
using UnityEngine.UI;

public class MenuVisualFX : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform[] uiElements;

    [Header("Rotation Oscillation")]
    public float rotationSpeed = 0.8f;
    public float rotationAmount = 2f;

    [Header("Breathing Effect")]
    public float breathSpeed = 1.5f;
    public float breathAmount = 0.03f;

    [Header("Floating Effect")]
    public float floatSpeed = 1f;
    public float floatAmplitude = 4f;

    private Vector3[] baseScales;
    private Vector2[] basePositions;

    void Start()
    {
        if (uiElements != null && uiElements.Length > 0)
        {
            baseScales = new Vector3[uiElements.Length];
            basePositions = new Vector2[uiElements.Length];
            for (int i = 0; i < uiElements.Length; i++)
            {
                baseScales[i] = uiElements[i].localScale;
                basePositions[i] = uiElements[i].anchoredPosition;
            }
        }
    }

    void Update()
    {
        RotateUI();
        BreathingUI();
        FloatUI();
    }

    void RotateUI()
    {
        if (uiElements == null) return;

        float r = Mathf.Sin(Time.time * rotationSpeed) * rotationAmount;

        for (int i = 0; i < uiElements.Length; i++)
            uiElements[i].rotation = Quaternion.Euler(0, 0, r);
    }

    void BreathingUI()
    {
        if (uiElements == null) return;

        float s = (Mathf.Sin(Time.time * breathSpeed) + 1f) * 0.5f;
        s = 1f + s * breathAmount;

        for (int i = 0; i < uiElements.Length; i++)
            uiElements[i].localScale = baseScales[i] * s;
    }

    void FloatUI()
    {
        if (uiElements == null) return;

        for (int i = 0; i < uiElements.Length; i++)
        {
            float y = Mathf.Sin(Time.time * floatSpeed + i) * floatAmplitude;
            uiElements[i].anchoredPosition = basePositions[i] + new Vector2(0, y);
        }
    }
}
