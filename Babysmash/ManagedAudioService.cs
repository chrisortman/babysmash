using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BabySmash {
    public class ManagedAudioService {
        private readonly ConcurrentDictionary<string, string> _cachedWavs = new ConcurrentDictionary<string, string>();
        private readonly TempFileCollection _tempFiles = new TempFileCollection();
        private readonly ConcurrentQueue<IAudioRequest> _soundQueue;
        private Task _audioPlayerTask;
        private MediaElement _mediaElement;
        private const UInt32 SND_ASYNC = 0;
        private const UInt32 SND_NOSTOP = 1;

        public ManagedAudioService() {
            _cachedWavs = new ConcurrentDictionary<string, string>();   
            _tempFiles = new TempFileCollection();
            _soundQueue = new ConcurrentQueue<IAudioRequest>();
            _mediaElement = new MediaElement();
            _audioPlayerTask = new Task(() => {
                while(true) {
                    IAudioRequest request = null;
                    if(_soundQueue.TryDequeue(out request)) {
                        var wrs = request as WavResourceSound;
                        if (wrs != null) {
                            _mediaElement.Source = new Uri("file:///" + wrs.AudioFilePath);
                            
                        }
                    }
                }
            });
        }

        public async void PlayWavResource(string wav) {
            string resourceName = await GetWavResource(wav);
            QueueSound(new WavResourceSound(resourceName,SND_ASYNC));
        }

        public async void PlayWavResourceYield(string wav) {
            string resourceName = await GetWavResource(wav);
            QueueSound(new WavResourceSound(resourceName, SND_ASYNC | SND_NOSTOP));
        }

        private void QueueSound(WavResourceSound wavResourceSound) {
            _soundQueue.Enqueue(wavResourceSound);
        }

        private async Task<string> GetWavResource(string wav) {
            string retVal = null;
            if (_cachedWavs.TryGetValue(wav, out retVal)) {
                return retVal;
            } else {
                // get the namespace 
                string strNameSpace = Assembly.GetExecutingAssembly().GetName().Name;

                // get the resource into a stream
                using (Stream str = Assembly.GetExecutingAssembly().GetManifestResourceStream(strNameSpace + wav)) {
                    string tempfile = System.IO.Path.GetTempFileName();
                    _tempFiles.AddFile(tempfile, false);

                    if (str == null) {
                        throw new InvalidOperationException("Could not load wav resource " + wav);
                    }

                    var bStr = new Byte[str.Length];
                    await str.ReadAsync(bStr, 0, (int) str.Length);

                    using (FileStream fs = File.Create(tempfile)) {
                        await fs.WriteAsync(bStr, 0, bStr.Length);
                    }

                    _cachedWavs.TryAdd(wav, tempfile);
                    return tempfile;
                }
            }
        }
    }

    public interface IAudioRequest {
        void Play();
    }
    public class WavResourceSound : IAudioRequest {
        private string _wavFile;
        public WavResourceSound(string wavFile, uint u) {
            _wavFile = wavFile;
        }

        public string AudioFilePath {
            get { return _wavFile; }
        }

        public void Play() {
        }
    }
}