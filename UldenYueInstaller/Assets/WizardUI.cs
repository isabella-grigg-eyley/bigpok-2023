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
    
    // TODO improve uninteractable state to be more grey
    private const string KeySuccess = "H5E1LL3291KJ"; // H5E1 LL32 91KJ
    private const string KeyUsed = "L4L1BH2$48YR"; // L4L1 BH2$ 48YR

    public WizardPopupUI m_wizardPopupUI = null;

    public Text m_headerText = null;

    public Text m_bodyText = null;

    public TextMeshProUGUI m_windowHeaderText = null;

    public GameObject m_keyInputParent = null;
    public List<TMP_InputField> m_keyInputs = null;

    public ProgressBar m_progressBar = null;

    // buttons
    public GameObject m_prevButton = null;
    public GameObject m_nextButton = null;
    public GameObject m_cancelButton = null;
    
    public TextMeshProUGUI m_cancelButtonText = null;
    
    public enum WizardState
    {
        Closed,
        WelcomeScreen,
        InstallingProgressBarScreen,
        Register,
        InstallationComplete
    }

    private WizardState currentWizardState = WizardState.WelcomeScreen;

    private void Start()
    {
        currentWizardState = WizardState.Closed;
        RefreshWizardUI();

        WizardPopupUI.AOnPopupClosed += OnWizardPopupClosed;
    }

    private void RefreshWizardUI()
    {
        gameObject.SetActive(currentWizardState > WizardState.Closed);
        if (currentWizardState == WizardState.Closed)
            return;
        
        string cancelButtonText = "<u>C</u>ancel";
        if (currentWizardState == WizardState.InstallationComplete) cancelButtonText = "<u>F</u>inish";

        m_cancelButtonText.text = cancelButtonText;
        
        m_progressBar.gameObject.SetActive(currentWizardState == WizardState.InstallingProgressBarScreen);
        
        m_prevButton.gameObject.SetActive(currentWizardState < WizardState.InstallationComplete);
        m_nextButton.gameObject.SetActive(currentWizardState < WizardState.InstallationComplete);
        m_keyInputParent.SetActive(currentWizardState == WizardState.Register);
        
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
                m_nextButton.GetComponent<Button>().interactable = false;
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

    private event Action AOnNextClicked = null;
    public void OnNextClicked()
    {
        if (AOnNextClicked != null) // do this instead
        {
            AOnNextClicked?.Invoke();
            AOnNextClicked = null;
            return;
        }
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

    private void OnWizardPopupClosed(WizardPopupUI.WizardPopupState lastState)
    {
        switch (lastState)
        {
            case WizardPopupUI.WizardPopupState.IntroLoadingBar:
                currentWizardState = WizardState.WelcomeScreen;
                RefreshWizardUI();
                break;
            case WizardPopupUI.WizardPopupState.KeySuccess:
                currentWizardState = WizardState.InstallationComplete;
                RefreshWizardUI();
                break;
            case WizardPopupUI.WizardPopupState.ErrorKeyUsed:
            case WizardPopupUI.WizardPopupState.ErrorKeyInvalid:
                currentWizardState = WizardState.Register;
                foreach (TMP_InputField keyInputField in m_keyInputs)
                {
                    keyInputField.text = "";
                }
                RefreshWizardUI();
                break;
            case WizardPopupUI.WizardPopupState.Closed:
            default:
                break;
        }
    }

    public void OnValueEnteredKeyEntry()
    {
        foreach (TMP_InputField keyInputField in m_keyInputs)
        {
            // trim spaces
            keyInputField.text = keyInputField.text.Replace(" ", "");

            if (keyInputField.text.Length < 4) // key not done
            {
                m_nextButton.GetComponent<Button>().interactable = false; // make sure still uninteractable
                return;
            }
        }
        
        // full key entered
        string key = string.Empty;
        foreach (TMP_InputField keyInputField in m_keyInputs)
        {
            key += keyInputField.text;
        }
        
        if (key.Length < 12)
            return; // dunno how we got here
        
        print("key: " + key);

        if (key == KeySuccess)
        {
            AOnNextClicked = null;
            AOnNextClicked += () => m_wizardPopupUI.SetState(WizardPopupUI.WizardPopupState.KeySuccess);
        }
        else if (key == KeyUsed)
        {
            AOnNextClicked = null;
            AOnNextClicked += () => m_wizardPopupUI.SetState(WizardPopupUI.WizardPopupState.ErrorKeyUsed);
        }
        else
        {
            AOnNextClicked = null;
            AOnNextClicked += () => m_wizardPopupUI.SetState(WizardPopupUI.WizardPopupState.ErrorKeyInvalid);
        }
        
        m_nextButton.GetComponent<Button>().interactable = true;
    }
}
