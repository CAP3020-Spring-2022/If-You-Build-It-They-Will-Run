using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BuildMusic : MonoBehaviour
{
    public GameObject camSwitchObject;
    camswitch camSwitch;
    AudioSource music;
    public AudioMixerSnapshot Build;
    public AudioMixerSnapshot Run;

    void Awake()
    {
        camSwitch = camSwitchObject.GetComponent<camswitch>();
        music = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (camSwitch.buildphase)
        {
            Build.TransitionTo(.1f);
        }
        else
        {
            Run.TransitionTo(.1f);
        }
    }
}