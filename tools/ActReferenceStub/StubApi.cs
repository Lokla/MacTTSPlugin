using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Advanced_Combat_Tracker
{
    public interface IActPluginV1
    {
        void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText);
        void DeInitPlugin();
    }

    public class FormActMain
    {
        public delegate void PlayTtsDelegate(string text);

        public PlayTtsDelegate PlayTtsMethod { get; set; }

        public DirectoryInfo AppDataFolder { get; set; } = new DirectoryInfo(Path.GetTempPath());
    }

    public static class ActGlobals
    {
        public static FormActMain oFormActMain { get; } = new FormActMain();
    }

    public class SettingsSerializer
    {
        public SettingsSerializer(Control owner)
        {
        }

        public void AddControlSetting(string name, Control control)
        {
        }

        public void ImportFromXml(XmlReader reader)
        {
        }

        public void ExportToXml(XmlWriter writer)
        {
        }
    }
}
