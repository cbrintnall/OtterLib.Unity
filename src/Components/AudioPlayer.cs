using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioPayload Payload;

    [SerializeField]
    bool playOnAwake;

    void Start()
    {
        if (playOnAwake)
        {
            Play();
        }
    }

    public void Play()
    {
        Payload.Play();
    }
}
