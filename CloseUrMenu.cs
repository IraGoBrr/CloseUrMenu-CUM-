//thanks to HimboHouseHusbandForHire for inspiring this mod, and Lenvoran for testing.
//https://twitch.tv/iragobrr if you wanna see my terrible mods in action
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
namespace CloseUrMenu
{
    public class CloseUrMenuSystem : ModSystem
    {
        private ICoreClientAPI capi;
        // if you experience lag for some reason, set this to higher.
        private const int CheckIntervalMs = 25;
        private double lastX, lastZ;
        private bool havePrevPos;
        private static readonly HashSet<string> InventoryDialogTypeNames = new HashSet<string>
        {
            "GuiDialogInventory",
        // if you want to close ur character menu too, delete this comment.    "GuiDialogCharacter",
        };
        public override void StartClientSide(ICoreClientAPI api)
        {
            capi = api;
            capi.Event.RegisterGameTickListener(OnClientTick, CheckIntervalMs);
        }
        private void OnClientTick(float dt)
        {
            EntityPlayer player = capi.World?.Player?.Entity;
            if (player == null) return;
            bool moving;
            if (player.MountedOn != null)
        // i might fiddle with this if mounted stuff doesn't feel good.
            {
                double x = player.Pos.X, z = player.Pos.Z;
                double dx = x - lastX, dz = z - lastZ;
                moving = havePrevPos && (dx * dx + dz * dz) > 0.0001;
                lastX = x; lastZ = z;
                havePrevPos = true;
            }
            else
            {
                moving = player.Controls.TriesToMove;
                havePrevPos = false;
            }
            if (!moving) return;
            foreach (object gui in new List<object>(capi.OpenedGuis))
            {
                if (gui is GuiDialog dlg && InventoryDialogTypeNames.Contains(dlg.GetType().Name))
                    dlg.TryClose();
            }
        }
    }
}
