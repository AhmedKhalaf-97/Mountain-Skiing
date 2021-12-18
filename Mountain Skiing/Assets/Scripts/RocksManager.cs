using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocksManager : MonoBehaviour
{
    bool shouldInstantiateRock;

    public float rockStartTime = 40f;
    public float rockLifeTime = 5f;

    public Player player;
    public Transform playerTransform;
    public Vector3 rockInstantiatingOffset = new Vector3(1f, 0f, 2f);

    Vector3 rockInstantiatingOffset_RightSide;
    Vector3 rockInstantiatingOffset_LeftSide;

    bool isRightDir;

    [Header("Rocks Controller")]
    public float rockMovingSpeed = 4f;
    public Vector3 rockMovingDir = new Vector3(0.5f, 0f, 1f);

    Vector3 rockMovingDir_Right;
    Vector3 rockMovingDir_Left;

    Transform rockTransform;
    Rigidbody rockRigidbody;

    [Header("The Pool")]
    public GameObject rockPrefab;
    public int poolCapacity = 5;


    Transform myTransform;

    void Awake()
    {
        myTransform = transform;

        rockInstantiatingOffset_RightSide = MultiplyVectors(rockInstantiatingOffset, new Vector3(1f, 1f, 1f));
        rockInstantiatingOffset_LeftSide = MultiplyVectors(rockInstantiatingOffset, new Vector3(-1f, 1f, 1f));

        rockMovingDir_Right = MultiplyVectors(rockMovingDir, new Vector3(-1f, 1f, 1f));
        rockMovingDir_Left = MultiplyVectors(rockMovingDir, new Vector3(1f, 1f, 1f));

        FillThePool();
    }

    void Start()
    {
        Invoke("InstantiateRock", rockStartTime);
    }

    #region The Pool
    void FillThePool()
    {
        for (int i = 0; i < poolCapacity; i++)
        {
            GameObject go = Instantiate(rockPrefab);
            go.transform.parent = myTransform;
            go.SetActive(false);
        }
    }

    GameObject InstantiateFromThePool()
    {
        GameObject go = myTransform.GetChild(0).gameObject;
        go.transform.parent = null;
        go.SetActive(true);

        return go;
    }

    void ReturnGameobjectToPool(Transform rockTransform)
    {
        rockTransform.parent = null;

        rockTransform.position = rockPrefab.transform.position;
        rockTransform.eulerAngles = rockPrefab.transform.eulerAngles;
        rockTransform.localScale = rockPrefab.transform.localScale;

        rockTransform.parent = myTransform;

        rockTransform.gameObject.SetActive(false);
    }
    #endregion

    void Update()
    {
        if (player.isJumping)
            shouldInstantiateRock = false;
        else
            shouldInstantiateRock = true;
    }

    void FixedUpdate()
    {
        if (rockRigidbody != null)
                rockRigidbody.MovePosition(rockTransform.position + rockMovingDir * Time.deltaTime * rockMovingSpeed);
    }

    void InstantiateRock()
    {
        if (!player.gameObject.activeInHierarchy)
            return;

        if (shouldInstantiateRock)
        {
            if (Random.Range(0, 2) == 0)
                isRightDir = true;
            else
                isRightDir = false;


            if (isRightDir)
            {
                rockInstantiatingOffset = rockInstantiatingOffset_RightSide;
                rockMovingDir = rockMovingDir_Right;
            }
            else
            {
                rockInstantiatingOffset = rockInstantiatingOffset_LeftSide;
                rockMovingDir = rockMovingDir_Left;
            }

            Transform rockTran = InstantiateFromThePool().transform;

            rockTran.position = playerTransform.position + rockInstantiatingOffset;

            rockRigidbody = rockTran.GetComponent<Rigidbody>();
            rockTransform = rockTran;
        }

        Invoke("RemoveRock", rockLifeTime);
    }

    Vector3 MultiplyVectors(Vector3 vectorA, Vector3 vectorB)
    {
        vectorA.x *= vectorB.x;
        vectorA.y *= vectorB.y;
        vectorA.z *= vectorB.z;

        return vectorA;
    }

    void RemoveRock() //Called From InstantiateRock().
    {
        if(rockTransform != null)
        {
            rockRigidbody = null;

            ReturnGameobjectToPool(rockTransform);

            rockTransform = null;
        }

        InstantiateRock();
    }
}