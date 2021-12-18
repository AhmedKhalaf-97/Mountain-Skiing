using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyArmatureConfiguration : MonoBehaviour
{
    public bool isNeedSpringJoints;

    [Header("Jelly Configuration")]
    public bool enableConfiguration;

    public float spring;
    public float damper;
    public float sphereColliderRadius;
    public PhysicMaterial physicMaterial;

    SphereCollider[] sphereColliders;
    SpringJoint[] springJoints;

    Transform myTransform;
    int bonesCount;

    List<Rigidbody> rigidbodies = new List<Rigidbody>();

    void Awake()
    {
        myTransform = transform;
        bonesCount = myTransform.childCount;
    }

    void Start()
    {
        if (isNeedSpringJoints)
        {
            AddSpringJointsInBones();
        }
    }

    void Update()
    {
        if (enableConfiguration)
        {
            ConfigureJellyArmatureBones();
        }
    }

    void ConfigureJellyArmatureBones()
    {
        if (sphereColliders == null)
        {
            sphereColliders = myTransform.gameObject.GetComponentsInChildren<SphereCollider>();
        }

        if (springJoints == null)
        {
            springJoints = myTransform.gameObject.GetComponentsInChildren<SpringJoint>();
        }

        foreach (SphereCollider sc in sphereColliders)
        {
            if (sc.radius != sphereColliderRadius)
            {
                sc.radius = sphereColliderRadius;
            }

            if (sc.sharedMaterial != physicMaterial)
            {
                sc.sharedMaterial = physicMaterial;
            }
        }

        foreach (SpringJoint sj in springJoints)
        {
            if (sj.spring != spring)
            {
                sj.spring = spring;
            }

            if (sj.damper != damper)
            {
                sj.damper = damper;
            }
        }
    }

    void AddSpringJointsInBones()
    {
        for (int i = 0; i < bonesCount; i++)
        {
            for (int x = 0; x < (bonesCount - 1); x++)
            {
                myTransform.GetChild(i).gameObject.AddComponent<SpringJoint>();
            }
        }

        ConnectSpringJointsToEachOther();
    }

    void ConnectSpringJointsToEachOther()
    {
        for (int i = 0; i < bonesCount; i++)
        {
            SpringJoint[] springJoints = myTransform.GetChild(i).gameObject.GetComponents<SpringJoint>();

            rigidbodies.Clear();
            for (int x = 0; x < bonesCount; x++)
            {
                rigidbodies.Add(new Rigidbody());
                rigidbodies[x] = myTransform.GetChild(x).gameObject.GetComponent<Rigidbody>();
            }
            rigidbodies.RemoveAt(i);

            for (int y = 0; y < springJoints.Length; y++)
            {
                springJoints[y].connectedBody = rigidbodies[y];
            }
        }
    }
}