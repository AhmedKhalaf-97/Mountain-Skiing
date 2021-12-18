using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Challenges : MonoBehaviour
{
    public Player player;

    public int challengeLevel = 1;

    public string challengeDescription;
    public bool isWithoutHitting;

    public bool isChallengePassed;

    public float challengeProgress;

    [Header("Challenges Modes")]
    public bool isScoreModed;
    public int challengeNeededScore;
    int minNeededScore = 200;

    [Space]
    public bool isCoinsModed;
    public int challengeNeededCoins;
    int minNeededCoins = 5;

    [Space]
    public bool isFlyTimesModed;
    public int challengeNeededFlyTimes;
    int minNeededFlyTimes = 2;


    void Awake()
    {
        if (DataSaveManager.IsDataExist("ChallengeLevel"))
            challengeLevel = (int)DataSaveManager.LoadData("ChallengeLevel");
        else
            challengeLevel = 1;
    }

    void Start()
    {
        GenerateRandomChallenge();
    }

    void Update()
    {
        if (!isChallengePassed)
            CheckIfChallengePassed();
    }

    void GenerateRandomChallenge()
    {
        int challengeIndex = Random.Range(1, 4);

        if(challengeIndex == 1)
        {
            isScoreModed = true;
            challengeNeededScore = Random.Range((minNeededScore + (5 * challengeLevel)), (minNeededScore + 100 + (5 * challengeLevel)));
        }

        if (challengeIndex == 2)
        {
            isCoinsModed = true;
            challengeNeededCoins = Random.Range((minNeededCoins + challengeLevel), (minNeededCoins + 3 + challengeLevel));
        }

        if (challengeIndex == 3)
        {
            isFlyTimesModed = true;
            challengeNeededFlyTimes = Random.Range((minNeededFlyTimes + challengeLevel), (minNeededFlyTimes + 3 + challengeLevel));
        }

        isWithoutHitting = RandomBoolean(30);

        SetChallengeDescription();
    }

    void SetChallengeDescription()
    {
        if (isScoreModed)
            challengeDescription = "Score " + challengeNeededScore;

        if (isCoinsModed)
            challengeDescription = "Collect " + challengeNeededCoins + " Coin";

        if (isFlyTimesModed)
            challengeDescription = "Fly x" + challengeNeededFlyTimes + " Times";

        if (isWithoutHitting)
            challengeDescription += " Without Hitting Any Trees Or Rocks";
    }

    void CheckIfChallengePassed()
    {
        if (isWithoutHitting)
            if (player.playerHits > 0)
            {
                challengeProgress = 0;
                return;
            }

        if (isScoreModed)
        {
            challengeProgress = (float)player.totalScore / (float)challengeNeededScore;

            if (player.totalScore >= challengeNeededScore)
                Challengepassed();
        }

        if (isCoinsModed)
        {
            challengeProgress = (float)player.coinsAcquiredDuringGameplay / (float)challengeNeededCoins;

            if (player.coinsAcquiredDuringGameplay >= challengeNeededCoins)
                Challengepassed();
        }       

        if (isFlyTimesModed)
        {
            challengeProgress = (float)player.flyTimes / (float)challengeNeededFlyTimes;

            if (player.flyTimes >= challengeNeededFlyTimes)
                Challengepassed();
        }
    }

    void Challengepassed()
    {
        player.PlayChallengeCompletedSFX();

        challengeLevel++;

        DataSaveManager.SaveData("ChallengeLevel", challengeLevel);

        isChallengePassed = true;
    }

    bool RandomBoolean(int conditionPossibilityPercentage)
    {
        int randomIndex = Random.Range(0, 100);

        if (randomIndex < conditionPossibilityPercentage)
            return true;
        else
            return false;
    }
}