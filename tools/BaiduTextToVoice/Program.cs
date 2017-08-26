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

        static void Main(string[] args)
        {
            var option = new Dictionary<string, object>()
            {
                {"spd", 5}, // 语速
                {"vol", 5}, // 音量
                {"per", 0}  // 发音人，4：情感度丫丫童声
            };
            var result = _ttsClient.Synthesis("亲爱的用户，您好，这是一个语音合成示例，感谢您对科大讯飞语音技术的支持！科大讯飞是亚太地区最大的语音上市公司，股票代码：002230", option);
            if (!result.Success) {
                Console.WriteLine(result.ErrorMsg);
                Console.ReadLine();
                return;
            }
            using (var stream = new MemoryStream(result.Data))
            using (var waveOutDevice = new WaveOut())
            using (var reader = new Mp3FileReader(stream)) {
                waveOutDevice.Init(reader);
                waveOutDevice.Play();
                waveOutDevice.PlaybackStopped += WaveOutDeviceOnPlaybackStopped;
                Console.ReadLine();
                waveOutDevice.Stop();
            }
        }

        private static void WaveOutDeviceOnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            //e.
        }
    }
}
