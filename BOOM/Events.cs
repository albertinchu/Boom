using System.Collections.Generic;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2;
using scp4aiur;
using Smod2.EventSystem.Events;
using System.Linq;


namespace BOOM
{
    partial class PlayersEvents : IEventHandlerPlayerDie, IEventHandlerSetRole, IEventHandlerThrowGrenade, IEventHandlerSetSCPConfig, IEventHandlerSetConfig,
        IEventHandlerCheckRoundEnd, IEventHandlerWaitingForPlayers, IEventHandlerRoundEnd, IEventHandlerElevatorUse,
        IEventHandlerPlayerHurt, IEventHandlerPlayerPickupItem

    {
        /////////////////////////////////////////////////////////////////Variables////////////////////////////////////////////////////////////////
        static Dictionary<string, int> Jugadores = new Dictionary<string, int>();
        private Boom plugin;

        public PlayersEvents(Boom plugin)
        {
            this.plugin = plugin;
        }        
      
        int contador = 0; int Scientists = 0; int Dboys = 0; int contadorpos = 0;     
        static string MVP = "nadie"; string mejor = "Nadie";

        public void OnPlayerDie(PlayerDeathEvent ev)
        {
            if (ev.Player.TeamRole.Role == Role.SCIENTIST)
            {
                Timing.Run(Respawn(ev.Player));
            }
            if((ev.Player.TeamRole.Role == Role.CLASSD)&&(contadorpos == 0))
            {
                Timing.Run(Respawn(ev.Player));
                contadorpos = 1;
            }
            if ((ev.Player.TeamRole.Role == Role.CLASSD) && (contadorpos == 1))
            {
                Timing.Run(RespawnD1(ev.Player));
                contadorpos = 2;
            }
            if ((ev.Player.TeamRole.Role == Role.CLASSD) && (contadorpos == 2))
            {
                Timing.Run(RespawnD2(ev.Player));
                contadorpos = 0;
            }


            ev.Killer.GiveItem(ItemType.FRAG_GRENADE);
            if (ev.DamageTypeVar == DamageType.FRAG)
            {
                Jugadores.Add(ev.Killer.SteamId,Jugadores[ev.Killer.SteamId]+ 1);
                ev.Killer.GiveItem(ItemType.FRAG_GRENADE);
                ev.Player.SendConsoleMessage("Has muerto, tu asesino fue:" + ev.Killer.Name, "green");
               
                if ((ev.Player.TeamRole.Role == Role.SCIENTIST) && ((ev.Killer.TeamRole.Role == Role.SPECTATOR)||(ev.Killer.TeamRole.Team == Team.CLASSD)) && (ev.Killer.SteamId != ev.Player.SteamId))
                {
                    Dboys = Dboys + 1;
                }
                if ((ev.Player.TeamRole.Role == Role.CLASSD) && ((ev.Killer.TeamRole.Team == Team.SCIENTIST)||(ev.Killer.TeamRole.Role == Role.SPECTATOR)) && (ev.Killer.SteamId != ev.Player.SteamId))
                {
                    Scientists = Scientists + 1;
                }
            }
        }

        public void OnSetRole(PlayerSetRoleEvent ev)
        {
            if (contador == 0) {
                foreach (Player player in PluginManager.Manager.Server.GetPlayers())
                {
                    Jugadores.Add(player.SteamId, 0);
                    contador = 1;
                }
            }
            if (ev.Player.TeamRole.Team != Team.SCP){ ev.Player.GiveItem(ItemType.FRAG_GRENADE); }
        }


        public static IEnumerable<float> Respawn(Player player)
        {
            var rolep = player.TeamRole.Role;
            player.SendConsoleMessage("Respawnearas en 5 segundos," + player.Name, "blue");
            
            yield return 5f;
            player.ChangeRole(rolep);          
        }
   

        public static IEnumerable<float> RespawnD1(Player player)
        {
            var rolep = player.TeamRole.Role;
            player.SendConsoleMessage("Respawnearas en 5 segundos," + player.Name, "blue");

            yield return 5f;
            player.ChangeRole(rolep);
            player.Teleport(PluginManager.Manager.Server.Map.GetSpawnPoints(Role.SCIENTIST).First());
        }
        public static IEnumerable<float> RespawnD2(Player player)
        {
            var rolep = player.TeamRole.Role;
            player.SendConsoleMessage("Respawnearas en 5 segundos," + player.Name, "blue");

            yield return 5f;
            player.ChangeRole(rolep);
            player.Teleport(PluginManager.Manager.Server.Map.GetSpawnPoints(Role.SCP_173).First());
        }

        public static IEnumerable<float> Granada(Player player)
        {         
            yield return 5f;
            player.GiveItem(ItemType.FRAG_GRENADE);
        }

        public void OnThrowGrenade(PlayerThrowGrenadeEvent ev)
        {
            Timing.Run(Granada(ev.Player));
        }

        public void OnSetSCPConfig(SetSCPConfigEvent ev)
        {
            ev.Ban049 = true;
            ev.Ban079 = true;
            ev.Ban096 = true;
            ev.Ban106 = true;
            ev.Ban173 = true;
            ev.Ban939_53 = true;
            ev.Ban939_89 = true;
            
        }

        public void OnSetConfig(SetConfigEvent ev)
        {
            switch (ev.Key)
            {
                case "team_respawn_queue":                 
                    ev.Value = "3434343434343434343434343434343434343434343434343434343434343434343434343434343434343434343434343434343434343434343434343434343434343";
                    break;
                case "auto_warhead_start":
                    ev.Value = 1800;
                    break;
                case "auto_warhead_start_lock":
                    ev.Value = false;
                    break;
                case "default_item_classd":
                    ev.Value = new int[] { 25 };
                    break;
                case "default_item_scientist":
                    ev.Value = new int[] { 25 };
                    break;
                case "minimum_MTF_time_to_spawn":
                    ev.Value = 10000;
                    break;
                case "maximum_MTF_time_to_spawn":
                    ev.Value = 10000;
                    break;
            }
        }

        public void OnCheckRoundEnd(CheckRoundEndEvent ev)
        {
           if(Scientists > Dboys) { ev.Status = ROUND_END_STATUS.MTF_VICTORY;  }
           if (Dboys > Scientists) { ev.Status = ROUND_END_STATUS.CI_VICTORY; }

        }

    

        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            plugin.RefreshConfig();
        }
        public static IEnumerable<float> Duracion()
        {
            yield return 900f;
            foreach(Player player in PluginManager.Manager.Server.GetPlayers())
            {
                player.Kill(DamageType.FRAG);
            }
        
        }
        
