using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.TextToSpeech.V1;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using NAudio.Wave;
using Newtonsoft.Json;

namespace Robot
{

    public class Speaker
    {
        public static IDictionary<string, string> cache = new Dictionary<string, string>();
        public Speaker()
        {
        }

        public static void say(string message)
        {
            ArduinoCommand.eventG.FireSpeakingStartEvent(message);
            if (ArduinoCommand.robot.Option.debug)
            {
                Console.WriteLine("Parole : {0}", message);
            }
            if (cache.TryGetValue(message, out string idCache))
            {
                PlayAudio(new StringBuilder("cache/vocal/").Append(idCache.ToString()).Append(".wav").ToString());
                return;
            }

            switch (ArduinoCommand.robot.Option.voiceService)
            {
                case "Microsoft":
                    SayWithMicrosoftRecognition(message);
                    break;
                case "Google":
                    SayWithGoogleTextToSpeech(message);
                    break;
                default:
                    break;
            }

         
        }

        public static async void SayWithMicrosoftRecognition(string message)
        {
            SpeechConfig config = SpeechConfig.FromSubscription(ArduinoCommand.robot.Option.voiceSubscriptionKey, ArduinoCommand.robot.Option.voiceSubscriptionRegion);
            config.SpeechSynthesisVoiceName = ArduinoCommand.robot.Option.voice;
            config.SpeechSynthesisLanguage = ArduinoCommand.robot.Option.langue;
            Guid id = Guid.NewGuid();
            var fileName = "cache/vocal/" + id.ToString() + ".wav";
            using (var fileOutput = Microsoft.CognitiveServices.Speech.Audio.AudioConfig.FromWavFileOutput(fileName))
            {
                using (var synthesizer = new SpeechSynthesizer(config, fileOutput))
                {
                    var text = message;
                    var result = await synthesizer.SpeakTextAsync(text);

                    if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                    {
                        cache[text] = id.ToString();
                        Console.WriteLine($"Parole synthétisé sur [{fileName}] pour le texte [{text}]");
                        SaveVocalCache();
                    }
                    else if (result.Reason == ResultReason.Canceled)
                    {
                        var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                        Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                        if (cancellation.Reason == CancellationReason.Error)
                        {
                            Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                            Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                            Console.WriteLine($"CANCELED: Did you update the subscription info?");
                        }
                    }
                }
            }
            PlayAudio(fileName);
        }

        public static void SayWithGoogleTextToSpeech(string message)
        {
            var client = TextToSpeechClient.Create();

            // The input to be synthesized, can be provided as text or SSML.
            var input = new SynthesisInput
            {
                Text = message
            };

            // Build the voice request.
            var voiceSelection = new VoiceSelectionParams
            {
                LanguageCode = ArduinoCommand.robot.Option.langue,
                SsmlGender = (SsmlVoiceGender) Enum.Parse(typeof(SsmlVoiceGender), ArduinoCommand.robot.Option.genre)
            };

            // Specify the type of audio file.
            var audioConfig = new Google.Cloud.TextToSpeech.V1.AudioConfig
            {
                AudioEncoding = AudioEncoding.Linear16
            };

            // Perform the text-to-speech request.
            var response = client.SynthesizeSpeech(input, voiceSelection, audioConfig);

            // Write the response to the output file.
            Guid id = Guid.NewGuid();
            var fileName = "cache/vocal/" + id.ToString() + ".wav";
            using (var output = File.Create(fileName))
            {
                response.AudioContent.WriteTo(output);
            }
            cache[message] = id.ToString();
            SaveVocalCache();
            Console.WriteLine($"Parole synthétisé sur [{fileName}] pour le texte  [{message}]");
            PlayAudio(fileName);
        }


        public static void PlayAudio(string file)
        {
            using (var audioFile = new AudioFileReader(file))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                Thread.Sleep(500);
                outputDevice.Play();
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(1000);
                }
                ArduinoCommand.eventG.FireSpeakingStopEvent(file);
            }
        }

        public static void LoadVocalCache()
        {
            Console.WriteLine("Chargement des fichiers vocaux ..");
            string filePath = Directory.GetCurrentDirectory() + "/cache/vocal/vocal.json";
            if (File.Exists(filePath))
            {
                using StreamReader file = File.OpenText(filePath);
                JsonSerializer serializer = new JsonSerializer();
                Console.WriteLine("Dossier trouver en cours de  récuperation.");
                cache =(Dictionary<string, string>) serializer.Deserialize(file, typeof(Dictionary<string, string>));
            }
        }

        public static void SaveVocalCache()
        {
            string filePath = Directory.GetCurrentDirectory() + "/cache/vocal/vocal.json";
            using StreamWriter file = File.CreateText(filePath);
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(file, cache);
        }
    }
}