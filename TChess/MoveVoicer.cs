using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using ChessDotNet;

namespace TChess2.TChess
{
    class MoveVoicer
    {

        public MoveVoicer()
        {
            Synthesizer = new SpeechSynthesizer();
        }

        private SpeechSynthesizer Synthesizer;

        /// <summary>
        /// Prints available voices. Hungarian is not supported :(
        /// </summary>
        public void PrintVoices()
        {
            foreach(var voice in Synthesizer.GetInstalledVoices())
            {
                Console.WriteLine(voice.VoiceInfo.Description);
            }
        }

        /// <summary>
        /// Says a move out loud.
        /// </summary>
        /// <param name="move">The move.</param>
        public void VoiceMove(Move move)
        {
           
        }
    }
}
