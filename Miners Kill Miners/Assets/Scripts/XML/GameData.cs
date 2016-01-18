using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
namespace Roland
{
    public class GameData 
    {
        [XmlAttribute("name")]
        public string Name;

        public int number;
        
    }
}