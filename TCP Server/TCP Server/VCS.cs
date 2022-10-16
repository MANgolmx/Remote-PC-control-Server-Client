using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using AudioSwitcher.AudioApi.CoreAudio;
using System.Net.Sockets;


namespace TCP_Server
{
    internal class VCS
    {
        static SpeechRecognitionEngine Speech_recognizer = new SpeechRecognitionEngine();
        static SpeechRecognitionEngine Activate_recognizer = new SpeechRecognitionEngine();

        static SpeechSynthesizer Synthesizer = new SpeechSynthesizer();

        static CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;

        static int attempts = 0;

        string[] name;

        public VCS(string[] name)
        {
            GrammaLoad(name);

            Speech_recognizer.LoadGrammarCompleted +=
                  new EventHandler<LoadGrammarCompletedEventArgs>(recognizer_LoadGrammarCompleted);
            Speech_recognizer.SpeechDetected +=
              new EventHandler<SpeechDetectedEventArgs>(recognizer_SpeechDetected);
            Speech_recognizer.SpeechRecognized +=
              new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);
            Speech_recognizer.RecognizeCompleted +=
                new EventHandler<RecognizeCompletedEventArgs>(recognizer_RecognizeCompleted);

            Activate_recognizer.LoadGrammarCompleted +=
              new EventHandler<LoadGrammarCompletedEventArgs>(recognizer_LoadGrammarCompleted);
            Activate_recognizer.SpeechRecognized +=
                new EventHandler<SpeechRecognizedEventArgs>(activateRecognizer_SpeechRecognized);
            Activate_recognizer.RecognizeCompleted +=
                 new EventHandler<RecognizeCompletedEventArgs>(activateRecognizer_RecognizeCompleted);
            Speech_recognizer.SpeechDetected +=
                new EventHandler<SpeechDetectedEventArgs>(activateRecognizer_SpeechDetected);

            foreach (var s in Synthesizer.GetInstalledVoices())
                Console.WriteLine(s.VoiceInfo.Description);

            //Synthesizer.SelectVoice("Microsoft Mark");
            Synthesizer.SelectVoice("Microsoft David Desktop");
            Synthesizer.SetOutputToDefaultAudioDevice();

            Speech_recognizer.SetInputToDefaultAudioDevice();
            Activate_recognizer.SetInputToDefaultAudioDevice();
            this.name = name;
        }

        public void StartListening()
        {
            Activate_recognizer.RecognizeAsync();
        }

        private static void activateRecognizer_SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            Console.WriteLine("  Activate speech detected at AudioPosition = {0}", e.AudioPosition);
        }

        private static void activateRecognizer_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            Activate_recognizer.RecognizeAsync();
        }

        private static void GrammaLoad(string[] names)
        {
            // Create a grammar.  
            Choices commands = new Choices(new string[] {
                    "Hello", "How are you", "What time is it", "Commands"});

            for (int i = 0; i <= 100; i++)
                commands.Add("Change volume to " + i);

            GrammarBuilder gbCommands = new GrammarBuilder();
            gbCommands.Append(commands);

            // Create a Grammar object and load it to the recognizer.  
            Grammar gCommands = new Grammar(gbCommands);
            gCommands.Name = ("Commands");
            Speech_recognizer.LoadGrammarAsync(gCommands);

            // Create a grammar.  
            Choices activateCommands = new Choices(names);

            GrammarBuilder gbActivateCommands = new GrammarBuilder();
            gbActivateCommands.Append(activateCommands);

            // Create a Grammar object and load it to the recognizer.  
            Grammar gActivateCommands = new Grammar(gbActivateCommands);
            gActivateCommands.Name = ("ActivateCommands");
            Activate_recognizer.LoadGrammarAsync(gActivateCommands);
        }

        private static void activateRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string speech = e.Result.Text;

            Console.WriteLine("  Activate speech recognized: " + speech);

            if (speech == "Shut up")
            {
                Synthesizer.SpeakAsyncCancelAll();
            }
            else if (speech == "Mark" || speech == "Markus")
            {
                if (attempts <= 0)
                    Speech_recognizer.RecognizeAsync();
                attempts = 10;
            }
        }

        private static void recognizer_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            if (attempts > 0)
            {
                Speech_recognizer.RecognizeAsync();
                attempts--;
            }
        }

        // Handle the SpeechDetected event.  
        static void recognizer_SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            Console.WriteLine("  Speech detected at AudioPosition = {0}", e.AudioPosition);
        }

        // Handle the LoadGrammarCompleted event.  
        static void recognizer_LoadGrammarCompleted(object sender, LoadGrammarCompletedEventArgs e)
        {
            Console.WriteLine("Grammar loaded: " + e.Grammar.Name);
        }

        // Handle the SpeechRecognized event.  
        static void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string speech = e.Result.Text;

            Console.WriteLine("  Speech recognized: " + speech);

            if (e.Result.Confidence > 0.65)
            {
                if (speech == "Hello")
                {
                    attempts = 0;
                    Synthesizer.SpeakAsync("Hello, I am Mark 1, voice control system");
                }
                else if (speech == "How are you")
                {
                    attempts = 0;
                    Synthesizer.SpeakAsync("All systems online");
                }
                else if (speech == "What time is it")
                {
                    attempts = 0;
                    Synthesizer.SpeakAsync("It is: " + DateTime.Now.ToString("h mm tt"));
                    if (DateTime.Now.Hour > 23 || DateTime.Now.Hour < 6)
                        Synthesizer.SpeakAsync("You need to rest");
                }
                else if (e.Result.Words.Count == 4)
                {
                    attempts = 0;
                    defaultPlaybackDevice.Volume = Int32.Parse(e.Result.Words[3].Text);
                    Synthesizer.SpeakAsync("Volume changed");
                }
            }
        }

    }
}
