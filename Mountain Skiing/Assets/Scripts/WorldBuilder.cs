using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{

    [Header("The Pool")]
    public GameObject basicStructureUnit;

    public int poolCapacity = 10;

    public Transform structureUnitsPoolTransform;

    [Header("The Builder")]
    public int maxWorldUnits = 3;

    public float worldUnitLength = 60;

    public Vector3 worldUnitEularAngle;

    public Vector3 buildingDirection;

    public Transform playerTransform;

    int totalUnitsBuildedInLifetime = 0;

    Transform myTransform;


    void Awake()
    {
        myTransform = transform;

        FillThePool();
    }

    void Start()
    {
        BuildTheWorld(maxWorldUnits); 
    }

    void Update()
    {
        MaintainTheMaxWorldUnits();
    }

    #region Structure Units Pool
    void FillThePool()
    {
        for (int i = 0; i < poolCapacity; i++)
        {
            GameObject go = Instantiate(basicStructureUnit);
            go.transform.parent = structureUnitsPoolTransform;
            go.SetActive(false);
        }
    }

    GameObject InstantiateFromThePool()
    {
        GameObject go = structureUnitsPoolTransform.GetChild(0).gameObject;
        go.transform.parent = null;
        go.SetActive(true);

        return go;
    }

    void ReturnGameobjectToPool(Transform structureUnit)
    {
        structureUnit.parent = null;

        structureUnit.position = basicStructureUnit.transform.position;
        structureUnit.eulerAngles = basicStructureUnit.transform.eulerAngles;
        structureUnit.localScale = basicStructureUnit.transform.localScale;

        structureUnit.parent = structureUnitsPoolTransform;

        structureUnit.gameObject.SetActive(false);
    }
    #endregion


    void BuildTheWorld(int worldUnitsNeededCount)
    {
        for (int i = 0; i < worldUnitsNeededCount; i++)
        {
            Transform unitTransform = InstantiateFromThePool().transform;

            unitTransform.parent = myTransform;
            unitTransform.localPosition = worldUnitLength * buildingDirection * totalUnitsBuildedInLifetime;
            unitTransform.localEulerAngles = worldUnitEularAngle;

            totalUnitsBuildedInLifetime++;
        }
    }

    void MaintainTheMaxWorldUnits()
    {
        if (Vector3.Distance(myTransform.GetChild(0).position, playerTransform.position) > 90)
        {
            ReturnGameobjectToPool(myTransform.GetChild(0));
            BuildTheWorld(1);
        }
    }
}