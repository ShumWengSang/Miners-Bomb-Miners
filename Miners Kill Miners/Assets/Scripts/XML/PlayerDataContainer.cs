using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;


namespace Roland
{
    [System.Serializable]
    [XmlRoot("PlayerDataCollection")]
    public class PlayerDataContainer
    {
        [XmlArray("PlayerDatas")]
        [XmlArrayItem("PlayerData")]
        public List<PlayerData> playerDatas = new List<PlayerData>();

        public void Save(string path)
        {
            var serializer = new XmlSerializer(typeof(PlayerDataContainer));
            using (var stream = new FileStream(path, FileMode.Create))
            {
                serializer.Serialize(stream, this);
            }
        }

        public static PlayerDataContainer Load(string path)
        {
            var serializer = new XmlSerializer(typeof(PlayerDataContainer));
            using (var stream = new FileStream(path, FileMode.Open))
            {
                return serializer.Deserialize(stream) as PlayerDataContainer;
            }
        }

        //Loads the xml directly from the given string. Useful in combination with www.text.
        public static PlayerDataContainer LoadFromText(string text)
        {
            var serializer = new XmlSerializer(typeof(PlayerDataContainer));
            return serializer.Deserialize(new StringReader(text)) as PlayerDataContainer;
        }
    }
}