        public void OnRoundEnd(RoundEndEvent ev)
        {
            MVP = Jugadores.Keys.Max();
            foreach(Player player in PluginManager.Manager.Server.GetPlayers())
            {
                if(MVP == player.SteamId){ mejor = player.Name; }
            }

            if (Scientists > Dboys)
            {
                PluginManager.Manager.Server.Map.Broadcast(6, "Ganan Los Científicos, el mejor jugador fué" + mejor, false);
            }
            if(Dboys > Scientists)
            {
                PluginManager.Manager.Server.Map.Broadcast(6, "Ganan Los Clases D, el mejor jugador  fué" + mejor, true);
            }
            if(Dboys == Scientists)
            {
                PluginManager.Manager.Server.Map.Broadcast(6, "¡EMPATE xD!, El mejor jugador fué" + mejor, true);
            }

        }

        public void OnElevatorUse(PlayerElevatorUseEvent ev)
        {
            ev.AllowUse = false;
        }

        public void OnPlayerHurt(PlayerHurtEvent ev)
        {
           
            if ((ev.DamageType == DamageType.COM15)||(ev.DamageType != DamageType.USP)||(ev.DamageType == DamageType.P90)||(ev.DamageType == DamageType.LOGICER) || (ev.DamageType == DamageType.E11_STANDARD_RIFLE) || (ev.DamageType == DamageType.MP7))
            {
                if (ev.Attacker.TeamRole.Role == Role.CLASSD)
                {
                    Scientists = Scientists + 1;
                    ev.Attacker.Kill(DamageType.LURE);
                }
                if (ev.Attacker.TeamRole.Role == Role.SCIENTIST)
                {
                    Dboys = Dboys + 1;
                    ev.Attacker.Kill(DamageType.LURE);
                }
            }
        }

        public void OnPlayerPickupItem(PlayerPickupItemEvent ev)
        {
            if((ev.Item.ItemType == ItemType.COM15)|| (ev.Item.ItemType == ItemType.USP) || (ev.Item.ItemType == ItemType.P90) || (ev.Item.ItemType == ItemType.LOGICER) || (ev.Item.ItemType == ItemType.E11_STANDARD_RIFLE) || (ev.Item.ItemType == ItemType.MP4))
            {
                ev.ChangeTo = ItemType.FRAG_GRENADE;
            }
        }
    }
}



