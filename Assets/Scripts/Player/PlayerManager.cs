using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static bool gameOver;
    public GameObject gameOverPanel;

    public static bool isGameStarted;
    public GameObject startingText;

    public static int numberOfCoins;
    public Text coinsText;

    private TileManager tileManager;
    public Button autoAssistToggleButton;
    
    public int coinsRequiredForAutoAssist = 5;
    public float autoAssistDurationSeconds = 30f;
    
    private Coroutine autoAssistCoroutine;
    private float autoAssistTimeRemaining = 0f;
    
    private Text buttonText;

    void Start()
    {
        Time.timeScale = 1;
        gameOver = false;
        isGameStarted = false;
        numberOfCoins = 0;
        
        buttonText = autoAssistToggleButton.GetComponentInChildren<Text>();
        buttonText.text= $"Auto assist ({coinsRequiredForAutoAssist} Coins)";

        tileManager = FindObjectOfType<TileManager>();

        if (autoAssistToggleButton != null && tileManager != null)
        {
            autoAssistToggleButton.onClick.AddListener(() =>
            {
                if (numberOfCoins >= coinsRequiredForAutoAssist)
                {
                    if (autoAssistCoroutine != null)
                        StopCoroutine(autoAssistCoroutine);

                    autoAssistToggleButton.interactable = false; 

                    autoAssistCoroutine = StartCoroutine(EnableAutoAssistForSeconds(autoAssistDurationSeconds));
                    numberOfCoins -= coinsRequiredForAutoAssist;
                    UpdateAutoAssistStatusText();
                }
            });
        }

        UpdateAutoAssistStatusText();
    }
    
    IEnumerator EnableAutoAssistForSeconds(float seconds)
    {
        tileManager.autoAssistEnabled = true;
        autoAssistTimeRemaining = seconds;

        while (autoAssistTimeRemaining > 0)
        {
            UpdateAutoAssistStatusText();
            yield return new WaitForSeconds(1f);
            autoAssistTimeRemaining -= 1f;
        }

        tileManager.autoAssistEnabled = false;
        autoAssistToggleButton.interactable = true; 
        autoAssistTimeRemaining = 0f;
        UpdateAutoAssistStatusText();
    }
    
    private void UpdateAutoAssistStatusText()
    {
        if (buttonText != null && tileManager != null)
        {
            if (tileManager.autoAssistEnabled)
                buttonText.text= $"Auto Assist: ON ({Mathf.CeilToInt(autoAssistTimeRemaining)}s left)";
            else
                buttonText.text= $"Auto assist ({coinsRequiredForAutoAssist} Coins)";
        }
    }

    void Update()
    {
        if (gameOver)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0;
        }

        coinsText.text = "Coins: " + numberOfCoins;

        if (SwipeManager.tap && !isGameStarted)
        {
            isGameStarted = true;
            Destroy(startingText);
        }
    }
}