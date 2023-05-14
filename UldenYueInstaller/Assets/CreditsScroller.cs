using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class CreditsScroller : MonoBehaviour
{
    public RectTransform m_creditsText = null;

    public RectTransform m_background = null;

    public float m_scrollTime = 10.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        // Start music
        ScrollCredits();
    }

    private void ScrollCredits()
    {
        float distanceToScroll = m_creditsText.rect.y + m_background.rect.y;

        Vector3 targetPos = m_creditsText.localPosition;
        targetPos.y -= distanceToScroll; // minus?
        targetPos.y -= m_creditsText.localPosition.y; // minus?
        TweenerCore<Vector3, Vector3, VectorOptions> tween = 
            DOTween.To(() => m_creditsText.localPosition, 
                x => m_creditsText.localPosition = x, 
                targetPos, m_scrollTime);
        tween.SetEase(Ease.Linear);
        tween.onComplete += OnTweenComplete;
        tween.Play();
    }

    private void OnTweenComplete()
    {
        print("credits done");
        // fade out music
    }
}