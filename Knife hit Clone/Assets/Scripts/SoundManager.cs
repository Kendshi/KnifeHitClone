using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager singltone;
    public AudioSource[] sounds;

    private void Awake()
    {
        sounds = new AudioSource[2];

        singltone = this;
    }

    void Start()
    {
        sounds = GetComponents<AudioSource>();
    }

}
