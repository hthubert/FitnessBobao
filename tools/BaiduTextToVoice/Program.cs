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
                    buffer[i + channel] = Convert.ToInt16(amplitude * Math.Sin(t * i));
                }
            }
            return null;
        }

        static byte[] CreateSilentSound(WaveFormat format, int seconds)
        {
            return new byte[format.AverageBytesPerSecond * seconds];
        }

        private static Stream WriteSpeech(byte[] speech, int repeat, ref WaveFileWriter writer)
        {
            const int readSize = 1024 * 4;
            Stream output = null;
            using (var stream = new MemoryStream(speech))
            using (var reader = new Mp3FileReader(stream)) {
                if (writer == null) {
                    output = new MemoryStream();
                    writer = new WaveFileWriter(output, reader.WaveFormat);
                }
                var seconds = (int)Math.Ceiling(reader.TotalTime.TotalSeconds);
                var bytes = CreateSilentSound(reader.WaveFormat, seconds);
                var index = 0;
                while (reader.Read(bytes, index, readSize) == readSize) {
                    index += readSize;
                }
                for (var i = 0; i < repeat; i++) {
                    writer.Write(bytes, 0, bytes.Length);
                }
            }
            return output;
        }

        static void Main(string[] args)
        {
            var option = new Dictionary<string, object>()
            {
                {"spd", 5}, // 语速
                {"vol", 5}, // 音量
                {"per", 0}  // 发音人，4：情感度丫丫童声
            };
            var text = "滴,滴,答,10,9,8,7,6,5,4,3,2,1";
            WaveFileWriter writer = null;
            Stream output = null;
            foreach (var s in text.Split(',')) {
                var result = _ttsClient.Synthesis(s, option);
                if (!result.Success) {
                    Console.WriteLine(result.ErrorMsg);
                }
                var ret = WriteSpeech(result.Data, 1, ref writer);
                if (ret != null) {
                    output = ret;
                }
            }
            writer?.Flush();
            output?.Seek(0, SeekOrigin.Begin);
            using (var fs = new FileStream("test.wav", FileMode.Create)) {
                output?.CopyTo(fs);
                fs.Close();
            }
            writer?.Close();
            Console.ReadLine();
        }

    }
}
