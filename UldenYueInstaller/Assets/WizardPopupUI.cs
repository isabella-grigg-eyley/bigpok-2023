using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WizardPopupUI : MonoBehaviour
{
    public enum WizardPopupState
    {
        Closed,
        IntroLoadingBar,
        ErrorKeyUsed, // Over Register
        ErrorKeyInvalid,  // Over Register
        KeySuccess,  // Over Register
    }

    public TextMeshProUGUI m_headerText = null;
    public TextMeshProUGUI m_bodyText = null;

    public GameObject m_warningImage = null;
    public GameObject m_successImage = null;

    private WizardPopupState m_currentPopupState = WizardPopupState.IntroLoadingBar;

    public ProgressBar m_progressBar = null;

    public List<GameObject> m_closeButtons = null;

    public AudioSource m_chord = null;

    public static event Action<WizardPopupState> AOnPopupClosed = null; // sends last popup state

    void Start()
    {
        SetState(WizardPopupState.IntroLoadingBar);
    }

    public void SetState(WizardPopupState state)
    {
        m_currentPopupState = state;
        RefreshUI();
    }

    private void RefreshUI()
    {
        gameObject.SetActive(m_currentPopupState > WizardPopupState.Closed);
        m_progressBar.gameObject.SetActive(m_currentPopupState == WizardPopupState.IntroLoadingBar);
        foreach (GameObject closeButton in m_closeButtons)
        {
            closeButton.SetActive(m_currentPopupState != WizardPopupState.IntroLoadingBar);
        }
        
        switch (m_currentPopupState)
        {
            case WizardPopupState.IntroLoadingBar:
                m_headerText.text = "Loading Uldun Yue© Setup Wizard";
                m_bodyText.text = "Loading Uldun Yue© Setup Wizard...";
                m_warningImage.SetActive(false);
                m_successImage.SetActive(true);
                m_progressBar.OnProgressBarCompleted += OnLoadingBarComplete;
                StartCoroutine(m_progressBar.PlayProgressAnim());
                
                break;
            case WizardPopupState.ErrorKeyUsed:
                m_headerText.text = "Registration Error";
                m_bodyText.text = "Error: Registration key has already been used.";
                m_warningImage.SetActive(true);
                m_successImage.SetActive(false);
                m_chord.Play();
                break;
            case WizardPopupState.ErrorKeyInvalid:
                m_headerText.text = "Registration Error";
                m_bodyText.text = "Error: Registration key is invalid.";
                m_warningImage.SetActive(true);
                m_successImage.SetActive(false);
                m_chord.Play();
                break;
            case WizardPopupState.KeySuccess:
                m_headerText.text = "Registration Complete";
                m_bodyText.text = "Registration success!\nEnjoy playing Uldun Yue©!";
                m_warningImage.SetActive(false);
                m_successImage.SetActive(true);
                break;
            case WizardPopupState.Closed:
            default:
                break;
        }
    }

    public void OnPopupClosed()
    {
        AOnPopupClosed?.Invoke(m_currentPopupState);
        SetState(WizardPopupState.Closed);
    }

    public void OnLoadingBarComplete()
    {
        m_progressBar.OnProgressBarCompleted -= OnLoadingBarComplete;
        OnPopupClosed();
    }
}
