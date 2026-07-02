using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Text.RegularExpressions;
using Advanced_Combat_Tracker;

namespace MacTTSPlugin
{
    public sealed class MacTTSPlugin : UserControl, IActPluginV1
    {
        private static readonly string[] FallbackVoices =
        {
            "Albert", "Alice", "Soumya", "Alva", "Aman", "Amélie", "Amira", "Anna", "Aru",
            "Bad News", "Bahh", "Bells", "Boing", "Bubbles", "Carmit", "Cellos", "Damayanti",
            "Daniel", "Daria", "Wobble", "Eddy (Deutsch (Deutschland))", "Eddy (Englisch (UK))",
            "Eddy (Englisch (USA))", "Eddy (Spanisch (Spanien))", "Eddy (Spanisch (Mexiko))",
            "Eddy (Finnisch (Finnland))", "Eddy (Französisch (Kanada))", "Eddy (Französisch (Frankreich))",
            "Eddy (Italienisch (Italien))", "Eddy (Japanisch (Japan))", "Eddy (Koreanisch (Südkorea))",
            "Eddy (Portugiesisch (Brasilien))", "Eddy (Chinesisch (China, Festland))", "Eddy (Chinesisch (Taiwan))",
            "Ellen", "Flo (Deutsch (Deutschland))", "Flo (Englisch (UK))", "Flo (Englisch (USA))",
            "Flo (Spanisch (Spanien))", "Flo (Spanisch (Mexiko))", "Flo (Finnisch (Finnland))",
            "Flo (Französisch (Kanada))", "Flo (Französisch (Frankreich))", "Flo (Italienisch (Italien))",
            "Flo (Japanisch (Japan))", "Flo (Koreanisch (Südkorea))", "Flo (Portugiesisch (Brasilien))",
            "Flo (Chinesisch (China, Festland))", "Flo (Chinesisch (Taiwan))", "Fred", "Geeta",
            "Good News", "Grandma (Deutsch (Deutschland))", "Grandma (Englisch (UK))",
            "Grandma (Englisch (USA))", "Grandma (Spanisch (Spanien))", "Grandma (Spanisch (Mexiko))",
            "Grandma (Finnisch (Finnland))", "Grandma (Französisch (Kanada))", "Grandma (Französisch (Frankreich))",
            "Grandma (Italienisch (Italien))", "Grandma (Japanisch (Japan))", "Grandma (Koreanisch (Südkorea))",
            "Grandma (Portugiesisch (Brasilien))", "Grandma (Chinesisch (China, Festland))",
            "Grandma (Chinesisch (Taiwan))", "Grandpa (Deutsch (Deutschland))", "Grandpa (Englisch (UK))",
            "Grandpa (Englisch (USA))", "Grandpa (Spanisch (Spanien))", "Grandpa (Spanisch (Mexiko))",
            "Grandpa (Finnisch (Finnland))", "Grandpa (Französisch (Kanada))", "Grandpa (Französisch (Frankreich))",
            "Grandpa (Italienisch (Italien))", "Grandpa (Japanisch (Japan))", "Grandpa (Koreanisch (Südkorea))",
            "Grandpa (Portugiesisch (Brasilien))", "Grandpa (Chinesisch (China, Festland))",
            "Grandpa (Chinesisch (Taiwan))", "Jester", "Ioana", "Jacques", "Joana", "Junior", "Kanya",
            "Karen", "Kathy", "Kyoko", "Lana", "Laura", "Lekha", "Lesya", "Linh", "Luciana",
            "Tünde", "Meijia", "Melina", "Milena", "Moira", "Mónica", "Montse", "Nora", "Ona",
            "Organ", "Paulina", "Piya", "Superstar", "Ralph", "Reed (Deutsch (Deutschland))",
            "Reed (Englisch (UK))", "Reed (Englisch (USA))", "Reed (Spanisch (Spanien))",
            "Reed (Spanisch (Mexiko))", "Reed (Finnisch (Finnland))", "Reed (Französisch (Kanada))",
            "Reed (Italienisch (Italien))", "Reed (Japanisch (Japan))", "Reed (Koreanisch (Südkorea))",
            "Reed (Portugiesisch (Brasilien))", "Reed (Chinesisch (China, Festland))",
            "Reed (Chinesisch (Taiwan))", "Rishi", "Rocko (Deutsch (Deutschland))",
            "Rocko (Englisch (UK))", "Rocko (Englisch (USA))", "Rocko (Spanisch (Spanien))",
            "Rocko (Spanisch (Mexiko))", "Rocko (Finnisch (Finnland))", "Rocko (Französisch (Kanada))",
            "Rocko (Französisch (Frankreich))", "Rocko (Italienisch (Italien))", "Rocko (Japanisch (Japan))",
            "Rocko (Koreanisch (Südkorea))", "Rocko (Portugiesisch (Brasilien))",
            "Rocko (Chinesisch (China, Festland))", "Rocko (Chinesisch (Taiwan))", "Samantha",
            "Sandy (Deutsch (Deutschland))", "Sandy (Englisch (UK))", "Sandy (Englisch (USA))",
            "Sandy (Spanisch (Spanien))", "Sandy (Spanisch (Mexiko))", "Sandy (Finnisch (Finnland))",
            "Sandy (Französisch (Kanada))", "Sandy (Französisch (Frankreich))",
            "Sandy (Italienisch (Italien))", "Sandy (Japanisch (Japan))",
            "Sandy (Koreanisch (Südkorea))", "Sandy (Portugiesisch (Brasilien))",
            "Sandy (Chinesisch (China, Festland))", "Sandy (Chinesisch (Taiwan))", "Sara", "Satu",
            "Shelley (Deutsch (Deutschland))", "Shelley (Englisch (UK))", "Shelley (Englisch (USA))",
            "Shelley (Spanisch (Spanien))", "Shelley (Spanisch (Mexiko))", "Shelley (Finnisch (Finnland))",
            "Shelley (Französisch (Kanada))", "Shelley (Französisch (Frankreich))",
            "Shelley (Italienisch (Italien))", "Shelley (Japanisch (Japan))",
            "Shelley (Koreanisch (Südkorea))", "Shelley (Portugiesisch (Brasilien))",
            "Shelley (Chinesisch (China, Festland))", "Shelley (Chinesisch (Taiwan))", "Sinji",
            "Tara (Englisch (Indien))", "Tessa", "Thomas", "Tina", "Tingting", "Trinoids", "Vani",
            "Whisper", "Xander", "Yelda", "Yuna", "Zarvox", "Zosia", "Zuzana"
        };

