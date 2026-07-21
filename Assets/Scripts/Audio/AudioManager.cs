using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Sources")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("Music")]
    [SerializeField] private AudioClip _backgroundClip;

    [Header("SFX")]    
    [SerializeField] private AudioClip _suspenseClip;
    [SerializeField] private AudioClip _correctClip;
    [SerializeField] private AudioClip _wrongClip;
    [SerializeField] private AudioClip _nextQuestionClip;
    [SerializeField] private AudioClip _pressButtonHintClip;

    public void PlayBackgroundMusic(bool enable = true)
    {
        if (enable)
            PlayMusic(_backgroundClip);
        else
            _musicSource.Stop();
    }

    public void PlaySuspense() => PlaySFX(_suspenseClip);
    public void PlayCorrect() => PlaySFX(_correctClip);
    public void PlayWrong() => PlaySFX(_wrongClip);
    public void PlayFirstQuestion() => PlaySFX(_nextQuestionClip);
    public void PlayPressButtonHint() => PlaySFX(_pressButtonHintClip);

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (clip == _musicSource.clip && _musicSource.isPlaying) return;
        _musicSource.clip = clip;
        _musicSource.Play();
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        _sfxSource.PlayOneShot(clip);
    }
}
