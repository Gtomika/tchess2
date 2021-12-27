using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace TChess2.TChess
{
    class ChessSoundPlayer
    {

        public ChessSoundPlayer()
        {
            moveSoundPlayer = new SoundPlayer(Properties.Resources.move);
            captureSoundPlayer = new SoundPlayer(Properties.Resources.capture);
            checkSoundPlayer = new SoundPlayer(Properties.Resources.check);
        }

        private SoundPlayer moveSoundPlayer;

        private SoundPlayer captureSoundPlayer;

        private SoundPlayer checkSoundPlayer;

        public void PlayMoveSound()
        {
            moveSoundPlayer.Play();
        }

        public void PlayCaptureSound()
        {
            captureSoundPlayer.Play();
        }

        public void PlayCheckSound()
        {
            checkSoundPlayer.Play();
        }
     }
}
