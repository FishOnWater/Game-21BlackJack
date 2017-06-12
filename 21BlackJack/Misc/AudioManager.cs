using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace _21BlackJack.Misc
{
    class AudioManager : GameComponent
    {
        #region Fields

        static AudioManager audioManager = null;
        public static AudioManager Instance
        {
            get
            {
                return audioManager;
            }
        }
        static readonly string soundAssetLocation = "Sound/";

        //Audio Data
        Dictionary<string, SoundEffectInstance> soundBank;
        Dictionary<string, Song> musicBank;

        #endregion

        #region Inicializações

        private AudioManager(Game game)
            : base(game) { }

        public static void Initialize(Game game)
        {
            audioManager = new AudioManager(game);
            audioManager.soundBank = new Dictionary<string, SoundEffectInstance>();
            audioManager.musicBank = new Dictionary<string, Song>();

            game.Components.Add(audioManager);
        }

        #endregion

        #region Loading Methods

        public static void LoadSound(string contentName, string alias)
        {
            SoundEffect soundeffect = audioManager.Game.Content.Load<SoundEffect>(soundAssetLocation + contentName);
            SoundEffectInstance soundEffectInstance = soundeffect.CreateInstance();

            if (!audioManager.soundBank.ContainsKey(alias))
            {
                audioManager.soundBank.Add(alias, soundEffectInstance);
            }
        }

        public static void LoadSong(string contentName, string alias)
        {
            Song song = audioManager.Game.Content.Load<Song>(soundAssetLocation + contentName);

            if (!audioManager.musicBank.ContainsKey(alias))
            {
                audioManager.musicBank.Add(alias, song);
            }
        }

        public static void LoadSounds() //Vai ser necessário trocar os nomes dos sons aqui neste método
        {
            LoadSound("Bet", "Bet");
            LoadSound("CardFlip", "Flip");
            LoadSound("CardShuffle", "Shuffle");
            LoadSound("Deal", "Deal"); 
        }

        public static void LoadMusic() //Também necessário mudar
        {
            LoadSong("InGameSong_Loop", "InGameSong_Loop");
            LoadSong("MenuMusic_Loop", "MenuMusic_Loop");
        }
        #endregion

        #region Sound Methods
        public SoundEffectInstance this[string soundName]
        {
            get
            {
                if (audioManager.soundBank.ContainsKey(soundName))
                {
                    return audioManager.soundBank[soundName];
                }
                else
                {
                    return null;
                }
            }
        }

        public static void PlaySound(string soundName)
        {
            if (audioManager.soundBank.ContainsKey(soundName))
            {
                audioManager.soundBank[soundName].Play();
            }
        }

        public static void PlaySound(string soundName, bool isLooped)
        {
            if (audioManager.soundBank.ContainsKey(soundName))
            {
                if (audioManager.soundBank[soundName].IsLooped != isLooped)
                    audioManager.soundBank[soundName].IsLooped = isLooped;
            }
            audioManager.soundBank[soundName].Play();
        }

        public static void PlaySound(string soundName, bool isLooped, float volume)
        {
            if (audioManager.soundBank[soundName].IsLooped != isLooped)
                audioManager.soundBank[soundName].IsLooped = isLooped;
            audioManager.soundBank[soundName].Volume = volume;
            audioManager.soundBank[soundName].Play();
        }

        public static void StopSound(string soundName)
        {
            if (audioManager.soundBank.ContainsKey(soundName))
                audioManager.soundBank[soundName].Stop();
        }

        public static void StopSounds()
        {
            foreach (SoundEffectInstance sound in audioManager.soundBank.Values)
            {
                if (sound.State != SoundState.Stopped)
                    sound.Stop();
            }
        }

        public static void PauseResumeSounds(bool resumeSounds)
        {
            SoundState state = resumeSounds ? SoundState.Paused : SoundState.Playing;

            foreach(SoundEffectInstance sound in audioManager.soundBank.Values)
            {
                if(sound.State == state)
                {
                    if (resumeSounds)
                        sound.Resume();
                    else
                        sound.Pause();
                }
            }
        }

        public static void PlayMusic(string musicSoundName)
        {
            if (audioManager.musicBank.ContainsKey(musicSoundName))
            {
                if (MediaPlayer.State != MediaState.Stopped)
                    MediaPlayer.Stop();
            }
            MediaPlayer.IsRepeating = true;

            MediaPlayer.Play(audioManager.musicBank[musicSoundName]);
        }

        public static void StopMusic()
        {
            if (MediaPlayer.State != MediaState.Stopped)
                MediaPlayer.Stop();
        }
        #endregion

        #region Instance Disposal Methods
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    foreach(var item in soundBank)
                    {
                        item.Value.Dispose();
                    }
                    soundBank.Clear();
                    soundBank = null;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
        #endregion
    }
}
