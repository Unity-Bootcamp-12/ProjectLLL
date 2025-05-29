using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeInPanel : MonoBehaviour
{
    [SerializeField] private Image _backGroundImage;
    [SerializeField] private float fadeDuration = 1f;

    private void Awake()
    {
        if (_backGroundImage == null)
        {
            _backGroundImage = GetComponent<Image>();
        }
        SetAlpha(0f);
        gameObject.SetActive(false);
    }

    private void SetAlpha(float alpha)
    {
        if (_backGroundImage != null)
        {
            Color c = _backGroundImage.color;
            c.a = alpha;
            _backGroundImage.color = c;
        }
    }

    public void FadeIn()
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeInCoroutine());
    }

    private IEnumerator FadeInCoroutine()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(1f);
    }
}
