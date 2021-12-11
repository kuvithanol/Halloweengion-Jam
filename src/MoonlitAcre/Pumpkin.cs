using System;
using RWCustom;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MoonlitAcre {
    public class Pumpkin : PlayerCarryableItem, IDrawable, IPlayerEdible {
        public Vector2 rotation;
        public Vector2 lastRotation;
        public Vector2? setRotation;

        public Pumpkin(AbstractPhysicalObject abstractPhysicalObject) : base(abstractPhysicalObject) {
            //Stripped from DangleFruit

            bodyChunks = new BodyChunk[1];
            bodyChunks[0] = new(this, 0, new(0.0f, 0.0f), 8f, 0.9f);
            bodyChunkConnections = new BodyChunkConnection[0];
            airFriction = 0.999f;
            gravity = 0.9f;
            bounce = 0.2f;
            surfaceFriction = 0.7f;
            collisionLayer = 1;
            waterFriction = 0.95f;
            buoyancy = 1.1f;
        }

        public override void PlaceInRoom(Room placeRoom) {
            base.PlaceInRoom(placeRoom);
            firstChunk.HardSetPosition(placeRoom.MiddleOfTile(abstractPhysicalObject.pos));
            rotation = Custom.RNV();
            lastRotation = rotation;
        }

        public override void Update(bool eu) {
            base.Update(eu);
            if (room.game.devToolsActive && Input.GetKey("b"))
                firstChunk.vel += Custom.DirVec(firstChunk.pos, Input.mousePosition) * 3f;
            lastRotation = rotation;
            if (grabbedBy.Count > 0) {
                rotation = Custom.PerpendicularVector(Custom.DirVec(firstChunk.pos, grabbedBy[0].grabber.mainBodyChunk.pos));
                rotation.y = Mathf.Abs(rotation.y);
            }

            if (setRotation.HasValue) {
                rotation = setRotation.Value;
                setRotation = new Vector2?();
            }

            if (firstChunk.ContactPoint.y < 0) {
                rotation = (rotation - Custom.PerpendicularVector(rotation) * 0.1f * firstChunk.vel.x).normalized;
                firstChunk.vel.x *= 0.8f;
            }
        }

        public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam) {
            sLeaser.sprites = new FSprite[1];
            sLeaser.sprites[0] = new("pumpkin2") { scale = 1 };
            AddToContainer(sLeaser, rCam, null);
        }

        public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos) {
            Vector2 pos = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);
            Vector2 v = Vector3.Slerp(lastRotation, rotation, timeStacker);
            sLeaser.sprites[0].SetPosition(pos - camPos);
            sLeaser.sprites[0].rotation = Custom.VecToDeg(v);
            if (BitesLeft is not 0 and not 3) {
                sLeaser.sprites[0].element = Futile.atlasManager.GetElementWithName("pumpkinbit" + (3 - BitesLeft) + "2");
            }

            //sLeaser.sprites[0].color = blink <= 0 || Random.value >= 0.5 ? color : blinkColor;
            if (!slatedForDeletetion && room == rCam.room)
                return;
            sLeaser.CleanSpritesAndRemove();
        }

        public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette) {
        }

        public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner) {
            newContatiner ??= rCam.ReturnFContainer("Items");
            foreach (var fSprite in sLeaser.sprites) fSprite.RemoveFromContainer();

            newContatiner.AddChild(sLeaser.sprites[0]);
        }

        public void BitByPlayer(Creature.Grasp grasp, bool eu) {
            --BitesLeft;
            room.PlaySound(BitesLeft != 0 ? SoundID.Slugcat_Bite_Dangle_Fruit : SoundID.Slugcat_Eat_Dangle_Fruit, firstChunk.pos);
            firstChunk.MoveFromOutsideMyUpdate(eu, grasp.grabber.mainBodyChunk.pos);
            if (BitesLeft >= 1)
                return;
            ((Player)grasp.grabber).ObjectEaten(this);
            grasp.Release();
            Destroy();
        }

        public void ThrowByPlayer() {
            throw new NotImplementedException();
        }

        public int BitesLeft { get; private set; } = 3;
        public int FoodPoints => 2;
        public bool Edible => true;
        public bool AutomaticPickUp => true;
    }
}
