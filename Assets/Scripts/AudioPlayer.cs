using System;
using System.Collections.Generic;
using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts
{
    public enum GameMusic
    {
        Ingame
    }

    public enum GameEffect
    {
    }

    public class AudioPlayer : Script
    {
        [Header("Music")]
        public AudioSource MusicAudioSource;
        public List<AudioClip> IngameMusic;

        [Header("Effects")]
        public List<AudioSource> EffectAudioSources;
        public AudioSource EffectAudioSourceReserved;
        public List<AudioClip> Fall;
        public List<AudioClip> Hit;
        public List<AudioClip> ItemUnlocked;
        public List<AudioClip> Swing;

        private float _musicVolume;
        private float _effectsVolume;

        private Action _onPlay = () => { };
        private readonly Dictionary<GameMusic, int> _stopClips = new Dictionary<GameMusic, int>
        {
            { GameMusic.Ingame, 0 }
        };
        private readonly Dictionary<GameMusic, float> _stopTimes = new Dictionary<GameMusic, float>
        {
            { GameMusic.Ingame, 0 }
        };
        private bool _stop, _play;
        private const float VolumeStep = 0.05f;
        private readonly Dictionary<string, int> _effects = new Dictionary<string, int>();

        public static AudioPlayer Instance { get; private set; }

        public void Awake()
        {
            Instance = this;
        }

        public void Start()
        {
            _stopClips[GameMusic.Ingame] = CRandom.GetRandom(IngameMusic.Count);
            ContiniousPlay();
        }

        public void Update()
        {
            if (_stop)
            {
                if (MusicAudioSource.volume > 0)
                {
                    MusicAudioSource.volume -= VolumeStep;

                    if (MusicAudioSource.volume <= 0)
                    {
                        MusicAudioSource.volume = 0;

                        if (IngameMusic.Contains(MusicAudioSource.clip))
                        {
                            _stopClips[GameMusic.Ingame] = IngameMusic.IndexOf(MusicAudioSource.clip);
                            _stopTimes[GameMusic.Ingame] = MusicAudioSource.time;
                        }

                        _stop = false;
                    }
                }
                else
                {
                    _stop = false;
                }
            }
            else if (_play)
            {
                if (_onPlay != null)
                {
                    _onPlay();
                    _onPlay = null;
                }

                if (MusicAudioSource.volume < _musicVolume)
                {
                    MusicAudioSource.volume += VolumeStep;

                    if (MusicAudioSource.volume >= _musicVolume)
                    {
                        MusicAudioSource.volume = _musicVolume;
                        _play = false;
                    }
                }
                else
                {
                    _play = false;
                }
            }
        }

        public void ContiniousPlay()
        {
            if (!MusicAudioSource.isPlaying && MusicAudioSource.clip != null)
            {
                PlayMusicNext(GameMusic.Ingame);
            }

            TaskScheduler.CreateTask(ContiniousPlay, Id, 2);
        }

        public void StopMusic()
        {
            _stop = true;
        }

        public void PlayMusic(GameMusic music)
        {
            _play = true;

            if (music == GameMusic.Ingame && !IngameMusic.Contains(MusicAudioSource.clip))
            {
                _onPlay = () =>
                {
                    MusicAudioSource.clip = IngameMusic[_stopClips[GameMusic.Ingame]];
                    MusicAudioSource.time = _stopTimes[GameMusic.Ingame];
                    MusicAudioSource.loop = false;
                    MusicAudioSource.Play();
                };
            }
        }

        public void PlayMusicNext(GameMusic music)
        {
            _play = true;

            if (music == GameMusic.Ingame)
            {
                _onPlay = () =>
                {
                    var index = IngameMusic.IndexOf(MusicAudioSource.clip);

                    index++;

                    if (index >= IngameMusic.Count)
                    {
                        index = 0;
                    }

                    MusicAudioSource.clip = IngameMusic[index];
                    MusicAudioSource.time = 0;
                    MusicAudioSource.loop = false;
                    MusicAudioSource.Play();
                };
            }
        }

        public void PlayEffect(List<AudioClip> clips, bool reserved = false)
        {
            if (clips.Count == 0) return;

            PlayEffect(clips[CRandom.GetRandom(clips.Count)]);
        }

        public void PlayEffect(AudioClip clip, float pitch = 1, bool reserved = false, bool loop = false)
        {
            if (_effects.ContainsKey(clip.name) && _effects[clip.name] == Time.frameCount) return;

            var played = false;

            foreach (var audioSource in EffectAudioSources)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = clip;
                    audioSource.loop = false;
                    audioSource.pitch = pitch;
                    audioSource.Play();
                    played = true;
                    break;
                }
            }

            if (!played && reserved && !EffectAudioSourceReserved.isPlaying)
            {
                EffectAudioSourceReserved.PlayOneShot(clip);
                played = true;
            }

            if (played)
            {
                if (_effects.ContainsKey(clip.name))
                {
                    _effects[clip.name] = Time.frameCount;
                }
                else
                {
                    _effects.Add(clip.name, Time.frameCount);
                }
            }
        }

        public void PlayEffectLoop(AudioClip clip, float pitch = 1)
        {
            PlayEffect(clip, pitch, false, loop: true);
        }

        public void StopEffectLoop(AudioClip clip)
        {
            foreach (var audioSource in EffectAudioSources)
            {
                if (audioSource.clip == clip)
                {
                    audioSource.Stop();
                    audioSource.loop = false;
                    audioSource.clip = null;
                }
            }
        }

        public void SetVolume(bool sound)
        {
            SetVolume(sound, sound);
        }

        public void SetVolume(bool music, bool effects)
        {
            _musicVolume = 0.5f;
            _effectsVolume = 1.0f;

            MusicAudioSource.volume = _musicVolume;
            MusicAudioSource.mute = !music;

            EffectAudioSourceReserved.volume = _effectsVolume;
            EffectAudioSourceReserved.mute = !effects;

            foreach (var audioSource in EffectAudioSources)
            {
                audioSource.volume = _effectsVolume;
                audioSource.mute = !effects;
            }
        }
    }
}