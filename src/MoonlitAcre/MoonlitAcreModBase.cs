using BepInEx;
using UnityEngine;
using Colour = UnityEngine.Color;
using ReaperLizard;
using System.Reflection;
using MonoMod.RuntimeDetour;
using static SandboxUnlockCore.Main;

namespace MoonlitAcre {

    [BepInPlugin("com.rainworldgame.halloweegionjam.plugin", "Halloweegion Jam (Moonlit Acre)", "1.0")]
    public class MoonlitAcreModBase : BaseUnityPlugin {
        private void OnEnable() {
            //SovSam and LB's colorful daddies
            FestiveRot.ApplyHooks();

            On.AbstractConsumable.IsTypeConsumable += AbstractConsumableOnIsTypeConsumable;
            On.Room.Loaded += RoomOnLoaded;
            On.AbstractPhysicalObject.Realize += AbstractPhysicalObjectOnRealize;
            On.Player.Grabability += PlayerOnGrabability;
            On.RainWorld.Start += RainWorldOnStart;
            On.ItemSymbol.SpriteNameForItem += ItemSymbolOnSpriteNameForItem;
            On.ItemSymbol.ColorForItem += ItemSymbolOnColorForItem;
            On.MultiplayerUnlocks.SandboxUnlockForSymbolData += MultiplayerUnlocksOnSandboxUnlockForSymbolData;
            On.MultiplayerUnlocks.SymbolDataForSandboxUnlock += MultiplayerUnlocksOnSymbolDataForSandboxUnlock;

            //Lightning Colour change by LB Gamer
            On.Lightning.ctor += LightningOnCtor;

            //Reaper lizards shouldn't replace magentas and be always unlocked anymore
            new Hook(
                typeof(NLELizard).GetMethod("SaveState_AbstractCreatureFromString", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance),
                typeof(MoonlitAcreModBase).GetMethod(nameof(NLELizard_SaveState_AbstractCreatureFromStringHK), BindingFlags.NonPublic | BindingFlags.Static));
            new Hook(
                typeof(NLELizard).GetMethod("SaveState_AbstractCreatureToString", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance),
                typeof(MoonlitAcreModBase).GetMethod(nameof(NLELizard_SaveState_AbstractCreatureToStringHK), BindingFlags.NonPublic | BindingFlags.Static));
            new Hook(
                typeof(NLESandbox).GetMethod("Unlock", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance),
                typeof(MoonlitAcreModBase).GetMethod(nameof(NLESandbox_UnlockHK), BindingFlags.NonPublic | BindingFlags.Static));
        }

        private IconSymbol.IconSymbolData MultiplayerUnlocksOnSymbolDataForSandboxUnlock(On.MultiplayerUnlocks.orig_SymbolDataForSandboxUnlock orig, MultiplayerUnlocks.SandboxUnlockID unlockID)
            => unlockID == EnumExt_MoonlitAcre.PumpkinUnlock 
            ? new(CreatureTemplate.Type.StandardGroundCreature, EnumExt_MoonlitAcre.Pumpkin, 0) 
            : orig(unlockID);

        private MultiplayerUnlocks.SandboxUnlockID MultiplayerUnlocksOnSandboxUnlockForSymbolData(On.MultiplayerUnlocks.orig_SandboxUnlockForSymbolData orig, IconSymbol.IconSymbolData data)
            => data.itemType == EnumExt_MoonlitAcre.Pumpkin 
            ? EnumExt_MoonlitAcre.PumpkinUnlock 
            : orig(data);

        private static AbstractCreature NLELizard_SaveState_AbstractCreatureFromStringHK(On.SaveState.orig_AbstractCreatureFromString orig, World world, string creatureString, bool onlyInCurrentRegion) 
            => orig(world, creatureString, onlyInCurrentRegion);

        private static string NLELizard_SaveState_AbstractCreatureToStringHK(On.SaveState.orig_AbstractCreatureToString_AbstractCreature_WorldCoordinate orig, AbstractCreature critter, WorldCoordinate pos)
            => orig(critter, pos);

