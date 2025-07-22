using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField]
    private AudioSource effectSource;
    [SerializeField]
    private AudioClip jumpClip;
    [SerializeField]
    private AudioClip tapClip;
    [SerializeField]
    private AudioClip hurtClip;
    private bool hasPlayEffectSound=false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public bool HasPlayEffectSound()
    {
        return hasPlayEffectSound;
    }
    public void SetHasPlayEffectSound(bool value)
    {
        hasPlayEffectSound = value;
    }

    void Start()
    {
        effectSource.Stop();
        hasPlayEffectSound = true;
    }
    
    public void PlayJumpClip()
    {
        effectSource.PlayOneShot(jumpClip);
    }
    public void PlayTapClip()
    {
        effectSource.PlayOneShot(tapClip);
    }
    public void PlayHurtClip()
    {
        effectSource.PlayOneShot(hurtClip);
    }
}
