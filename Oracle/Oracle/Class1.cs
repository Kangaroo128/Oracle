using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace Oracle
{
    class Program
    {
        static void Main(string[] args)
        {
            string articleText = WebInteractions.Wikipedia.ReadArticle("Texas Toast");

            //Initialize a new instance of the SpeechSynthesizer.
            SpeechSynthesizer synth = new SpeechSynthesizer();

            //Select a voice.
            synth.SelectVoiceByHints(VoiceGender.Female);

            //Configure the audio output.
            synth.SetOutputToDefaultAudioDevice();

            //Speak a text string synchronously.
            synth.Speak(articleText);
        }
    }
}
