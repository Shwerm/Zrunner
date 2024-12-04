using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Fucntion to flash green sqaure buttons for combat qte UI.
/// </summary>
public class GreenSqaureFlash : MonoBehaviour
{
    [SerializeField] private float FlashAlphaMax = 1f;
    [SerializeField] private float FlashAlphaMin = 0f;
    [SerializeField] private float flashSpeed = 3f;
    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    void OnEnable()
    {
        StartCoroutine(FlashAlpha(FlashAlphaMax, FlashAlphaMin));
    }

    /// <summary>
    /// Lerp for alpha value of image.
    /// </summary>
    /// <param name="targetAlpha"></param>
    /// <param name="startAlpha"></param>
    /// <returns></returns>
    private IEnumerator FlashAlpha(float targetAlpha, float startAlpha)
    {
        while (true)
        {
            for (float t = 0f; t < 1f; t += Time.deltaTime * flashSpeed)
            {
                Color newColor = image.color;
                newColor.a = Mathf.Lerp(startAlpha, targetAlpha, t);
                image.color = newColor;
                yield return null;
            }
        }
    }
}