        private Label pluginStatusLabel;
        private FormActMain.PlayTtsDelegate oldTtsDelegate;
        private bool isShuttingDown;
        private readonly string settingsFile = Path.Combine(
            ActGlobals.oFormActMain.AppDataFolder.FullName,
            "Config\\MacTTSPlugin.config.xml");

        private SettingsSerializer xmlSettings;
        private readonly MacSpeaker speaker = new MacSpeaker();
        private readonly Action<string> speakerLogHandler;
        private readonly Action<Exception> speakerErrorHandler;

        private TextBox txtBinaryPath;
        private ComboBox cmbVoice;
        private NumericUpDown numRate;
        private TextBox txtExtraArgs;
        private CheckBox chkUseQueue;
        private CheckBox chkFallback;
        private Button btnTest;
        private Button btnCopyLog;
        private Button btnRefreshVoices;
        private ListBox lstLog;

        public MacTTSPlugin()
        {
            BuildUi();
            speakerLogHandler = msg => SafeLog(msg);
            speakerErrorHandler = ex => SafeLog("ERROR: " + ex.Message);
            speaker.OnLog += speakerLogHandler;
            speaker.OnError += speakerErrorHandler;
        }

        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            pluginStatusLabel = pluginStatusText;
            pluginScreenSpace.Controls.Add(this);
            Dock = DockStyle.Fill;

            xmlSettings = new SettingsSerializer(this);
            LoadSettings();
            LoadAvailableVoices();
            ApplyUiToSpeaker();

            oldTtsDelegate = ActGlobals.oFormActMain.PlayTtsMethod;
            ActGlobals.oFormActMain.PlayTtsMethod = new FormActMain.PlayTtsDelegate(PlayTts);

            pluginStatusLabel.Text = "MacTTSPlugin started";
            SafeLog("Plugin initialized");
        }

        public void DeInitPlugin()
        {
            if (isShuttingDown)
            {
                return;
            }

            isShuttingDown = true;

            try
            {
                ActGlobals.oFormActMain.PlayTtsMethod = oldTtsDelegate;
            }
            catch
            {
                // Ignore unload-time restore failures.
            }

            try
            {
                speaker.OnLog -= speakerLogHandler;
                speaker.OnError -= speakerErrorHandler;
                speaker.Dispose();
            }
            catch
            {
                // Ignore background shutdown issues.
            }

            try
            {
                SaveSettings();
            }
            catch
            {
                // Ignore unload-time persistence issues.
            }

            try
            {
                pluginStatusLabel.Text = "MacTTSPlugin stopped";
            }
            catch
            {
                // Ignore if ACT is already tearing down the UI.
            }
        }

