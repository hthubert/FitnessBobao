using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Baidu.Aip.Speech;
using NAudio.Wave;

namespace BaiduTextToVoice
{
    class Program
    {
        private static readonly Tts _ttsClient;

        static Program()
        {
            _ttsClient = new Tts("4zGAhiQANs9tjMv4p1BCGmdh", "fbb284bd4c52f984bf63c78f0d661600");
        }

        static byte[] CreateOneSecondSilentSound(WaveFormat format)
        {
            const int amplitude = 0;
            var numSamples = (uint)(format.SampleRate * format.Channels);
            var buffer = new short[numSamples];
            var t = (Math.PI * 2 * (format.SampleRate / 100.0)) / (format.AverageBytesPerSecond * format.Channels);            
            for (var i = 0; i < buffer.Length; i++) {
                for (var channel = 0; channel < format.Channels; channel++) {
                    buffer[i+channel] = Convert.ToInt16(amplitude * Math.Sin(t * i));
                }
            }
            return null;
        }

        static byte[] CreateSilentSound(WaveFormat format, int seconds)
        {
            return new byte[format.AverageBytesPerSecond * seconds];
        }

        static void Main(string[] args)
        {
            var option = new Dictionary<string, object>()
            {
                {"spd", 5}, // 语速
                {"vol", 5}, // 音量
                {"per", 0}  // 发音人，4：情感度丫丫童声
            };
            var result = _ttsClient.Synthesis("原地高抬腿", option);
            if (!result.Success) {
                Console.WriteLine(result.ErrorMsg);
                Console.ReadLine();
                return;
            }
            using (var stream = new MemoryStream(result.Data))
            using (var reader = new Mp3FileReader(stream)) {
                //reader.WaveFormat
                //CreateOneSecondSilentSound(reader.WaveFormat);
                var seconds = Math.Ceiling(reader.TotalTime.TotalSeconds);
                //var 
                //reader.Read()
            }

            Console.ReadLine();
        }

    }
}
