using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Mathematics;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager INSTANCE;

    private CinemachineFreeLook freeLookCam;
    private CinemachineVirtualCamera topRig, midRig, botRig;
    private CinemachineBasicMultiChannelPerlin topChannel, midChannel, botChannel;
    private Vector2 topTarget, midTarget, botTarget;
    
    [SerializeField] private float smoothSpeed;

    private float timer;
    private bool stopping;

    void Awake ()
    {
        INSTANCE = this;

        freeLookCam = GetComponent<CinemachineFreeLook>();
    
        topRig = freeLookCam.GetRig(0);
        midRig = freeLookCam.GetRig(1);
        botRig = freeLookCam.GetRig(2);

        topChannel = topRig.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        midChannel = midRig.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        botChannel = botRig.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

    }

    void Start() 
    {
        StopShake();
    }

    void Update() 
    {
        if (timer > 0) 
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
                StopShake();
        }

        if (topChannel.m_AmplitudeGain != topTarget.x) 
        {
            SmoothUpdateShake();
        }
    }

    public void ShakeOnce(float intensity, float frequency, float time) 
    {
        stopping = false;
        Vector2 newTarget = new Vector2(intensity, frequency);
        topTarget =  newTarget;
        midTarget =  newTarget;
        botTarget =  newTarget;
        timer = time;
    }

    public void StopShake() 
    {
        ShakeOnce(0, 0, 0);
        stopping = true;
    }

    private void SmoothUpdateShake() 
    {
        float newSmoothSpeed = smoothSpeed;

        if (stopping)
            newSmoothSpeed *= 0.35f;

        topChannel.m_AmplitudeGain = Mathf.Lerp(topChannel.m_AmplitudeGain, topTarget.x, newSmoothSpeed * Time.deltaTime);
        topChannel.m_FrequencyGain = Mathf.Lerp(topChannel.m_FrequencyGain, topTarget.y, newSmoothSpeed * Time.deltaTime);
            
        midChannel.m_AmplitudeGain = Mathf.Lerp(midChannel.m_AmplitudeGain, midTarget.x, newSmoothSpeed * Time.deltaTime);
        midChannel.m_FrequencyGain = Mathf.Lerp(midChannel.m_FrequencyGain, midTarget.y, newSmoothSpeed * Time.deltaTime);

        botChannel.m_AmplitudeGain = Mathf.Lerp(botChannel.m_AmplitudeGain, botTarget.x, newSmoothSpeed * Time.deltaTime);
        botChannel.m_FrequencyGain = Mathf.Lerp(botChannel.m_FrequencyGain, botTarget.y, newSmoothSpeed * Time.deltaTime);
    }
}
