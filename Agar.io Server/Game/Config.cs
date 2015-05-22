using System;
using System.Xml.Linq;

namespace AgarioServer.Game
{
    public class Config
    {
        public static UInt32 ServerId;
        public static Int32 RealmCount;
        public static String Host;
        public static Int32 Port;

        public static Int32 VirusCount;
        public static Int32 FoodCount;
        public static Int32 RealmWidth;
        public static Int32 RealmHeight;

        public static Double IntervalPosition;
        public static Int32 IntervalFood;
        public static Int32 IntervalVirus;
        public static Int32 IntervalRanking;

        public static String MySqlHost;
        public static String MySqlName;
        public static String MySqlUser;
        public static String MySqlPass;

        public static void Load()
        {
            XElement xml = XElement.Load("config.xml");

            XElement game = xml.Element("game");
            Config.ServerId = UInt32.Parse(game.Element("id").Value);
            Config.RealmCount = Int32.Parse(game.Element("realms").Value);
            Config.Host = game.Element("host").Value;
            Config.Port = Int32.Parse(game.Element("port").Value);

            XElement realm = xml.Element("realm");
            Config.VirusCount = Int32.Parse(realm.Element("virus").Value);
            Config.FoodCount = Int32.Parse(realm.Element("food").Value);
            Config.RealmWidth = Int32.Parse(realm.Element("width").Value);
            Config.RealmHeight = Int32.Parse(realm.Element("height").Value);

            XElement interval = xml.Element("interval");
            Config.IntervalPosition = 1000 / Int32.Parse(interval.Element("position").Value);
            Config.IntervalFood = Int32.Parse(interval.Element("food").Value) * 1000;
            Config.IntervalVirus = Int32.Parse(interval.Element("virus").Value) * 1000;
            Config.IntervalRanking = Int32.Parse(interval.Element("ranking").Value) * 1000;

            XElement database = xml.Element("database");
            Config.MySqlHost = database.Element("host").Value;
            Config.MySqlName = database.Element("name").Value;
            Config.MySqlUser = database.Element("user").Value;
            Config.MySqlPass = database.Element("pass").Value;
        }
    }
}
