using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using System.IO;
using Terraria.ID;

namespace NPCDebuffModifier
{
    public class Config
    {
        private static string filepath = Path.Combine(TShock.SavePath, "NPCBuffs.json");

        public Dictionary<int, int> Buffs = new Dictionary<int, int>()
        {
            { BuffID.OnFire, 5 }
        };
        public double OilDamageMultiplier { get; set; } = 2;
        public int BuffTimerInMilliseconds { get; set; } = 250;

        private static void Write(Config file)
        {
            try
            {
                File.WriteAllText(filepath, JsonConvert.SerializeObject(file, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception at 'Config.Write': {0}",
                        ex.Message);
                Console.WriteLine(ex.ToString());
            }
        }

        public static Config Read()
        {

            Config file = new Config();
            try
            {
                if (!File.Exists(filepath))
                {
                    Write(file);
                }
                else
                {
                    file = JsonConvert.DeserializeObject<Config>(File.ReadAllText(filepath), new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                    File.WriteAllText(filepath, JsonConvert.SerializeObject(file, Formatting.Indented));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception at 'Config.Read': {0}\nCheck logs for details.",
                        ex.Message);
                Console.WriteLine(ex.ToString());
            }
            return file;
        }
    }
}
