using System;
using System.IO;
using System.Linq;
using System.Text;
using NAudio.Wave;

namespace MorseMp3Decoder
{
    class Program
    {
        static byte[] GetWavBytes(string filename)
        {
            var bytes = File.ReadAllBytes(filename);
            var headerLength = GetWavHeaderLength(bytes);
            return bytes.Skip(headerLength+16*4).ToArray();
        }

        static int GetWavHeaderLength(byte[] wave)
        {
            int subchunk1Size = BitConverter.ToInt32(wave, 16);
            if (subchunk1Size < 16)
            {
                Console.WriteLine("This is not a valid wav file");
                return -1;
            }
            return subchunk1Size;
        }

        static int[] GetSamples(byte[] wave)
        {
            var res = new int[wave.Length / 4];
            for (int i = 0; i < wave.Length-4; i += 4)
            {
                res[i/4] = BitConverter.ToInt32(wave, i);
            }
            return res;
        }

        static byte[] Minimize(int[] samples)
        {
            int frameLength = 16;
            var res = new byte[samples.Length / frameLength];
            long offset = 0;
            int resIndex = 0;
            while (offset <= samples.Length - frameLength)
            {
                byte isBeep = 0;
                for (int i = 0; i < frameLength; i++)
                {
                    if (samples[offset+i]  != 0)
                    {
                        // signal 
                        isBeep = 1;
                        break;
                    }
                }
                res[resIndex] = isBeep;
                resIndex++;
                offset += frameLength;
            }

            return res;
        }

        static string GetMorseString(byte[] beeps)
        {
            StringBuilder morseSb = new StringBuilder();
            const int DOT_LENGTH_MAX = 200;
            const byte DOT_LENGTH_MIN = 20;
            const int DASH_LENGTH_MIN = 300;
            const int PAUSE_LENGTH_MIN = 1000;
            const int SPACE_LENGTH_MIN = 3000;
            
            bool countingBeepLength = false;
            int beepLength = 0;
            bool countingPauseLength = false;
            int pauseLength = 0;
            for (int i = 0; i < beeps.Length; i++)
            {

                if (beeps[i] == 1)
                {
                    if (countingPauseLength)
                    {
                        if (pauseLength > SPACE_LENGTH_MIN)
                        {
                            morseSb.Append('\\');
                        }else if (pauseLength > PAUSE_LENGTH_MIN)
                        {
                            morseSb.Append(' ');
                        }
                        countingPauseLength = false;
                        pauseLength = 0;
                    }
                    beepLength++;
                    // signal start
                    countingBeepLength = true;
                }
                else
                {
                    countingPauseLength = true;
                    pauseLength++;
                    if(!countingBeepLength)
                        continue;
                    // signal ended
                    if (beepLength < DOT_LENGTH_MAX && beepLength > DOT_LENGTH_MIN)
                    {
                        morseSb.Append('.');
                    } 
                    else if (beepLength>DASH_LENGTH_MIN)
                    {
                        morseSb.Append('-');                        
                    }
                    countingBeepLength = false;
                    beepLength = 0;
                }
            }

            return morseSb.ToString();
        }

        private static string ConvertMp3ToWav(string mp3path)
        {
            var wavPath = $"{mp3path}.wav";
            using (Mp3FileReader mp3 = new Mp3FileReader(mp3path))
            {
                using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
                {
                    WaveFileWriter.CreateWaveFile(wavPath, pcm);
                }
            }

            return wavPath;
        }
        
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Drop a wav/mp3 file to this program");
                Console.ReadLine();
                return;
            }

            bool fileGenerated = false; 
            string wavPath;
            if (args[0].EndsWith(".wav"))
            {
                wavPath = args[0];
            }
            else
            {
                wavPath = ConvertMp3ToWav(args[0]);
                fileGenerated = true;
            }

            var bytes = GetWavBytes(wavPath);
            if (fileGenerated && wavPath.EndsWith(".mp3.wav"))
            {
                File.Delete(wavPath);
            }
            var samples = GetSamples(bytes);
            var beeps = Minimize(samples);
            var morseString = GetMorseString(beeps);
            var text = MorseToString.GetFullString(morseString);
            Console.WriteLine(morseString);
            Console.WriteLine(text);
            Console.ReadLine();
        }
    }
}
