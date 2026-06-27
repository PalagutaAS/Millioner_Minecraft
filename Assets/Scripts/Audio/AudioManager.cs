using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Sources")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("Music")]
    [SerializeField] private AudioClip _introClip;
    [SerializeField] private AudioClip _suspenseClip;

    [Header("SFX")]
    [SerializeField] private AudioClip _correctClip;
    [SerializeField] private AudioClip _wrongClip;
    [SerializeField] private AudioClip _winClip;
    [SerializeField] private AudioClip _gameOverClip;
    [SerializeField] private AudioClip _audienceClip;
    [SerializeField] private AudioClip _phoneClip;

    public void PlayIntro() => PlayMusic(_introClip);
    public void PlaySuspense() => PlayMusic(_suspenseClip);
    public void PlayCorrect() => PlaySFX(_correctClip);
    public void PlayWrong() => PlaySFX(_wrongClip);
    public void PlayWin() => PlaySFX(_winClip);
    public void PlayGameOver() => PlaySFX(_gameOverClip);
    public void PlayAudienceMurmur() => PlaySFX(_audienceClip);
    public void PlayPhoneRing() => PlaySFX(_phoneClip);

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        _musicSource.clip = clip;
        _musicSource.Play();
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        _sfxSource.PlayOneShot(clip);
    }
}
