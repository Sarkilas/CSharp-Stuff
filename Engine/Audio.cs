using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ingenia.Data;
using IrrKlang;
using Microsoft.Xna.Framework;

namespace Ingenia.Engine
{
    /// <summary>
    /// Allows processing and playing audio files.
    /// </summary>
    static class Audio
    {
        // Sound engine
        static ISoundEngine Engine = new ISoundEngine(),
            AmbienceEngine = new ISoundEngine(),
            MusicEngine = new ISoundEngine();

        // Sources
        static Dictionary<string, ISoundSource> Sources = new Dictionary<string, ISoundSource>(),
            AmbienceSources = new Dictionary<string, ISoundSource>(),
            MusicSources = new Dictionary<string, ISoundSource>();

        // Music and ambience
        static string Music = null,
            Ambience = null;

        /// <summary>
        /// Updates the audio class.
        /// Required for releasing system resources.
        /// </summary>
        public static void Update(GameTime gameTime)
        {
            Engine.SoundVolume = Database.settings.SoundVolume;
            AmbienceEngine.SoundVolume = Database.settings.AmbienceVolume;
            if(Ambience != null)
                if (AmbienceSources.ContainsKey(Ambience))
                    if (!AmbienceEngine.IsCurrentlyPlaying(Ambience))
                        AmbienceEngine.Play2D(Ambience);
            MusicEngine.SoundVolume = Database.settings.MusicVolume;
            if(Music != null)
                if (MusicSources.ContainsKey(Music))
                    if (!MusicEngine.IsCurrentlyPlaying(Music))
                        MusicEngine.Play2D(Music);
        }

        /// <summary>
        /// Plays a sound file.
        /// </summary>
        /// <param name="key">The data key for loading.</param>
        public static void PlaySound(string key)
        {
            if(!Sources.ContainsKey(key))
                Sources.Add(key, Engine.AddSoundSourceFromMemory(GameData.Data[key], key));

            Engine.Play2D(key);
        }

        /// <summary>
        /// Plays an ambience file.
        /// </summary>
        /// <param name="key">The data key for loading.</param>
        public static void PlayAmbience(string key)
        {
            if (Ambience == key) return;
            if (!AmbienceSources.ContainsKey(key))
                AmbienceSources.Add(key, AmbienceEngine.AddSoundSourceFromMemory(GameData.Data[key], key));

            AmbienceEngine.StopAllSounds();
            AmbienceEngine.Play2D(key);
            Ambience = key;
        }

        /// <summary>
        /// Stops playing any ambience.
        /// </summary>
        public static void StopAmbience() { Ambience = null; AmbienceEngine.StopAllSounds(); }

        /// <summary>
        /// Plays a music file.
        /// </summary>
        /// <param name="key">The data key for loading.</param>
        public static void PlayMusic(string key)
        {
            if (Music == key) return;
            if (!MusicSources.ContainsKey(key))
                MusicSources.Add(key, MusicEngine.AddSoundSourceFromMemory(GameData.Data[key], key));

            MusicEngine.StopAllSounds();
            MusicEngine.Play2D(key);
            Music = key;
        }

        /// <summary>
        /// Stops playing any music.
        /// </summary>
        public static void StopMusic() { Music = null; MusicEngine.StopAllSounds(); }
    }
}
