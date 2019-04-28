
using Smod2;
using Smod2.Attributes;
using scp4aiur;

namespace BOOM
{
    [PluginDetails(
          author = "Albertinchu ",
          name = "Boom",
          description = "Boooom",
          id = "albertinchu.gamemode.Boom",
          version = "1.0.0",
          SmodMajor = 3,
          SmodMinor = 0,
          SmodRevision = 0
          )]
    public class Boom : Plugin
    {

        public override void OnDisable()
        {
            this.Info("Boom - Desactivado");
        }

        public override void OnEnable()
        {
            Info("Boom - activado.");
        }

        public override void Register()
        {
            GamemodeManager.Manager.RegisterMode(this);
            Timing.Init(this);
            this.AddEventHandlers(new PlayersEvents(this));

        }
        public void RefreshConfig()
        {


        }
    }

}