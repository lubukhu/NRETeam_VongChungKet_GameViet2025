// [tooltips] Quản lý và phát tất cả các loại âm thanh trong game, bao gồm SFX, BGM và các âm thanh sự kiện đặc biệt.
using UnityEngine;
using Obvious.Soap;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [Tooltip("AudioSource chuyên dùng để phát nhạc nền (BGM).")]
    [SerializeField] private AudioSource bgmAudioSource;
    private AudioSource _sfxAudioSource;

    [Header("Sound Effect Clips")]
    [SerializeField] private AudioClip cardDragSound;
    [SerializeField] private AudioClip cardSwipeSound;
    [SerializeField] private AudioClip cardFlipSound;
    [SerializeField] private AudioClip cardShuffleSound;
    
    [Header("Background Music")]
    [Tooltip("Danh sách các bản nhạc nền sẽ được phát luân phiên.")]
    [SerializeField] private List<AudioClip> backgroundMusicPlaylist;
    private int _currentTrackIndex = 0;

    [Header("Ending SFX")]
    [SerializeField] private AudioClip goodEndingSfx;
    [SerializeField] private AudioClip badEndingSfx;

    private void Awake()
    {
        _sfxAudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (backgroundMusicPlaylist.Count > 0)
        {
            PlayNextTrackInPlaylist();
        }
    }
    
    private void Update()
    {
        if (bgmAudioSource != null && !bgmAudioSource.isPlaying && backgroundMusicPlaylist.Count > 0)
        {
            PlayNextTrackInPlaylist();
        }
    }
    
    private void PlayNextTrackInPlaylist()
    {
        if (bgmAudioSource == null || backgroundMusicPlaylist.Count == 0) return;

        bgmAudioSource.clip = backgroundMusicPlaylist[_currentTrackIndex];
        bgmAudioSource.Play();
        
        _currentTrackIndex = (_currentTrackIndex + 1) % backgroundMusicPlaylist.Count;
    }
    
    public void PlayCardDragSound() { PlaySfx(cardDragSound); }
    public void PlayCardSwipeSound() { PlaySfx(cardSwipeSound); }
    public void PlayCardFlipSound() { PlaySfx(cardFlipSound); }
    public void PlayCardShuffleSound() { PlaySfx(cardShuffleSound); }
    public void PlayGoodEndingSfx()
    {
        PlaySfx(goodEndingSfx);
    }

    public void PlayBadEndingSfx()
    {
        PlaySfx(badEndingSfx);
    }
    
    public void PlaySfx(AudioClip clip)
    {
        if (clip != null && _sfxAudioSource != null)
        {
            _sfxAudioSource.PlayOneShot(clip);
        }
    }
}