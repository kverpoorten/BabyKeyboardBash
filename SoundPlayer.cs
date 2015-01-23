using System;
using System.IO;
using System.Windows.Forms;

namespace BabyKeyboardBash
{
    public class SoundPlayer
    {
        private string m_AlphabetSoundsPath;
        private string m_RandomSoundsPath;
        private System.Media.SoundPlayer m_SoundPlayer;
        private Random m_RandomGenerator;

        public SoundPlayer(string alphabetSoundsPath, string randomSoundsPath)
        {
            m_SoundPlayer = new System.Media.SoundPlayer();
            m_AlphabetSoundsPath = alphabetSoundsPath;
            m_RandomSoundsPath = randomSoundsPath;
            m_RandomGenerator = new Random(DateTime.Now.Millisecond);
        }

        public void Play(Keys key)
        {
            //play the sound matching with this key
            bool playRandomSound = true;
            if ((key >= Keys.A) && (key <= Keys.Z))
            {
                //play a letter sound!
                var letterSoundFile = Path.Combine(m_AlphabetSoundsPath, key.ToString().ToUpper() + ".wav");
                if (File.Exists(letterSoundFile))
                {
                    playRandomSound = false;
                    m_SoundPlayer.SoundLocation = letterSoundFile;
                    m_SoundPlayer.Play();
                }
            }

            if (playRandomSound)
            {
                //play a random other sound
                var randomSounds = Directory.GetFiles(m_RandomSoundsPath, "*.wav");
                if (randomSounds.Length > 0)
                {
                    var randomSoundToPlay = m_RandomGenerator.Next(0, randomSounds.Length - 1);
                    m_SoundPlayer.SoundLocation = randomSounds[randomSoundToPlay];
                    m_SoundPlayer.Play();
                }
            }
        }
    }
}
