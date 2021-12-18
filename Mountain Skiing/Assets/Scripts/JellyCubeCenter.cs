using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JellyCubeCenter : MonoBehaviour
{
    [Header("Center Variables")]
    public GameObject[] Bones;

    public Vector3 Center;

    public SkinnedMeshRenderer SMRenderer;
    public Mesh playerBakedMesh;

    void Awake ()
    {

    }

    void FixedUpdate ()
    {
        CenterTheGameObject();
	}

    void CenterTheGameObject()
    {
        for (int i = 0; i < Bones.Length; i++)
        {
            Center += Bones[i].transform.position;
        }

        Center = Center / Bones.Length;

        transform.position = Center;

        Center = Vector3.zero;

        //PlayerBakeMeshToCollider();
    }

    void PlayerBakeMeshToCollider()
    {
        SMRenderer.BakeMesh(playerBakedMesh);
        gameObject.GetComponent<MeshCollider>().sharedMesh = playerBakedMesh;
    }
}
