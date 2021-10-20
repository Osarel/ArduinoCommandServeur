using Google.Cloud.TextToSpeech.V1;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Logging;
using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Robot
{

    public class Speaker
    {
        public static IDictionary<string, string> cache = new Dictionary<string, string>();
        public static ILogger log = ArduinoCommand.loggerProvider.CreateLogger("Voix");
        public Speaker()
        {
        }

        public static void Say(string message)
        {
            ArduinoCommand.eventG.FireSpeakingStartEvent(message);
            log.LogInformation("Parole : {0}", message.Replace("\n", ""));
            if (cache.TryGetValue(message, out string idCache))
            {
                PlayAudio(new StringBuilder("cache/vocal/").Append(idCache.ToString()).Append(".wav").ToString());
                return;
            }

            switch (ArduinoCommand.robot.Options.voiceService)
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
            SpeechConfig config = SpeechConfig.FromSubscription(ArduinoCommand.robot.Options.voiceSubscriptionKey, ArduinoCommand.robot.Options.voiceSubscriptionRegion);
            config.SpeechSynthesisVoiceName = ArduinoCommand.robot.Options.voice;
            config.SpeechSynthesisLanguage = ArduinoCommand.robot.Options.langue;
            Guid id = Guid.NewGuid();
            var fileName = "cache/vocal/" + id.ToString() + ".wav";
            using (var fileOutput = Microsoft.CognitiveServices.Speech.Audio.AudioConfig.FromWavFileOutput(fileName))
            {
                using var synthesizer = new SpeechSynthesizer(config, fileOutput);
                var text = message;
                var result = await synthesizer.SpeakTextAsync(text);

                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    cache[text] = id.ToString();
                    log.LogDebug($"Parole synthétisé sur [{fileName}] pour le texte [{text}]");
                    SaveVocalCache();
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                    log.LogError($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        log.LogError($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        log.LogError($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                        log.LogError($"CANCELED: Did you update the subscription info?");
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
                LanguageCode = ArduinoCommand.robot.Options.langue,
                SsmlGender = (SsmlVoiceGender)Enum.Parse(typeof(SsmlVoiceGender), ArduinoCommand.robot.Options.genre)
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
            log.LogDebug($"Parole synthétisé sur [{fileName}] pour le texte  [{message}]");
            PlayAudio(fileName);
        }


        public static void PlayAudio(string file)
        {
            using var audioFile = new AudioFileReader(file);
            using var outputDevice = new WaveOutEvent();
            outputDevice.Init(audioFile);
            Thread.Sleep(500);
            outputDevice.Play();
            while (outputDevice.PlaybackState == PlaybackState.Playing)
            {
                Thread.Sleep(1000);
            }
            ArduinoCommand.eventG.FireSpeakingStopEvent(file);
        }

        public static void LoadVocalCache()
        {
            log.LogInformation("Chargement des fichiers vocaux ..");
            string filePath = Directory.GetCurrentDirectory() + "/cache/vocal/vocal.json";
            if (File.Exists(filePath))
            {
                using StreamReader file = File.OpenText(filePath);
                JsonSerializer serializer = new JsonSerializer();
                log.LogInformation("Dossier trouver en cours de  récuperation.");
                cache = (Dictionary<string, string>)serializer.Deserialize(file, typeof(Dictionary<string, string>));
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