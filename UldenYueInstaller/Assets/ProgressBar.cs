using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image m_barFill = null;
    public TextMeshProUGUI m_progressText = null;

    private float m_progress = 0.0f;
    
    [Header("Sort by target percentage")]
    public List<ProgressAnimationPoints> AnimationPointsList = null;

    public event Action OnProgressBarCompleted = null;

    [Serializable]
    public class ProgressAnimationPoints
    {
        public float TimeToReach = 0.0f;
        public float TargetPercentage = 0.0f;
    }

    void Start()
    {
        SetProgress(0.0f);
    }

    public IEnumerator PlayProgressAnim()
    {
        SetProgress(0.0f);

        foreach (ProgressAnimationPoints animationPoint in AnimationPointsList)
        {
            yield return new WaitForSeconds(animationPoint.TimeToReach);
            SetProgress(animationPoint.TargetPercentage);
        }

        if (m_progress < 1.0f) // ensure we're at 100%
        {
            yield return new WaitForSeconds(AnimationPointsList[0].TimeToReach);
            SetProgress(1.0f);
        }
        
        OnProgressBarCompleted?.Invoke();
    }

    private void SetProgress(float percent) // 0.0-1.0f == 0-100%
    {
        percent = Mathf.Clamp(percent, 0.0f, 1.0f);
        m_progress = percent;
        m_barFill.fillAmount = percent;
        m_progressText.text = $"{percent*100.0f}%";
        m_progressText.color = percent >= 0.5f ? Color.white : Color.black;
    }
}
