using UnityEngine;

public class OptionMenu : MonoBehaviour
{
    public void SetVolume(float volume)
    {
        SoundManager.Instance.audioMixer.SetFloat("volume", Mathf.Log10(volume) * 20);
    }

    public void Sound()
    {
        AudioListener.pause = !AudioListener.pause;
    }
}
