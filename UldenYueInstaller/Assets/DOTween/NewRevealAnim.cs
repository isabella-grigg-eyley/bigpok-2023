using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NewRevealAnim : MonoBehaviour
{
    public AudioSource m_music = null;

    public List<FadeInData> fadeInData = null;

    private float time = 0.0f;

    [Serializable]
    public class FadeInData
    {
        public Image m_image = null;
        
        public float timeFromStartToFadeIn = 5.0f;
        public float timeToFadeIn = 2.0f;

        public bool fading = false;

        public void Init()
        {
            Color colour = m_image.color;
            colour.a = 0.0f;
            m_image.color = colour;
        }

        public void StartFade()
        {
            fading = true;
            DOTween.ToAlpha(() => m_image.color, x => m_image.color = x, 1.0f, timeToFadeIn).Play();
        }
    }
    void Start()
    {
        time = 0.0f;
        m_music.Play();
        for (var i = 0; i < fadeInData.Count; i++)
        {
            fadeInData[i].Init();
        }
    }

    private void Update()
    {
        time += Time.deltaTime;

        for (int i = 0; i < fadeInData.Count; i++)
        {
            FadeInData fadeData = fadeInData[i];

            if (fadeData.fading)
            {
                fadeInData.Remove(fadeData);
                i--;
                continue;
            }

            if (fadeData.timeFromStartToFadeIn <= time)
            {
                print("start fade");
                fadeData.StartFade();
                fadeInData.Remove(fadeData);
                i--;
            }
        }
    }
}
