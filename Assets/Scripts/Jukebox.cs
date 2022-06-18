using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Jukebox : MonoBehaviour
{
    [SerializeField] private List<AudioClip> audioClips = new List<AudioClip>();
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        DirectoryInfo songDir = new DirectoryInfo(Application.dataPath+"/Sound/Resources/Song/Resources");
        FileInfo[] fileNames = songDir.GetFiles("*.mp3");
        foreach (FileInfo file in fileNames)
        {
            AudioClip newClip = (AudioClip) Resources.Load(file.Name.Replace(".mp3", ""));
            audioClips.Add(newClip);
        }

        audioSource.clip = audioClips[Random.Range(0, audioClips.Count - 1)];
        audioSource.Play();
    }
}