        private void PlayTts(string text)
        {
            try
            {
                speaker.Enqueue(text);
            }
            catch (Exception ex)
            {
                SafeLog("ERROR: " + ex.Message);
                if (chkFallback.Checked)
                {
                    oldTtsDelegate?.Invoke(text);
                }
            }
        }

        private void ApplyUiToSpeaker()
        {
            speaker.BinaryPath = txtBinaryPath.Text.Trim();
            speaker.Voice = cmbVoice.Text.Trim() == "System Default" ? string.Empty : cmbVoice.Text.Trim();
            speaker.Rate = (int)numRate.Value;
            speaker.ExtraArguments = txtExtraArgs.Text;
            speaker.UseQueue = chkUseQueue.Checked;
        }

        private void BuildUi()
        {
            AutoScroll = true;

            var toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 42,
                Padding = new Padding(8, 5, 8, 5),
                BackColor = SystemColors.Control
            };

            var toolbarFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = false,
                BackColor = Color.Transparent,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 8,
                Padding = new Padding(8, 48, 8, 8),
                AutoSize = true
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            txtBinaryPath = new TextBox { Name = "txtBinaryPath", Text = @"Z:\usr\bin\say", Dock = DockStyle.Fill };
            cmbVoice = new ComboBox { Name = "cmbVoice", DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
            numRate = new NumericUpDown { Name = "numRate", Minimum = 80, Maximum = 500, Value = 260, Dock = DockStyle.Left, Width = 120 };
            txtExtraArgs = new TextBox { Name = "txtExtraArgs", Text = "{text}", Dock = DockStyle.Fill };
            chkUseQueue = new CheckBox { Name = "chkUseQueue", Checked = true, Text = "Use queue", Dock = DockStyle.Left };
            chkFallback = new CheckBox { Name = "chkFallback", Checked = false, Text = "Fallback to ACT default on error", Dock = DockStyle.Left };
            btnTest = new Button { Name = "btnTest", Text = "Test speech", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, FlatStyle = FlatStyle.System };
            btnCopyLog = new Button { Name = "btnCopyLog", Text = "Copy log", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, FlatStyle = FlatStyle.System };
            btnRefreshVoices = new Button { Name = "btnRefreshVoices", Text = "Refresh voices", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, FlatStyle = FlatStyle.System };
            lstLog = new ListBox { Name = "lstLog", Dock = DockStyle.Fill, Height = 220 };

            btnTest.Click += (_, __) => PlayTts("Mac TTS test from plugin");
            btnCopyLog.Click += (_, __) => CopyLogToClipboard();
            btnRefreshVoices.Click += (_, __) => ReloadVoices();

            txtBinaryPath.Leave += (_, __) => ApplyUiToSpeaker();
            cmbVoice.SelectedIndexChanged += (_, __) => ApplyUiToSpeaker();
            numRate.ValueChanged += (_, __) => ApplyUiToSpeaker();
            txtExtraArgs.Leave += (_, __) => ApplyUiToSpeaker();
            chkUseQueue.CheckedChanged += (_, __) => ApplyUiToSpeaker();
            chkFallback.CheckedChanged += (_, __) => ApplyUiToSpeaker();

            var actionsLabel = new Label
            {
                Text = "Actions",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 5, 12, 0)
            };

            toolbarFlow.Controls.Add(actionsLabel);
            toolbarFlow.Controls.Add(btnTest);
            toolbarFlow.Controls.Add(btnCopyLog);
            toolbarFlow.Controls.Add(btnRefreshVoices);
            toolbar.Controls.Add(toolbarFlow);

            AddRow(layout, 0, "Binary path", txtBinaryPath);
            AddRow(layout, 1, "Voice", cmbVoice);
            AddRow(layout, 2, "Rate", numRate);
            AddRow(layout, 3, "Extra args", txtExtraArgs);
            AddRow(layout, 4, "Queue", chkUseQueue);
            AddRow(layout, 5, "Fallback", chkFallback);

            var logLabel = new Label { Text = "Log", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.Controls.Add(logLabel, 0, 6);
            layout.Controls.Add(lstLog, 1, 6);

            Controls.Clear();
            Controls.Add(layout);
            Controls.Add(toolbar);
            layout.SendToBack();
            toolbar.BringToFront();
        }

