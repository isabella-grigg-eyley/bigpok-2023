using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WizardUI : MonoBehaviour
{
    /*
    1. Loading bar (loading installer)
    2. Welcome to the installer! (Full screen)
    3. Installing... (full screen)
    4. Please register your copy of Ulden Yue. you need a key!
    5. if key == used
        Popup error: This key has been used before!
    6. if key == wrong
        Popup error: This key is invalid
    7. if key == right
        Your copy of Ulden Yue has been registered
        
        
        stretch: EULA, where to install
     * */

    public Text m_headerText = null;

    public Text m_bodyText = null;

    public TextMeshProUGUI m_windowHeaderText = null;

    public InputField m_keyInput = null;

    public ProgressBar m_progressBar = null;

    // buttons
    public GameObject m_prevButton = null;
    public GameObject m_nextButton = null;
    public GameObject m_cancelButton = null;
    
    public TextMeshProUGUI m_cancelButtonText = null;
    
    public enum WizardState
    {
        WelcomeScreen,
        InstallingProgressBarScreen,
        Register,
        InstallationComplete
    }

    public enum WizardPopupState
    {
        IntroLoadingBar,
        ErrorKeyUsed, // Over Register
        ErrorKeyInvalid,  // Over Register
        KeySuccess,  // Over Register
    }

    private WizardState currentWizardState = WizardState.WelcomeScreen;
    private WizardPopupState currentPopupState = WizardPopupState.IntroLoadingBar;

    private void Start()
    {
        currentWizardState = WizardState.WelcomeScreen;
        RefreshWizardUI();
    }

    private void RefreshWizardUI()
    {
        //m_keyInput.gameObject.SetActive(currentWizardState == WizardState.Register);

        string cancelButtonText = "<u>C</u>ancel";
        if (currentWizardState == WizardState.InstallationComplete) cancelButtonText = "<u>F</u>inish";

        m_cancelButtonText.text = cancelButtonText;
        
        m_progressBar.gameObject.SetActive(currentWizardState == WizardState.InstallingProgressBarScreen);
        
        m_prevButton.gameObject.SetActive(currentWizardState < WizardState.InstallationComplete);
        m_nextButton.gameObject.SetActive(currentWizardState < WizardState.InstallationComplete);
        
        switch (currentWizardState)
        {
            case WizardState.WelcomeScreen:
            {
                string headerText = "Welcome to the Uldun Yue© Setup Wizard!";
                string bodyText = "This wizard will install Uldun Yue© and all necessary files on your computer.";
                m_headerText.text = headerText;
                m_bodyText.text = bodyText;
                break;
            }
            case WizardState.InstallingProgressBarScreen:
            {
                string headerText = "Installing...";
                string bodyText = "";
                m_headerText.text = headerText;
                m_bodyText.text = bodyText;
                // Start progress bar animation
                m_progressBar.OnProgressBarCompleted += OnProgressBarComplete;
                m_nextButton.GetComponent<Button>().interactable = false;
                StartCoroutine(m_progressBar.PlayProgressAnim());
                break;
            }
            case WizardState.Register:
            {
                string headerText = "License Registration";
                string bodyText = "Please enter your registration key of Uldun Yue©.\nYou can find this key in your Uldun Yue© CD-ROM box.";
                m_headerText.text = headerText;
                m_bodyText.text = bodyText;
                break;
            }
            case WizardState.InstallationComplete:
            {
                string headerText = "Installation Complete";
                string bodyText = "The setup wizard has successfully installed Uldun Yue©.\nClick Finish to exit the wizard and open the Uldun Yue© Launcher.";
                m_headerText.text = headerText;
                m_bodyText.text = bodyText;
                break;
            }
        }
        //todo: loading bar anim
    }
    
    public void OnNextClicked()
    {
        currentWizardState++;
        print(currentWizardState.ToString());
        RefreshWizardUI();
    }

    public void OnCancelClicked()
    {
        if (currentWizardState == WizardState.InstallationComplete)
        {
            print("end celebration");
            return;
        }
        Application.Quit();
    }

    private void OnProgressBarComplete()
    {
        m_progressBar.OnProgressBarCompleted -= OnProgressBarComplete;
        
        // Progress finished, unlock next button
        m_nextButton.GetComponent<Button>().interactable = true;
    }
}
