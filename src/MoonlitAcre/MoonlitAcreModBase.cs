using BepInEx;
using UnityEngine;
using Colour = UnityEngine.Color;

namespace MoonlitAcre {
    
    [BepInPlugin("com.rainworldgame.halloweegionjam.plugin", "Halloweegion Jam (Moonlit Acre)", "1.0")]
    public class MoonlitAcreModBase : BaseUnityPlugin {
        private void OnEnable() {
            
            On.AbstractConsumable.IsTypeConsumable += AbstractConsumableOnIsTypeConsumable;
            On.Room.Loaded += RoomOnLoaded;
            On.AbstractPhysicalObject.Realize += AbstractPhysicalObjectOnRealize;
            On.Player.Grabability += PlayerOnGrabability;
            On.RainWorld.Start += RainWorldOnStart;
            
            //Lightning Colour change by LB Gamer
            On.Lightning.ctor += LightningOnCtor;
        }

        private int PlayerOnGrabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj) {
            var result = orig(self, obj);
            return obj is Pumpkin ? (int)Player.ObjectGrabability.OneHand : result;
        }

        private void RainWorldOnStart(On.RainWorld.orig_Start orig, RainWorld self) {
            orig(self);
            EmbeddedResourceLoader.LoadEmbeddedResource("pumpkin");
        }

        private void LightningOnCtor(On.Lightning.orig_ctor orig, Lightning self, Room room, float intensity, bool bkgOnly) {
            orig(self, room, intensity, bkgOnly);
            if (room.world.region.name == "HW") {
                self.bkgGradient = new Colour[2];
                self.bkgGradient[0] = new Colour(0.984313727f, 0.564705881f, 0.160784325f);
                self.bkgGradient[1] = new Colour(1f, 0.5f, 0f);
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