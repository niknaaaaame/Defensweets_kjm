using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip onClip;
    public AudioClip offClip;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Toggle>().onValueChanged.AddListener(PlayToggleSound);
    }

    void PlayToggleSound(bool isOn)
    {
        if (audioSource == null) return;

        if (isOn && onClip != null)
            audioSource.PlayOneShot(onClip);
        else if (!isOn && offClip != null)
            audioSource.PlayOneShot(offClip);
    }

}
