using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    void Start()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;
            s.source.volume = s.volume;
        }
        PlaySound("MainTheme");
    }

    public void PlaySound(string name)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == name)
            {
                s.source.Play();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
