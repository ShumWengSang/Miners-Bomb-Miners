using System.Xml;
using System.Xml.Serialization;

namespace Roland
{
    public class PlayerData
    {
        [XmlAttribute("name")]
        public string Name;

        public int AmountOfGames;

        public void CreatePlayerData(string name, int games)
        {
            Name = name;
            AmountOfGames = games;
        }
    }
}