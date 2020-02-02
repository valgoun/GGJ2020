using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundTypes
{
    INPUT_PICKUP, //
    INPUT_BREAK, //
    ENEMY_ATTACK_SWORD, //
    ENEMY_ATTACK_GUN, //
    ENEMY_DIE, //
    PLAYER_ATTACK_MELEE, //
    PLAYER_ATTACK_GUN, //
    PLAYER_HIT, //
    PLAYER_DIE, //
    MENU_SELECTION //
}

public class SoundManager : SerializedMonoBehaviour
{
    public static SoundManager Instance;

    public struct SoundStruct
    {
        public AudioClip Sound;
        public float Volume;
    }
    public Dictionary<SoundTypes, SoundStruct> Sounds;

    public AudioSource DefaultSource;

    private void Awake()
    {
        if (Instance)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void PlaySound(SoundTypes sound)
    {
        PlaySound(sound, DefaultSource);
    }

    public void PlaySound(SoundTypes sound, AudioSource source)
    {
        if (Sounds.ContainsKey(sound))
        {
            SoundStruct info = Sounds[sound];
            source.clip = info.Sound;
            source.volume = info.Volume;
            source.Play();
        }
    }
}
