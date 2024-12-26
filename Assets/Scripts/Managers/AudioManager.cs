using System.Collections.Generic;
using UnityEngine;

public class AudioManager
{
    private Dictionary<string, AudioClip> audioClips;
    private AudioSource audioSource;

    // 오디오 매니저 초기화
    public void Init()
    {
        // AudioSource 추가
        GameObject audioObject = new GameObject("AudioManager_AudioSource");
        audioSource = audioObject.AddComponent<AudioSource>();
        Object.DontDestroyOnLoad(audioObject);

        // 기본 볼륨 설정
        audioSource.volume = 1.0f;
        audioSource.playOnAwake = false; // 시작 시 재생하지 않도록 설정

        // AudioClip Dictionary 초기화
        audioClips = new Dictionary<string, AudioClip>();

        // 각 오디오 클립 로드
        LoadAudioClips("Audio/GameOver", "GameOver");
        LoadAudioClips("Audio/HealthDown", "HealthDown");
        LoadAudioClips("Audio/Jump", "Jump");
        LoadAudioClips("Audio/Pickup", "Pickup");
    }

    // 특정 폴더의 모든 오디오 클립 로드
    private void LoadAudioClips(string path, string key)
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>(path);
        foreach (var clip in clips)
        {
            Debug.Log($"Loaded clip: {clip.name} from path: {path}");
            if (!audioClips.ContainsKey(key))
                audioClips[key] = clip; // 중복 방지
        }
    }


    // 효과음 재생
    public void PlaySound(string key, float volume = 1.0f)
    {
        if (audioClips.TryGetValue(key, out var clip))
        {
            audioSource.PlayOneShot(clip, volume);
        }
        else
        {
            Debug.LogWarning($"AudioManager: {key} 오디오 클립이 없습니다.");
        }
    }

    // BGM 재생 (루프)
    public void PlayBGM(string key, float volume = 1.0f)
    {
        if (audioClips.TryGetValue(key, out var clip))
        {
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.volume = volume;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"AudioManager: {key} BGM 클립이 없습니다.");
        }
    }

    // BGM 중지
    public void StopBGM()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    // 오디오 클립 캐시 초기화
    public void Clear()
    {
        audioClips.Clear();
        audioSource.Stop();
    }
}
