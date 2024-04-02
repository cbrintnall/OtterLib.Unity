using UnityEngine;

public static class GlobalAudioExtensions
{
    public static void PlayAs(this AudioClip clip, AudioPayload audioPayload)
    {
        audioPayload.Clip = clip;
        audioPayload.Play();
    }
}
