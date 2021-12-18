using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Player player;
    public Challenges challenges;

    public bool isChallengeMode = true;

    public GameObject instructionsPanel;

    public GameObject[] requiredGameobjectsToPlay;

    [Header("Lives Upgrade")]
    int maxPlayerLives = 10;
    public int upgradeRequiredCoins;

    [Header("UI Variables")]
    public Text[] coinsAcquiredTexts;
    public Text coinsAcquiredGameplayText;
    public Text totalScoreText;
    public Text highScoreText;
    public Text flyTimesText;
    public Text currentChallengeNumberText;
    public Text nextChallengeNumberText;
    public Text modeText;
    public Text challengeDescriptionText;
    public Text livesUpgradeRequiredCoinsText;

    public Image challengeBarImage;
    public Image playerHitsBarImage;
    public Image playerLivesUpgradeBarImage;

    public Button challengeSelectButton;
    public Button endlessSelectButton;
    public Button upgradeButton;

    void Awake()
    {
        player.LoadGameData();

        SetUpgradePlayerLivePrice();
    }

    void Update()
    {
        UpdateUI();
    }

    public void PlayGame()
    {
        if (isChallengeMode)
            TinySauce.OnGameStarted(levelNumber: challenges.challengeLevel.ToString());
        else
            TinySauce.OnGameStarted();

        foreach (GameObject go in requiredGameobjectsToPlay)
            go.SetActive(true);

        Invoke("CloseInstructionsPanel", 5f);
    }

    void UpdateUI()
    {
        foreach (Text coinText in coinsAcquiredTexts)
            coinText.text = player.totalCoinsAcquired.ToString();

        coinsAcquiredGameplayText.text = player.coinsAcquiredDuringGameplay.ToString();
        totalScoreText.text = player.totalScore.ToString();
        highScoreText.text = player.highScore.ToString();
        flyTimesText.text = player.flyTimes.ToString();
        currentChallengeNumberText.text = challenges.challengeLevel.ToString();
        nextChallengeNumberText.text = (challenges.challengeLevel + 1).ToString();

        if (isChallengeMode)
        {
            modeText.text = "CHALLENGE: " + challenges.challengeLevel;
            challengeDescriptionText.text = challenges.challengeDescription;

            challengeSelectButton.transform.localScale = (Vector2.one * 1.5f);
            endlessSelectButton.transform.localScale = Vector2.one;

            challengeBarImage.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            modeText.text = "ENDLESS";
            challengeDescriptionText.text = "YOU WILL NOT LOSE IN ENDLESS MODE. THERE IS NO RULES";

            challengeSelectButton.transform.localScale = Vector2.one;
            endlessSelectButton.transform.localScale = (Vector2.one * 1.5f);


            challengeBarImage.transform.parent.gameObject.SetActive(false);
        }

        livesUpgradeRequiredCoinsText.text = upgradeRequiredCoins.ToString();

        challengeBarImage.fillAmount = challenges.challengeProgress;
        playerHitsBarImage.fillAmount = (float)(player.playerLives - player.playerHits) / (float)player.playerLives;
        playerLivesUpgradeBarImage.fillAmount = (float)player.playerLives / (float)maxPlayerLives;

        if(maxPlayerLives == player.playerLives || player.totalCoinsAcquired < upgradeRequiredCoins)
            upgradeButton.interactable = false;
        else
            upgradeButton.interactable = true;
    }

    public void SwitchToChallengeMode(bool isChallenge)
    {
        if (isChallenge)
        {
            isChallengeMode = true;
            challenges.enabled = true;
        }
        else
        {
            isChallengeMode = false;
            challenges.enabled = false;
        }
    }

    void SetUpgradePlayerLivePrice()
    {
        upgradeRequiredCoins = ((maxPlayerLives - 2) - (maxPlayerLives - player.playerLives)) * 50;
    }

    public void UpgradePlayerLives()
    {
        if(player.totalCoinsAcquired >= upgradeRequiredCoins)
        {
            player.PlayUpgradeSFX();

            player.totalCoinsAcquired -= upgradeRequiredCoins;
            player.playerLives++;

            SetUpgradePlayerLivePrice();

            player.SaveCoinsAcquired();
            DataSaveManager.SaveData("PlayerLives", player.playerLives);
        }
    }

    void CloseInstructionsPanel()
    {
        instructionsPanel.SetActive(false);
    }
}