        private static void AddRow(TableLayoutPanel layout, int row, string labelText, Control control)
        {
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            var label = new Label
            {
                Text = labelText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            layout.Controls.Add(label, 0, row);
            layout.Controls.Add(control, 1, row);
        }

        private void SafeLog(string message)
        {
            if (IsDisposed || isShuttingDown)
            {
                return;
            }

            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new Action<string>(SafeLog), message);
                    return;
                }

                lstLog.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
                lstLog.TopIndex = lstLog.Items.Count - 1;
            }
            catch (ObjectDisposedException)
            {
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void LoadSettings()
        {
            xmlSettings.AddControlSetting(txtBinaryPath.Name, txtBinaryPath);
            xmlSettings.AddControlSetting(cmbVoice.Name, cmbVoice);
            xmlSettings.AddControlSetting(numRate.Name, numRate);
            xmlSettings.AddControlSetting(txtExtraArgs.Name, txtExtraArgs);
            xmlSettings.AddControlSetting(chkUseQueue.Name, chkUseQueue);
            xmlSettings.AddControlSetting(chkFallback.Name, chkFallback);

            if (!File.Exists(settingsFile))
            {
                return;
            }

            using (var fs = new FileStream(settingsFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var xReader = new XmlTextReader(fs))
            {
                try
                {
                    while (xReader.Read())
                    {
                        if (xReader.NodeType == XmlNodeType.Element && xReader.LocalName == "SettingsSerializer")
                        {
                            xmlSettings.ImportFromXml(xReader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    SafeLog("Error loading settings: " + ex.Message);
                }
            }
        }

        private void LoadAvailableVoices()
        {
            if (cmbVoice == null)
            {
                return;
            }

            cmbVoice.BeginUpdate();
            cmbVoice.Items.Clear();
            cmbVoice.Items.Add("System Default");

            int voiceCount = 0;
            foreach (var voice in QueryInstalledVoices())
            {
                cmbVoice.Items.Add(voice);
                voiceCount++;
            }

            if (voiceCount == 0)
            {
                foreach (var voice in FallbackVoices)
                {
                    cmbVoice.Items.Add(voice);
                    voiceCount++;
                }

                SafeLog("Voice discovery returned 0 voices; loaded built-in fallback list.");
            }

            cmbVoice.SelectedIndex = 0;
            cmbVoice.EndUpdate();

            SafeLog($"Loaded {voiceCount} installed voices.");
        }

        private void ReloadVoices()
        {
            LoadAvailableVoices();
            ApplyUiToSpeaker();
        }

        private void CopyLogToClipboard()
        {
            try
            {
                var lines = new List<string>();
                foreach (var item in lstLog.Items)
                {
                    lines.Add(item.ToString());
                }

                Clipboard.SetText(string.Join(Environment.NewLine, lines));
                SafeLog("Log copied to clipboard.");
            }
            catch (Exception ex)
            {
                SafeLog("ERROR copying log: " + ex.Message);
            }
        }

        private IEnumerable<string> QueryInstalledVoices()
        {
            var voices = new List<string>();

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = txtBinaryPath != null ? txtBinaryPath.Text.Trim() : @"Z:\usr\bin\say",
                    Arguments = "-v ?",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var process = Process.Start(psi))
                {
                    if (process == null)
                    {
                        return voices;
                    }

                    string output = process.StandardOutput.ReadToEnd() + Environment.NewLine + process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    foreach (string line in output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        Match match = Regex.Match(line, @"^(?<name>.+?)\s+(?<locale>[a-z]{2}(?:_[A-Z]{2})?)\s+#", RegexOptions.CultureInvariant);
                        if (match.Success)
                        {
                            string voiceName = match.Groups["name"].Value.Trim();
                            if (!string.IsNullOrWhiteSpace(voiceName) && !voices.Contains(voiceName))
                            {
                                voices.Add(voiceName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SafeLog("Voice list unavailable: " + ex.Message);
            }

            return voices;
        }

        private void SaveSettings()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(settingsFile));

            using (var fs = new FileStream(settingsFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            using (var xWriter = new XmlTextWriter(fs, System.Text.Encoding.UTF8))
            {
                xWriter.Formatting = Formatting.Indented;
                xWriter.Indentation = 1;
                xWriter.IndentChar = '\t';
                xWriter.WriteStartDocument(true);
                xWriter.WriteStartElement("Config");
                xWriter.WriteStartElement("SettingsSerializer");
                xmlSettings.ExportToXml(xWriter);
                xWriter.WriteEndElement();
                xWriter.WriteEndElement();
                xWriter.WriteEndDocument();
                xWriter.Flush();
            }
        }
    }
}
