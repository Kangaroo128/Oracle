using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace Oracle
{
    class OracleTester
    {
        static void Main(string[] args)
        {
            //Initialize a new instance of the SpeechSynthesizer.
            SpeechSynthesizer synth = new SpeechSynthesizer();

            //Select a voice.
            synth.SelectVoiceByHints(VoiceGender.Female);

            synth.Rate = -2;

            //Configure the audio output.
            synth.SetOutputToDefaultAudioDevice();

            //Speak text string synchronously.
            string articleText = WebInteractions.Wiktionary.ReadArticle("test");
            if (articleText != null)
            {
                synth.Speak(articleText);
            }
            else
            {
                synth.Speak("I could not locate any information on that topic.");
            }
        }
    }
}
