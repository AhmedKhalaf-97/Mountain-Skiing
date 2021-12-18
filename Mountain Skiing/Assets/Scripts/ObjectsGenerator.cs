using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectsGenerator : MonoBehaviour
{
    public bool separateByGroups;
    public Objects[] objects;

    [Tooltip("If you didn't choose the Object Parent, It will be automatically assigned with this transform")]
    public Transform objectsParent;

    BoxCollider myBoxCollider;

    public IgnoreAxis ignoreAxis;
    public enum IgnoreAxis { DontIgnoreAnyAxis, IgnoreXAxis, IgnoreYAxis, IgnoreZAxis};

    [System.Serializable]
    public class Objects
    {
        public GameObject theObject;
        public int objectsCount = 10;
        public Vector3 objectPositionAdjustion;
        public bool randomizeYAxisRotation;
        public Vector3 objectRotation; // I will randomize only Y axis rotation specifically for this game. It can be removed in the future.
    }

    void Awake()
    {
        if (GetComponent<BoxCollider>() != null)
            myBoxCollider = GetComponent<BoxCollider>();
        else
        {
            this.enabled = false;
            print("Assign BoxCollider First, The Script will not work without it");
        }

        if (objectsParent == null)
            objectsParent = transform;

        if (separateByGroups)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                GameObject go = new GameObject();
                go.transform.parent = objectsParent;
                go.name = objects[i].theObject.name;
            }
        }

        InstantiateObjectsRandomly();
    }

    void InstantiateObjectsRandomly()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            for (int x = 0; x < objects[i].objectsCount; x++)
            {
                Vector3 objectRot;

                if (objects[i].randomizeYAxisRotation)
                    objectRot = new Vector3(objects[i].objectRotation.x, Random.Range(0, 180), objects[i].objectRotation.z);
                else
                    objectRot = objects[i].objectRotation;

                GameObject go = Instantiate(objects[i].theObject, (GetRandomPointInsideBoxCollider() + objects[i].objectPositionAdjustion), Quaternion.Euler(objectRot));

                if (separateByGroups)
                    go.transform.parent = objectsParent.GetChild(i);
                else
                    go.transform.parent = objectsParent;
            }
        }
    }

    Vector3 GetRandomPointInsideBoxCollider()
    {
        Vector3 randomPoint = new Vector3(Random.Range(myBoxCollider.bounds.min.x, myBoxCollider.bounds.max.x),
            Random.Range(myBoxCollider.bounds.min.y, myBoxCollider.bounds.max.y),
            Random.Range(myBoxCollider.bounds.min.z, myBoxCollider.bounds.max.z));

        if (ignoreAxis == IgnoreAxis.IgnoreXAxis)
            randomPoint = new Vector3(0f, randomPoint.y, randomPoint.z);
        else if (ignoreAxis == IgnoreAxis.IgnoreYAxis)
            randomPoint = new Vector3(randomPoint.x, 0f, randomPoint.z);
        else if (ignoreAxis == IgnoreAxis.IgnoreZAxis)
            randomPoint = new Vector3(randomPoint.x, randomPoint.y, 0f);

        return randomPoint;
    }
}