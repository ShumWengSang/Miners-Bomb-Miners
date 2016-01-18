using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Roland
{
    [System.Serializable]
    [XmlRoot("GameDataContainer")]
    public class GameDataContainer
    {
        [XmlArray("GameDatas")]
        [XmlArrayItem("GameData")]
        public List<GameData> playerDatas = new List<GameData>();

        public void Save(string path)
        {
            var serializer = new XmlSerializer(typeof(GameDataContainer));
            using (var stream = new FileStream(path, FileMode.Create))
            {
                serializer.Serialize(stream, this);
            }
        }

        public static GameDataContainer Load(string path)
        {
            var serializer = new XmlSerializer(typeof(GameDataContainer));
            using (var stream = new FileStream(path, FileMode.Open))
            {
                return serializer.Deserialize(stream) as GameDataContainer;
            }
        }

        //Loads the xml directly from the given string. Useful in combination with www.text.
        public static GameDataContainer LoadFromText(string text)
        {
            var serializer = new XmlSerializer(typeof(GameDataContainer));
            return serializer.Deserialize(new StringReader(text)) as GameDataContainer;
        }
    }
}