using UnityEngine;
using System.Collections;

public class MenuCamera3DFX : MonoBehaviour
{
    [Header("Shake Settings")]
    public float shakeAmplitude = 0.2f;
    public float shakeFrequency = 1f;
    public float shakeDuration = 0.5f;

    [Header("Rotation Settings")]
    public float waitTime = 5f;
    public float rotationDuration = 1f;

    private Vector3 startPos;
    private Quaternion startRot;
    private bool isShaking = false;
    float totalRotation = 0f;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        StartCoroutine(RotationLoop());
    }

    void Update()
    {
        if (isShaking)
        {
            float x = (Mathf.PerlinNoise(Time.time * shakeFrequency, 0f) - 0.5f) * shakeAmplitude;
            float y = (Mathf.PerlinNoise(0f, Time.time * shakeFrequency) - 0.5f) * shakeAmplitude;
            float z = (Mathf.PerlinNoise(Time.time * shakeFrequency, Time.time * shakeFrequency) - 0.5f) * shakeAmplitude;

            transform.position = startPos + new Vector3(x, y, z);
        }
    }

    IEnumerator RotationLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);

            isShaking = true;

            float elapsed = 0f;
            Quaternion initialRot = transform.rotation;

            totalRotation += 45f;
            Quaternion targetRot = Quaternion.Euler(0f, totalRotation, 0f);

            while (elapsed < rotationDuration)
            {
                float t = elapsed / rotationDuration;
                transform.rotation = Quaternion.Slerp(initialRot, targetRot, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.rotation = targetRot;
            isShaking = false;
            transform.position = startPos;
        }
    }
}
