using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class AudioManager : MonoBehaviour
{
    [Header("Sources")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("Music")]
    [SerializeField] private AudioClip _backgroundClip;
    
    [Header("Objects")]
    [SerializeField] private Slider _slider;

    [Header("SFX")]
    [SerializeField] private AudioClip _suspenseClip;
    [SerializeField] private AudioClip _correctClip;
    [SerializeField] private AudioClip _wrongClip;
    [SerializeField] private AudioClip _nextQuestionClip;
    [SerializeField] private AudioClip _pressButtonHintClip;
    
    [Inject] private SaveService _saveService;
    
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
    
    private void OnEnable()
    {
        if (_slider != null)
            _slider.onValueChanged.AddListener(SetVolume);
    }
    
    private void OnDisable()
    {
        if (_slider != null)
            _slider.onValueChanged.RemoveListener(SetVolume);
    }

    private void SetVolume(float volume)
    {
        _musicSource.volume = volume;
        _sfxSource.volume = volume;

        _saveService.Data.volume = volume;
    }

    public void UpdateSlider(float dataVolume)
    {
        _musicSource.volume = dataVolume;
        _sfxSource.volume = dataVolume;
        _slider.value = dataVolume;
    }
}
