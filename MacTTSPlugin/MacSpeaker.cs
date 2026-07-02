using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;

namespace MacTTSPlugin
{
    internal sealed class MacSpeaker : IDisposable
    {
        private readonly BlockingCollection<string> queue = new BlockingCollection<string>();
        private readonly Thread worker;
        private volatile bool disposed;

        public string BinaryPath { get; set; } = @"Z:\usr\bin\say";
        public string Voice { get; set; } = string.Empty;
        public int Rate { get; set; } = 260;
        public string ExtraArguments { get; set; } = string.Empty;
        public bool UseQueue { get; set; } = true;

        public event Action<string> OnLog;
        public event Action<Exception> OnError;

        public MacSpeaker()
        {
            worker = new Thread(WorkLoop)
            {
                IsBackground = true,
                Name = "MacSpeakerWorker"
            };
            worker.Start();
        }

        public void Enqueue(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || disposed)
            {
                return;
            }

            if (UseQueue)
            {
                queue.Add(text);
                return;
            }

            SpeakOnce(text);
        }

        private void WorkLoop()
        {
            try
            {
                foreach (string text in queue.GetConsumingEnumerable())
                {
                    SpeakOnce(text);
                }
            }
            catch (ObjectDisposedException)
            {
                // Expected during shutdown.
            }
        }

        private void SpeakOnce(string text)
        {
            try
            {
                string args = BuildArguments(text);
                var psi = new ProcessStartInfo
                {
                    FileName = BinaryPath,
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false
                };

                using (var p = Process.Start(psi))
                {
                    if (p == null)
                    {
                        throw new InvalidOperationException("Process could not be started.");
                    }
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex);
            }
        }

        private string BuildArguments(string text)
        {
            string safeText = QuoteArgument(text);
            string extra = (ExtraArguments ?? string.Empty).Trim();

            if (extra.Contains("{text}"))
            {
                extra = extra.Replace("{text}", safeText);
            }
            else if (extra.Length > 0)
            {
                extra = extra + " " + safeText;
            }
            else
            {
                extra = safeText;
            }

            string voiceArg = string.IsNullOrWhiteSpace(Voice) ? string.Empty : $"-v {QuoteArgument(Voice)} ";
            string rateArg = Rate > 0 ? $"-r {Rate} " : string.Empty;

            string result = (voiceArg + rateArg + extra).Trim();
            OnLog?.Invoke($"say {result}");
            return result;
        }

        private static string QuoteArgument(string value)
        {
            string escaped = Regex.Replace(value ?? string.Empty, "(\\\\*)\"", "$1$1\\\\\"");
            escaped = Regex.Replace(escaped, "(\\\\+)$", "$1$1");
            return "\"" + escaped + "\"";
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            try
            {
                queue.CompleteAdding();
            }
            catch (ObjectDisposedException)
            {
            }

            try
            {
                if (worker.IsAlive)
                {
                    worker.Join(500);
                }
            }
            catch (ThreadStateException)
            {
            }

            queue.Dispose();
        }
    }
}
