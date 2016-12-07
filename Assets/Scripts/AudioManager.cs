using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class AudioManager : MonoBehaviour {

    [System.Serializable]
    public class AudioList
    {
        public List<AudioClip> Clips;
    }


    public List<string> SoundKeys;

    public List<AudioList> SoundValues;


    private Dictionary<string, List<AudioClip>> m_SoundDictionary;

    public Dictionary<string, AudioList> test;
    void Start()
    {
        
        PopulateSoundDictionary();
    }

    private void PopulateSoundDictionary()
    {
        m_SoundDictionary = new Dictionary<string, List<AudioClip>>();

        Assert.AreEqual<int>(SoundKeys.Count, SoundValues.Count);

        for (int i = 0; i < SoundKeys.Count; i++)
        {
            m_SoundDictionary[SoundKeys[i]] = SoundValues[i].Clips;
        }
    }

    public void PlaySoundAtLocation(string soundKey, Vector3 sourcePosition)
    {
        //Get Sound List Associated to Key
        var sounds = m_SoundDictionary[soundKey];
        if (sounds.Count == 0) return;
        //Play renadom sound
        int iAudio = Mathf.RoundToInt(UnityEngine.Random.Range(0, sounds.Count - 1));
        AudioSource.PlayClipAtPoint(sounds[iAudio], sourcePosition);
    }
}
