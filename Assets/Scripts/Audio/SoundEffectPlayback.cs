using UnityEngine;

public static class SoundEffectPlayback
{
    public static void Play(SoundEffectId soundEffectId)
    {
        SoundEffectConfig config = SoundEffectConfig.Load();
        if (config == null)
        {
            return;
        }

        if (!config.TryGetClip(soundEffectId, out AudioClip clip, out float volume) || clip == null)
        {
            return;
        }

        Vector3 playPosition = ResolvePlayPosition();
        AudioSource.PlayClipAtPoint(clip, playPosition, volume);
    }

    private static Vector3 ResolvePlayPosition()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            return mainCamera.transform.position;
        }

        AudioListener listener = Object.FindObjectOfType<AudioListener>();
        if (listener != null)
        {
            return listener.transform.position;
        }

        return Vector3.zero;
    }
}
