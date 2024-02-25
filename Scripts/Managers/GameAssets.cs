using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets _i;

    private void Awake()
    {
        if (_i != null && _i != this)
        {
            Destroy(this);
        }
        else
        {
            _i = this;
        }
    }

    public SoundAudioClip[] soundAudioClipArray;

    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }


    // GameOject içerisinde birden fazla ParticlSystem bulunuyorsa bununla çalýþtýr.
    public void PlayParticleSystemInGameObjects(Transform parentPos) 
    {
        int childCount = parentPos.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform childTransform = parentPos.GetChild(i);

            if (childTransform.TryGetComponent<ParticleSystem>(out ParticleSystem particleSystem))
            {
                particleSystem.Play();
            }
        }
    }
}
