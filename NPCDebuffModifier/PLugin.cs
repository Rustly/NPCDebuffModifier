using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaApi.Server;
using Terraria;
using Terraria.ID;
using System.Timers;
using TShockAPI;
using System.Reflection;

namespace NPCDebuffModifier
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        public override string Name => "NPCDebuffModifier";
        public override string Author => "Rustly";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;
        public override string Description => "Increases damage values on debuffs.";

        public static Config Config { get; set; }
        private DateTime LastUpdate { get; set; }

        public override void Initialize()
        {
            Config = Config.Read();
            LastUpdate = DateTime.UtcNow;
            TShockAPI.Hooks.GeneralHooks.ReloadEvent += OnReload;
            ServerApi.Hooks.GameUpdate.Register(this, OnGameUpdate);
        }

        public Plugin(Main game) : base(game)
        {

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameUpdate.Deregister(this, OnGameUpdate);
                TShockAPI.Hooks.GeneralHooks.ReloadEvent -= OnReload;
            }
            base.Dispose(disposing);
        }

        private void OnReload(TShockAPI.Hooks.ReloadEventArgs args)
        {
            Config = Config.Read();
        }

        private int[] fireBuffs = new int[]
        {
            BuffID.OnFire, BuffID.CursedInferno, BuffID.Frostburn, BuffID.ShadowFlame
        };

        private void OnGameUpdate(EventArgs args)
        {
            if ((DateTime.UtcNow - LastUpdate).TotalMilliseconds >= Config.BuffTimerInMilliseconds)
            {
                if (TShock.Utils.GetActivePlayerCount() == 0)
                {
                    LastUpdate = DateTime.UtcNow;
                    return;
                }

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    var npc = Main.npc[i];
                    if (npc == null || npc.type == 0 || npc.buffType.All(b => b <= 0))
                        continue;

                    foreach (var buff in npc.buffType)
                    {
                        if (Config.Buffs.ContainsKey(buff))
                        {
                            bool oil = npc.buffType.Any(b => b == BuffID.Oiled);
                            int damage = Config.Buffs[buff];
                            if (fireBuffs.Contains(buff) && oil)
                                damage = (int)(damage * Config.OilDamageMultiplier);
                            npc.life -= damage;
                            npc.netUpdate = true;
                            if (npc.life <= 0)
                            {
                                npc.life = 1;
                                break;
                            }
                        }
                    }
                }

                LastUpdate = DateTime.UtcNow;
            }
        }
    }
}
