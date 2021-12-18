using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform targetTransform;
    public float smothness = 5f;

    [Header("Shaking")]
    public float shaknessRange = 0.2f;
    public float shaknessTimer = 0.5f;
    float shaknessTimerCountdown;

    Vector3 lerpPos;
    Vector3 targetPos;
    Vector3 offset;

    Transform myTransform;

    void Start()
    {
        myTransform = transform;
        offset = myTransform.position - targetTransform.position;
    }

    void FixedUpdate()
    {
        targetPos = targetTransform.position + offset;

        lerpPos = Vector3.Lerp(myTransform.position, targetPos, smothness * Time.fixedDeltaTime);

        myTransform.position = lerpPos;
    }

    public void ShakeCamera()
    {
        StartCoroutine(ShakeCameraCoroutine());
    }

    IEnumerator ShakeCameraCoroutine()
    {
        shaknessTimerCountdown = shaknessTimer;

        while (true)
        {
            shaknessTimerCountdown -= Time.deltaTime;

            if (shaknessTimerCountdown > 0)
            {
                myTransform.position += Random.insideUnitSphere * shaknessRange;
            }
            else
            {
                break;
            }

            yield return null;
        }
    }
}
