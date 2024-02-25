using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    public enum Sound
    {
        PlayerMove,
        KelesShoot,
        KelesReload,
        PlayerEmptyGun,
        M4A4Shoot,
        M4A4Reload,
        EnemyHit,
        EnemyDie,
        Bomb,
        AmmoCollected,
        HealthCollected,
        GrenadeCollected,
        HostageSelect,
        BarrelExplosion
    }

    private static Dictionary<Sound, float> soundTimerDictionary;

    public static void Initilaize()
    {
        soundTimerDictionary = new Dictionary<Sound, float>();
        soundTimerDictionary[Sound.PlayerEmptyGun] = 0f;
    }
       

    public static void PlaySound(Sound sound)
    {
        if (CanPlaySound(sound))
        {
            GameObject soundGameObject = new GameObject("Sound");
            soundGameObject.tag = "Sound";
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.PlayOneShot(GetAudioClip(sound));
            Object.Destroy(soundGameObject, 3f);
        }
    }

    private static bool CanPlaySound(Sound sound)
    {

        float lastTimePlayed;
        float playerSoundTimerMax;

        switch (sound)
        {
            case Sound.PlayerEmptyGun:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    lastTimePlayed = soundTimerDictionary[sound];
                    playerSoundTimerMax = 0.5f;
                    if (lastTimePlayed + playerSoundTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            default:
                return true;

          

        }
    }
    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets._i.soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }
        Debug.LogError("Sound " + sound + "Not Found!");
        return null;


    }

}
