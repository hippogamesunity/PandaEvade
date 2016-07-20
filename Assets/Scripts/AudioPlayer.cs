using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts
{
    public enum GameMusic
    {
        Menu,
        Battle,
        Victory
    }

    public enum GameEffect
    {
    }

    public class AudioPlayer : SingleScript<AudioPlayer>
    {
        [Header("Music")]
        public AudioSource MusicAudioSource;
        public List<AudioClip> MenuMusic;
        public List<AudioClip> BattleMusic;
        public List<AudioClip> VictoryMusic;

        [Header("Effects")]
        public List<AudioSource> EffectAudioSources;
        public AudioSource EffectAudioSourcesReserved;

        private float _musicVolume;
        private float _effectsVolume;

        private Action _onPlay = () => { };
        private readonly Dictionary<GameMusic, int> _stopClips = new Dictionary<GameMusic, int>
        {
            { GameMusic.Menu, 0 },
            { GameMusic.Battle, 0 },
            { GameMusic.Victory, 0 }
        };
        private readonly Dictionary<GameMusic, float> _stopTimes = new Dictionary<GameMusic, float>
        {
            { GameMusic.Menu, 0 },
            { GameMusic.Battle, 0 },
            { GameMusic.Victory, 0 }
        };
        private bool _stop, _play;
        private const float VolumeStep = 0.05f;
        
        public void Start()
        {
            _stopClips[GameMusic.Menu] = CRandom.GetRandom(MenuMusic.Count);
            _stopClips[GameMusic.Battle] = CRandom.GetRandom(BattleMusic.Count);
            _stopClips[GameMusic.Victory] = CRandom.GetRandom(VictoryMusic.Count);
            
            SetVolume();
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

                        if (MenuMusic.Contains(MusicAudioSource.clip))
                        {
                            _stopClips[GameMusic.Menu] = MenuMusic.IndexOf(MusicAudioSource.clip);
                            _stopTimes[GameMusic.Menu] = MusicAudioSource.time;
                        }
                        else if (BattleMusic.Contains(MusicAudioSource.clip))
                        {
                            _stopClips[GameMusic.Battle] = BattleMusic.IndexOf(MusicAudioSource.clip);
                            _stopTimes[GameMusic.Battle] = MusicAudioSource.time;
                        }
                        else if (VictoryMusic.Contains(MusicAudioSource.clip))
                        {
                            _stopClips[GameMusic.Victory] = VictoryMusic.IndexOf(MusicAudioSource.clip);
                            _stopTimes[GameMusic.Victory] = 0;
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
                var clips = MenuMusic.Contains(MusicAudioSource.clip) ? MenuMusic : BattleMusic.Contains(MusicAudioSource.clip) ? BattleMusic : VictoryMusic;

                if (clips.Count > 1)
                {
                    clips = clips.Where(i => i != MusicAudioSource.clip).ToList();
                }

                var clip = clips[CRandom.GetRandom(clips.Count)];

                MusicAudioSource.PlayOneShot(clip);
            }

            TaskScheduler.CreateTask(ContiniousPlay, Id, 2);
        }
       
        public void StopMusic()
        {
            _stop = true;
        }

        public void PlayMusic(GameMusic music, int index = -1)
        {
            _play = true;
            
            if (music == GameMusic.Menu && !MenuMusic.Contains(MusicAudioSource.clip))
            {
                _onPlay = () =>
                {
                    MusicAudioSource.clip = MenuMusic[_stopClips[GameMusic.Menu]];
                    MusicAudioSource.time = _stopTimes[GameMusic.Menu];
                    MusicAudioSource.loop = false;
                    MusicAudioSource.Play();
                };
            }
            else if (music == GameMusic.Battle && !BattleMusic.Contains(MusicAudioSource.clip))
            {
                _onPlay = () =>
                {
                    MusicAudioSource.clip = BattleMusic[_stopClips[GameMusic.Battle]];
                    MusicAudioSource.time = _stopTimes[GameMusic.Battle];
                    MusicAudioSource.loop = false;
                    MusicAudioSource.Play();
                };
            }
            else if (music == GameMusic.Victory && !VictoryMusic.Contains(MusicAudioSource.clip))
            {
                _onPlay = () =>
                {
                    MusicAudioSource.clip = VictoryMusic[index];
                    MusicAudioSource.time = 0;
                    MusicAudioSource.loop = true;
                    MusicAudioSource.Play();
                };
            }
        }

        public void PlayEffect(List<AudioClip> clips, bool reserved = false)
        {
            if (clips.Count == 0) return;

            PlayEffect(clips[CRandom.GetRandom(clips.Count)]);
        }

        public void PlayEffect(AudioClip clip, bool reserved = false)
        {
            foreach (var audioSource in EffectAudioSources)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(clip);

                    return;
                }
            }

            if (reserved && !EffectAudioSourcesReserved.isPlaying)
            {
                EffectAudioSourcesReserved.PlayOneShot(clip);
            }
        }

        public void SetVolume()
        {
            _musicVolume = Profile.Instance.Sound.Bool ? 0.5f : 0;
            _effectsVolume = _musicVolume;

            MusicAudioSource.volume = _musicVolume;

            foreach (var audioSource in EffectAudioSources) // TODO: Reserved?
            {
                audioSource.volume = _effectsVolume;
            }
        }
    }
}