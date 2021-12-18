using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameManager gameManager;
    public Challenges challenges;

    [Header("Movement")]
    public float generalMoveSpeed = 2f;
    public float forwardMoveSpeed = 2f;
    public float horizontalSpeedDividerFactor = 1.5f;

    public float windForce = 0.5f;
    public float windsTime = 5f;
    public float windsRepeatRate = 30f;

    public CameraFollow cameraFollow;

    float windDir;
    float[] windForces;
    Coroutine applyWindsCoroutine;

    Vector3 inputVectors;

    float horizontalAxis;
    float startingHorizonatlAxis;
    float tLerp;

    public float buttonXAxis;

    public float lerpSensitivity = 1f;

    [Header("Jumping")]
    public int flyTimes;
    public bool isJumping;
    public float jumpTime = 6f;
    public float jumpDistance = 1f;
    float jumpTimer;
    float jumpValue;

    [Header("Coins Acquirment")]
    public int totalCoinsAcquired;
    public int coinsAcquiredDuringGameplay;

    [Header("Player Lives")]
    public bool isHitByTree;
    public bool isBypassTree;
    public int playerHits;
    public int playerLives;

    public GameObject gameOverPanel;

    [Header("Scoring")]
    public int highScore;
    public int totalScore;
    public int additiveScoreAmount = 100;

    int progressiveScore;
    int additiveScore;

    public Transform additiveScoreTextParent;

    [Header("Particle Systems")]
    public Transform smokeParticlesTransform;
    public Transform snowParticlesTransform;

    [Header("The Texts Pool")]
    public GameObject additiveScoreTextGameobject;

    public int poolCapacity = 10;

    public Transform additiveScoreTextPoolTransform;

    [Header("Audio")]
    public AudioSource skiingSFXAC;
    public AudioSource coinsCollectAC;
    public AudioSource whooshAC;
    public AudioSource snowImpactAC;
    public AudioSource treeHitAC;
    public AudioSource challengeCompletedAC;
    public AudioSource gameoverAC;
    public AudioSource upgradeAC;


    Transform myTransform;

    Rigidbody myRigidbody;


    void Awake()
    {
        FillThePool();
    }

    void Start()
    {
        myTransform = transform;

        myRigidbody = GetComponent<Rigidbody>();

        jumpTimer = jumpTime;

        windForces = new float[2] {windForce, -windForce };

        InvokeRepeating("ApplyWinds", windsRepeatRate, windsRepeatRate);
    }

    void FixedUpdate()
    {

        if (Application.isMobilePlatform)
        {
            if (horizontalAxis != buttonXAxis)
            {
                horizontalAxis = Mathf.Lerp(startingHorizonatlAxis, buttonXAxis, tLerp);

                tLerp += lerpSensitivity * Time.deltaTime;
            }
            else
            {
                tLerp = 0;
                startingHorizonatlAxis = 0;
            }
        }
        else
            horizontalAxis = Input.GetAxis("Horizontal");


        inputVectors = new Vector3((horizontalAxis + windDir / horizontalSpeedDividerFactor), jumpValue, forwardMoveSpeed);

        myRigidbody.MovePosition(myTransform.position + inputVectors * Time.fixedDeltaTime * generalMoveSpeed);

        myTransform.eulerAngles = new Vector3(0, (inputVectors.x * 25), 0);

        snowParticlesTransform.position = new Vector3((windDir * 16), snowParticlesTransform.position.y, snowParticlesTransform.position.z);
    }

    public void ResetTLerp()
    {
        tLerp = 0f;

        startingHorizonatlAxis = horizontalAxis;
    }

    void Update()
    {
        if (isJumping) //Called from PlayerCollisionTrigger.
        {
            Jump();

            if (smokeParticlesTransform.gameObject.activeInHierarchy)
                smokeParticlesTransform.gameObject.SetActive(false);
        }
        else
        {
            if (!smokeParticlesTransform.gameObject.activeInHierarchy)
                smokeParticlesTransform.gameObject.SetActive(true);
        }

        RecordScore();
    }

    void Jump()
    {
        jumpTimer -= Time.deltaTime;

        myRigidbody.useGravity = false;

        if (jumpTimer <= (jumpTime - 1))
        {
            jumpValue -= Time.deltaTime;

            if (jumpValue <= 0)
                jumpValue = 0f;
        }
        else
        {
            jumpValue = jumpDistance;
        }

        if(jumpTimer <= 0)
        {
            myRigidbody.useGravity = true;
            jumpValue = 0f;
            jumpTimer = jumpTime;

            isJumping = false;
        }
    }

    public void AcquireCoin(Transform coinTransfrom) //Called from PlayerCollisionTrigger.
    {
        coinsCollectAC.Play();

        coinsAcquiredDuringGameplay++;

        StartCoroutine(AutoHideShowUP(coinTransfrom.GetComponent<MeshRenderer>(), 2f));
    }

    IEnumerator AutoHideShowUP(MeshRenderer objectMeshRenderer, float hidingTime)
    {
        ParticleSystem coinHitParticleSystem = objectMeshRenderer.transform.GetComponentInChildren<ParticleSystem>();

        coinHitParticleSystem.Play();

        objectMeshRenderer.enabled = false;

        yield return new WaitForSeconds(hidingTime);

        objectMeshRenderer.enabled = true;

        coinHitParticleSystem.Stop();
    }

    public void SaveCoinsAcquired()
    {
        totalCoinsAcquired += coinsAcquiredDuringGameplay;

        DataSaveManager.SaveData("CoinsAcquired", totalCoinsAcquired);
    }

    public void RockHit()
    {
        PlayerHasBeenHit();
    }

    public void TreeHit(Transform treeTransform)
    {
        isHitByTree = true;

        TreeBrokeAndResetCoroutine(treeTransform);

        PlayerHasBeenHit();
    }

    public void TreeBrokeAndResetCoroutine(Transform treeTransform)
    {
        StartCoroutine(TreeBrokeAndReset(treeTransform, 2f));
    }

    IEnumerator TreeBrokeAndReset(Transform treeTransform, float brokingShowTime)
    {
        ParticleSystem treeHitParticleSystem = treeTransform.GetComponentInChildren<ParticleSystem>();
        GameObject treeBreakSFXGO = treeTransform.GetChild(4).gameObject;

        treeTransform.GetComponent<MeshCollider>().enabled = false;
        treeTransform.GetComponent<MeshRenderer>().enabled = false;

        for (int i = 0; i < treeTransform.childCount; i++)
        {
            treeTransform.GetChild(i).gameObject.SetActive(true);
        }

        treeHitParticleSystem.Play();

        treeBreakSFXGO.SetActive(true);

        yield return new WaitForSeconds(brokingShowTime);

        treeBreakSFXGO.SetActive(false);

        treeHitParticleSystem.Stop();

        treeTransform.GetComponent<MeshCollider>().enabled = true;
        treeTransform.GetComponent<MeshRenderer>().enabled = true;

        for (int i = 0; i < treeTransform.childCount; i++)
        {
            treeTransform.GetChild(i).gameObject.SetActive(false);

            treeTransform.GetChild(i).localPosition = Vector3.zero;
            treeTransform.GetChild(i).localEulerAngles = Vector3.zero;
        }
    }

    void PlayerHasBeenHit()
    {
        treeHitAC.Play();

        cameraFollow.ShakeCamera();

        if (!gameManager.isChallengeMode)
            return;

        playerHits++;

        if (playerHits >= playerLives)
            GameOver();
    }

    void GameOver()
    {
        gameoverAC.Play();

        StopAllCoroutines();
        CancelInvoke();

        GameFinished();
    }

    void ApplyWinds() //Called with Invoke Method on Start function.
    {
        if (applyWindsCoroutine != null)
            StopCoroutine(applyWindsCoroutine);

        applyWindsCoroutine = StartCoroutine(ApplyWindsCoroutine());
    }

    IEnumerator ApplyWindsCoroutine()
    {
        windDir = windForces[Random.Range(0, windForces.Length)];

        yield return new WaitForSeconds(windsTime);

        windDir = 0f;
    }

    public void BypassTree()
    {
        isBypassTree = true;

        StartCoroutine(BypassTreeCoroutine());
    }

    IEnumerator BypassTreeCoroutine()
    {
        yield return new WaitForSeconds(0.1f);

        if (!isHitByTree)
            AddAdditiveScore();

        isHitByTree = false;
        isBypassTree = false;
    }
    
    void AddAdditiveScore()
    {
        whooshAC.Play();

        additiveScore += additiveScoreAmount;

        Text additiveScoreText = InstantiateFromThePool().GetComponent<Text>();

        additiveScoreText.transform.SetParent(additiveScoreTextParent, false);
    }

    void RecordScore()
    {
        progressiveScore = (int)myTransform.position.z;

        totalScore = progressiveScore + additiveScore;
    }

    void SaveHighScore()
    {
        if (DataSaveManager.IsDataExist("HighScore"))
            highScore = (int)DataSaveManager.LoadData("HighScore");
        else
            highScore = 0;

        if (totalScore > highScore)
        {
            highScore = totalScore;
            DataSaveManager.SaveData("HighScore", highScore);
        }
    }

    void GameFinished()
    {
        gameOverPanel.SetActive(true);

        myTransform.gameObject.SetActive(false);
    }

    public void ReloadGame()
    {
        TinySauceGameFinished();

        SaveHighScore();
        SaveCoinsAcquired();

        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void TinySauceGameFinished()
    {
        int previousChallengeLevel = (challenges.challengeLevel - 1);

        if (gameManager.isChallengeMode)
        {
            if (challenges.isChallengePassed)
                TinySauce.OnGameFinished(levelNumber:previousChallengeLevel.ToString(), true, totalScore);
            else
                TinySauce.OnGameFinished(levelNumber: challenges.challengeLevel.ToString(), false, totalScore);
        }
        else
        {
            TinySauce.OnGameFinished(totalScore);
        }
    }

    public void LoadGameData()
    {
        if (DataSaveManager.IsDataExist("CoinsAcquired"))
            totalCoinsAcquired = (int)DataSaveManager.LoadData("CoinsAcquired");

        if (DataSaveManager.IsDataExist("HighScore"))
            highScore = (int)DataSaveManager.LoadData("HighScore");

        if (DataSaveManager.IsDataExist("PlayerLives"))
            playerLives = (int)DataSaveManager.LoadData("PlayerLives");
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void PlaySkiingSFX(bool shouldPlay)
    {
        if (shouldPlay)
        {
            skiingSFXAC.Play();
            snowImpactAC.Play();
        }
        else
            skiingSFXAC.Stop();
    }

    public void PlayChallengeCompletedSFX()
    {
        challengeCompletedAC.Play();
    }

    public void PlayUpgradeSFX()
    {
        upgradeAC.Play();
    }

    #region Text UI Units Pool (Not Usable For Normal Gameobjects)
    void FillThePool()
    {
        for (int i = 0; i < poolCapacity; i++)
        {
            GameObject go = Instantiate(additiveScoreTextGameobject);
            go.transform.SetParent(additiveScoreTextPoolTransform, false);
            go.SetActive(false);
        }
    }

    GameObject InstantiateFromThePool()
    {
        GameObject go = additiveScoreTextPoolTransform.GetChild(0).gameObject;
        go.SetActive(true);

        return go;
    }

    public void ReturnGameobjectToPool(Transform structureUnit)
    {
        structureUnit.position = additiveScoreTextGameobject.transform.position;

        structureUnit.SetParent(additiveScoreTextPoolTransform, false);

        structureUnit.gameObject.SetActive(false);
    }
    #endregion
}