using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Baidu.Aip.Speech;
using NAudio.Wave;
using NLog;

namespace Htggbb.FitnessCourse
{
    internal class SpeechCreator
    {
        private const string UsuallyItems = "go,10,9,8,7,6,5,4,3,2,1";
        private readonly Tts _tts;
        private readonly Dictionary<string, object> _ttsOptions;
        private readonly List<byte[]> _numbers = new List<byte[]>();
        private readonly List<byte[]> _prefixs = new List<byte[]>();
        private byte[] _tick;
        private readonly ILogger _logger;
        private WaveFormat _waveFormat;

        private static byte[] CreateSilentSound(WaveFormat format, int seconds)
        {
            return new byte[format.AverageBytesPerSecond * seconds];
        }

        private byte[] GetPcmData(byte[] speech)
        {
            const int readSize = 1024 * 4;
            using (var stream = new MemoryStream(speech))
            using (var reader = new Mp3FileReader(stream)) {
                var format = reader.WaveFormat;
                if (_waveFormat == null) {
                    _waveFormat = new WaveFormat(format.SampleRate, format.BitsPerSample, format.Channels);
                }
                _waveFormat = reader.WaveFormat;
                var seconds = (int)Math.Ceiling(reader.TotalTime.TotalSeconds);
                var bytes = CreateSilentSound(format, seconds);
                var index = 0;
                while (reader.Read(bytes, index, readSize) == readSize) {
                    index += readSize;
                }
                return bytes;
            }
        }

        private List<byte[]> GetSpeech(string speech)
        {
            var list = new List<byte[]>();
            foreach (var s in speech.Split(',')) {
                _logger.Debug($"baidu synthesis {s}.");
                var result = _tts.Synthesis(s, _ttsOptions);
                if (result.Success) {
                    list.Add(GetPcmData(result.Data));
                }
                else {
                    list.Add(null);
                    _logger.Error(result.ErrorMsg);
                }
                Thread.Sleep(1000);
            }
            return list;
        }

        private void LoadTick()
        {
            _logger.Debug("load tick");
            const int bufferSize = 1024 * 32;
            using (var reader = new WaveFileReader("tick.wav")) {
                _tick = CreateSilentSound(reader.WaveFormat, 1);
                var index = 0;
                while (reader.Read(_tick, index, bufferSize) == bufferSize) {
                    index += bufferSize;
                }
            }
        }

        public SpeechCreator()
        {
            _logger = LogManager.GetLogger(nameof(SpeechCreator));
            _tts = new Tts("4zGAhiQANs9tjMv4p1BCGmdh", "fbb284bd4c52f984bf63c78f0d661600");
            _ttsOptions = new Dictionary<string, object>
            {
                {"spd", 5}, // 语速
                {"vol", 5}, // 音量
                {"per", 0}  // 发音人，4：情感度丫丫童声
            };
        }

        public Task Init()
        {
            return Task.Run(() => {
                LoadTick();
                _prefixs.Clear();
                _numbers.Clear();
                var list = GetSpeech(UsuallyItems);
                for (var i = 0; i < list.Count; i++) {
                    if (i < 1) {
                        _prefixs.Add(list[i]);
                    }
                    else {
                        _numbers.Add(list[i]);
                    }
                }
            });
        }

        private void ProcessLine(string line, WaveFileWriter writer)
        {
            _logger.Debug($"Text To Speech: {line}");
            var items = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (items.Length < 1) {
                return;
            }
            var span = int.Parse(items[1]);
            var speech = GetSpeech($"{items[0]}{span}秒");
            writer.Write(speech[0], 0, speech[0].Length);
            foreach (var item in _prefixs) {
                writer.Write(item, 0, item.Length);
            }
            if (span <= 20) {
                WriteTick(writer, span - 5);
                for (var i = 0; i < 5; i++) {
                    var number = _numbers[i];
                    writer.Write(number, 0, number.Length);
                }
            }
            else {
                WriteTick(writer, span - _numbers.Count);
                foreach (var number in _numbers) {
                    writer.Write(number, 0, number.Length);
                }
            }
        }

        private void WriteTick(Stream writer, int count)
        {
            for (var i = 0; i < count; i++) {
                writer.Write(_tick, 0, _tick.Length);
            }
        }

        public Task Process(string path)
        {
            return Task.Run(() => {
                if (!File.Exists(path)) {
                    _logger.Error($"File not found: {path}.");
                    return;
                }
                if (_waveFormat == null) {
                    _logger.Error("Need init.");
                    return;
                }

                var lines = File.ReadAllLines(path);
                var wavFile = Path.ChangeExtension(path, "wav");
                using (var writer = new WaveFileWriter(wavFile, _waveFormat)) {
                    foreach (var line in lines) {
                        ProcessLine(line, writer);
                    }
                }
            });
        }
    }
}