        private static bool NLESandbox_UnlockHK(On.MultiplayerUnlocks.orig_SandboxItemUnlocked orig, MultiplayerUnlocks self, MultiplayerUnlocks.SandboxUnlockID unlock)
            => orig(self, unlock);

        private Colour ItemSymbolOnColorForItem(On.ItemSymbol.orig_ColorForItem orig, AbstractPhysicalObject.AbstractObjectType itemType, int intData) {
            if (itemType == EnumExt_MoonlitAcre.Pumpkin)
                return new(0.608f, 0.22f, 0f);
            return orig(itemType, intData);
        }

        private string ItemSymbolOnSpriteNameForItem(On.ItemSymbol.orig_SpriteNameForItem orig, AbstractPhysicalObject.AbstractObjectType itemType, int intData) {
            if (itemType == EnumExt_MoonlitAcre.Pumpkin)
                return "pumpkinicon";
            return orig(itemType, intData);
        }

        private int PlayerOnGrabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj) {
            var result = orig(self, obj);
            return obj is Pumpkin ? (int)Player.ObjectGrabability.OneHand : result;
        }

        private void RainWorldOnStart(On.RainWorld.orig_Start orig, RainWorld self) {
            orig(self);
            EmbeddedResourceLoader.LoadEmbeddedResource("pumpkin2");
            for (int i = 1; i <= 2; i++) EmbeddedResourceLoader.LoadEmbeddedResource("pumpkinbit" + i + "2");
            EmbeddedResourceLoader.LoadEmbeddedResource("pumpkinicon");
            items.Add(EnumExt_MoonlitAcre.PumpkinUnlock);
        }

        private void LightningOnCtor(On.Lightning.orig_ctor orig, Lightning self, Room room, float intensity, bool bkgOnly) {
            orig(self, room, intensity, bkgOnly);
            if (room.world.region != null && room.world.region.name == "HW") {
                self.bkgGradient = new Colour[2];
                self.bkgGradient[0] = new(0.984313727f, 0.564705881f, 0.160784325f);
                self.bkgGradient[1] = new(1f, 0.5f, 0f);
            }
        }

        private void AbstractPhysicalObjectOnRealize(On.AbstractPhysicalObject.orig_Realize orig, AbstractPhysicalObject self) {
            orig(self);
            if (self.type == EnumExt_MoonlitAcre.Pumpkin) {
                Debug.Log("REALIZED PUMPKIN");
                self.realizedObject = new Pumpkin(self);
            }
        }

        private void RoomOnLoaded(On.Room.orig_Loaded orig, Room self) {
            orig(self);
            for (int i = 0; i < self.roomSettings.placedObjects.Count; i++) {

                PlacedObject pObj = self.roomSettings.placedObjects[i];
                #if DEBUG
                
                if (pObj.type == EnumExt_MoonlitAcre.PumpkinPlacedObject) {
                    Debug.Log("COULD BE PUMPKIN");
                    Debug.Log($"ACTIVE PUMPKIN? {pObj.active}");
                    Debug.Log($"WHATEVER THE OTHER THING IS? {self.game.session is StoryGameSession gS && gS.saveState.ItemConsumed(self.world, false, self.abstractRoom.index, i)}");
                }
                
                #endif
                
                if (!pObj.active || pObj.type != EnumExt_MoonlitAcre.PumpkinPlacedObject) continue;
                if (self.game.session is StoryGameSession gameSession && gameSession.saveState.ItemConsumed(self.world, false, self.abstractRoom.index, i)) continue;
                
                Debug.Log("VALID PUMPKIN");
                
                var consumable = new AbstractConsumable(self.world, EnumExt_MoonlitAcre.Pumpkin, null, self.GetWorldCoordinate(pObj.pos), self.game.GetNewID(), self.abstractRoom.index, i, null) {
                    isConsumed = false
                };
                self.abstractRoom.entities.Add(consumable);
            }
        }

        private bool AbstractConsumableOnIsTypeConsumable(On.AbstractConsumable.orig_IsTypeConsumable orig, AbstractPhysicalObject.AbstractObjectType type) {
            return orig(type) || type == EnumExt_MoonlitAcre.Pumpkin;
        }
    }
}
