using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Kei.Gui
{
    public class KeiGuiOptions : ICloneable
    {

        private static readonly KeiGuiOptions _default = new KeiGuiOptions()
        {
            AutoStartAndConnect = false,
            ForceBroadcastTime = TimeSpan.FromMinutes(1.5),
            LocalIntranetAddress = "192.168.0.1",
            LocalKClientPort = 9029,
            LocalTrackerServerPort = 9057,
            _targetEndPoints = new List<string>()
            {
                "192.168.46.38:9029",
            },
        };

        public static KeiGuiOptions Default
        {
            get
            {
                return _default;
            }
        }

        public static KeiGuiOptions Current
        {
            get;
            set;
        }

        private KeiGuiOptions()
        {
        }

        private List<string> _targetEndPoints = new List<string>();

        public TimeSpan ForceBroadcastTime
        {
            get;
            set;
        }

        public string LocalIntranetAddress
        {
            get;
            set;
        }

        public int LocalKClientPort
        {
            get;
            set;
        }

        public int LocalTrackerServerPort
        {
            get;
            set;
        }

        public List<string> TargetEndPoints
        {
            get
            {
                return _targetEndPoints;
            }
        }

        public bool AutoStartAndConnect
        {
            get;
            set;
        }

        public void Save(XmlWriter writer)
        {
            writer.WriteStartDocument(true);

            writer.WriteStartElement("kei-gui");
            writer.WriteAttributeString("config-version", "1.0");

            writer.WriteElementString("LocalIntranetAddress", LocalIntranetAddress);
            writer.WriteElementString("LocalKClientPort", LocalKClientPort.ToString());
            writer.WriteElementString("LocalTrackerServerPort", LocalTrackerServerPort.ToString());
            writer.WriteElementString("ForceBroadcastTime", ForceBroadcastTime.ToString());

            writer.WriteStartElement("TargetEndPoints");
            foreach (var item in TargetEndPoints)
            {
                writer.WriteElementString("EndPoint", item);
            }
            writer.WriteEndElement();

            writer.WriteEndElement();

            writer.WriteEndDocument();
        }

        public static KeiGuiOptions Read(XmlReader reader)
        {
            var kg = new KeiGuiOptions();

            reader.ReadStartElement("kei-gui");

            kg.LocalIntranetAddress = reader.ReadElementString("LocalIntranetAddress");
            kg.LocalKClientPort = Convert.ToInt32(reader.ReadElementString("LocalKClientPort"));
            kg.LocalTrackerServerPort = Convert.ToInt32(reader.ReadElementString("LocalTrackerServerPort"));
            kg.ForceBroadcastTime = TimeSpan.Parse(reader.ReadElementString("ForceBroadcastTime"));

            reader.ReadStartElement("TargetEndPoints");
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                kg.TargetEndPoints.Add(reader.ReadElementString());
            }
            reader.ReadEndElement();

            reader.ReadEndElement();

            return kg;
        }

        public object Clone()
        {
            var kg = new KeiGuiOptions();

            kg.LocalIntranetAddress = LocalIntranetAddress;
            kg.LocalKClientPort = LocalKClientPort;
            kg.LocalTrackerServerPort = LocalTrackerServerPort;
            kg.ForceBroadcastTime = ForceBroadcastTime;

            foreach (var item in TargetEndPoints)
            {
                kg.TargetEndPoints.Add(item);
            }

            return kg;
        }
    }
}
