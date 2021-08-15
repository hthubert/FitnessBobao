using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Baidu.Aip.Speech;
using NAudio.Lame;
using NAudio.Wave;
using NLog;

namespace Htggbb.FitnessCourse
{
    internal class SpeechCreator
    {
        private const string NumberItems = "10,9,8,7,6,5,4,3,2,1";
        private readonly Tts _tts;
        private readonly Dictionary<string, object> _ttsOptions;
        private readonly List<byte[]> _numbers = new List<byte[]>();
        private byte[] _go;
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
            using (var reader = new Mp3FileReader(stream))
            {
                var format = reader.WaveFormat;
                if (_waveFormat == null)
                {
                    _waveFormat = new WaveFormat(format.SampleRate, format.BitsPerSample, format.Channels);
                }
                _waveFormat = reader.WaveFormat;
                var seconds = (int)Math.Ceiling(reader.TotalTime.TotalSeconds);
                var bytes = CreateSilentSound(format, seconds);
                var index = 0;
                while (reader.Read(bytes, index, readSize) == readSize)
                {
                    index += readSize;
                }
                return bytes;
            }
        }

        private byte[] GetSpeech(string speech)
        {
            _logger.Debug($"baidu synthesis {speech}.");
            var result = _tts.Synthesis(speech, _ttsOptions);
            if (result.Success)
            {
                return GetPcmData(result.Data);
            }
            _logger.Error(result.ErrorMsg);
            return null;
        }

        private void LoadTick()
        {
            _logger.Debug("load tick");
            const int bufferSize = 1024 * 32;
            using (var reader = new WaveFileReader("tick.wav"))
            {
                if (_waveFormat == null)
                {
                    var format = reader.WaveFormat;
                    _waveFormat = new WaveFormat(format.SampleRate, format.BitsPerSample, format.Channels);
                }
                _tick = CreateSilentSound(reader.WaveFormat, 1);
                var index = 0;
                while (reader.Read(_tick, index, bufferSize) == bufferSize)
                {
                    index += bufferSize;
                }
            }
        }

        private void LoadGo()
        {
            _logger.Debug("load go");

        }

        private void LoadNumbers()
        {
            _logger.Debug("load numbers");
            _numbers.Clear();
            var numbers = NumberItems.Split(',');
            foreach (var num in numbers)
            {
                var file = $"{num}.data";
                if (File.Exists(file))
                {
                    _numbers.Add(File.ReadAllBytes(file));
                }
                else
                {
                    var data = GetSpeech(num);
                    if (data != null)
                    {
                        File.WriteAllBytes(file, data);
                    }
                    _numbers.Add(data);
                }
            }
        }

        private int ProcessLine(string text, int second, Stream writer)
        {
            var speech = GetSpeech(text);
            writer.Write(speech, 0, speech.Length);
            if (_go != null)
            {
                writer.Write(_go, 0, _go.Length);
            }

            if (second <= 30 && second > 0)
            {
                WriteTick(writer, second - 5);
                for (var i = 0; i < 5; i++)
                {
                    var number = _numbers[i + 5];
                    writer.Write(number, 0, number.Length);
                }
            }
            else if (second > 30)
            {
                WriteTick(writer, second - _numbers.Count);
                foreach (var number in _numbers)
                {
                    writer.Write(number, 0, number.Length);
                }
            }
            return speech.Length / _waveFormat.AverageBytesPerSecond + second;
        }

        private void WriteTick(Stream writer, int count)
        {
            for (var i = 0; i < count; i++)
            {
                writer.Write(_tick, 0, _tick.Length);
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
            return Task.Run(() =>
            {
                LoadTick();
                LoadGo();
                LoadNumbers();
            });
        }

        public Task Process(string[] lines, string output)
        {
            void TaskRun()
            {
                if (_waveFormat == null)
                {
                    _logger.Error("Need init.");
                    return;
                }

                using (var stream = new MemoryStream())
                using (var writer = new WaveFileWriter(stream, _waveFormat))
                {
                    var lrc = new StringBuilder();
                    var span = TimeSpan.Zero;
                    foreach (var line in lines)
                    {
                        var items = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (items.Length < 1)
                        {
                            _logger.Warn($"无效的行： {line}");
                            continue;
                        }

                        var second = 0;
                        string speech;
                        if (items.Length > 1)
                        {
                            second = int.Parse(items[1]);
                            speech = $"{items[0]},{second}秒,Go";
                        }
                        else
                        {
                            speech = items[0];
                        }
                        lrc.AppendLine($"[{span:mm\\:ss}.00]{speech}");
                        second = ProcessLine(speech, second, writer);
                        span += TimeSpan.FromSeconds(second);
                    }
                    File.WriteAllText(Path.ChangeExtension(output, ".lrc"), lrc.ToString());
                    writer.Flush();
                    if (Path.GetExtension(output).ToLower() == ".mp3")
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        using (var mp3 = new LameMP3FileWriter(output, _waveFormat, LAMEPreset.ABR_128))
                        {
                            stream.CopyTo(mp3);
                        }
                    }
                    else
                    {
                        File.WriteAllBytes(output, stream.ToArray());
                    }
                    _logger.Debug("生成完成.");
                }
            }

            return Task.Run(TaskRun);
        }

        public Task Process(string path)
        {
            void TaskRun()
            {
                if (!File.Exists(path))
                {
                    _logger.Error($"File not found: {path}.");
                    return;
                }
                if (_waveFormat == null)
                {
                    _logger.Error("Need init.");
                    return;
                }
                var lines = File.ReadAllLines(path);
                var wavFile = Path.ChangeExtension(path, "wav");
                Process(lines, wavFile).Wait();
            }

            return Task.Run(TaskRun);
        }
    }
}
