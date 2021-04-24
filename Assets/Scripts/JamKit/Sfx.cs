using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace JamKit
{
    public class Sfx : SingletonBehaviour<Sfx>
    {
        private AudioSource _commonAudioSource;
        private AudioSource _musicAudioSource;

        private const float SfxVolume = 0.5f;
        private const float MusicVolume = 0.5f;

        private SfxDatabase _database;
        
        private void Awake()
        {
            _commonAudioSource = gameObject.AddComponent<AudioSource>();
            _commonAudioSource.volume = SfxVolume;
            _musicAudioSource = gameObject.AddComponent<AudioSource>();
            _musicAudioSource.volume = SfxVolume;
            _database = Resources.Load<SfxDatabase>("Sfx/SfxDatabase");
            if (Camera.main != null)
            {
                transform.SetParent(Camera.main.transform);
            }
        }

        [CanBeNull]
        private AudioClip GetClip(string clipName)
        {
            foreach (AudioClip audioClip in _database.Clips)
            {
                if (audioClip.name == clipName)
                {
                    return audioClip;
                }
            }
            Debug.LogError($"Audioclip not found: {clipName}");
            return null;
        }

        public void PlayRandom(string clipPrefix)
        {
            List<AudioClip> clips = _database.Clips.Where(x => x.name.StartsWith(clipPrefix)).ToList();

            if (clips.Count > 0)
            {
                _commonAudioSource.PlayOneShot(clips.GetRandom());
            }
            else
            {
                Debug.LogError($"Couldn't find any clips with prefix {clipPrefix}");
            }
        }

        public void Play(string clipName)
        {
            AudioClip clip = GetClip(clipName);
            if (clip != null)
            {
                _commonAudioSource.PlayOneShot(clip);
            }
        }

        public void ChangeMusicTrack(string clipName)
        {
            _musicAudioSource.clip = GetClip(clipName);
            _musicAudioSource.volume = MusicVolume;
        }

        public void FadeOutMusic(float duration)
        {
            AnimationCurve linearCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            Curve.Tween(linearCurve,
                duration,
                t => { _musicAudioSource.volume = Mathf.Lerp(MusicVolume, 0f, t); },
                () => { _musicAudioSource.volume = 0f; });
        }

    }

}