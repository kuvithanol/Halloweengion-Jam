using RWCustom;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MoonlitAcre {
    public class FestiveRot {
        internal static void ApplyHooks() {
            On.DaddyGraphics.InitiateSprites += DaddyGraphicsOnInitiateSprites;
            On.RoomCamera.SpriteLeaser.CleanSpritesAndRemove += SpriteLeaserOnCleanSpritesAndRemove;
            On.DaddyGraphics.DrawSprites += DaddyGraphicsOnDrawSprites;
            On.DaddyLongLegs.ctor += DaddyLongLegsOnCtor;
            On.DaddyGraphics.DaddyTubeGraphic.ApplyPalette += DaddyTubeGraphicOnApplyPalette;
            On.DaddyGraphics.DaddyDangleTube.ApplyPalette += DaddyDangleTubeOnApplyPalette;
            On.ShortcutGraphics.ShortCutColor += ShortcutGraphicsOnShortCutColor; ;
            On.DaddyCorruption.ctor += DaddyCorruptionOnCtor;
            On.DaddyCorruption.CorruptionTube.TubeGraphic.DrawSprite += TubeGraphicOnDrawSprite;
            On.DaddyCorruption.CorruptionTube.TubeGraphic.ApplyPalette += TubeGraphicOnApplyPalette;
        }

        private static Color ShortcutGraphicsOnShortCutColor(On.ShortcutGraphics.orig_ShortCutColor orig, ShortcutGraphics self, Creature crit, IntVector2 pos) {
            if (crit.Template.type == CreatureTemplate.Type.DaddyLongLegs && (crit as DaddyLongLegs).colorClass && self.room.world.region != null && self.room.world.region.name == "HW")
                return new(0f, 0.6f, 0.3f);
            return orig(self, crit, pos);
        }

        private static void TubeGraphicOnDrawSprite(On.DaddyCorruption.CorruptionTube.TubeGraphic.orig_DrawSprite orig, DaddyCorruption.CorruptionTube.TubeGraphic self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos) {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            if (self.owner.room.world.region != null && self.owner.room.world.region.name == "HW" && !self.owner.owner.GWmode) {
                foreach (FSprite s in sLeaser.sprites.AsEnumerable().Where(x => x.color == new Color(0.7f, 0f, 0f)).ToList()) {
                    s.color = new(0f, 0.6f, 0.3f);
                }
                foreach (FSprite s in sLeaser.sprites.AsEnumerable().Where(x => x.color.r > 0.1f && x is not TriangleMesh).ToList()) {
                    s.color = new(0.8f, 0f, 0f);
                }
            }
        }

        private static void TubeGraphicOnApplyPalette(On.DaddyCorruption.CorruptionTube.TubeGraphic.orig_ApplyPalette orig, DaddyCorruption.CorruptionTube.TubeGraphic self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette) {
            orig(self, sLeaser, rCam, palette);
            if (self.owner.room.world.region != null && self.owner.room.world.region.name == "HW" && !self.owner.owner.GWmode) {
                for (int i = 0; i < (sLeaser.sprites[self.firstSprite] as TriangleMesh).vertices.Length; i++) {
                    (sLeaser.sprites[self.firstSprite] as TriangleMesh).verticeColors[i] = new(0.01f, 0.01f, 0.01f);
                }
            }
        }

        private static void DaddyCorruptionOnCtor(On.DaddyCorruption.orig_ctor orig, DaddyCorruption self, Room room) {
            orig(self, room);
            if (room.world.region != null && room.world.region.name == "HW" && !self.GWmode) {
                self.eyeColor = new(0f, 0.6f, 0.3f);
                self.effectColor = new(0.7f, 0f, 0f);
            }
        }

        private static void DaddyDangleTubeOnApplyPalette(On.DaddyGraphics.DaddyDangleTube.orig_ApplyPalette orig, DaddyGraphics.DaddyDangleTube self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette) {
            orig(self, sLeaser, rCam, palette);
            if (self.owner.daddy.room.world.region != null && self.owner.daddy.room.world.region.name == "HW" && self.owner.daddy.colorClass) {
                for (int i = 0; i < (sLeaser.sprites[self.firstSprite] as TriangleMesh).vertices.Length; i++) {
                    (sLeaser.sprites[self.firstSprite] as TriangleMesh).verticeColors[i] = new(0.01f, 0.01f, 0.01f);
                }
            }
        }

        private static void DaddyTubeGraphicOnApplyPalette(On.DaddyGraphics.DaddyTubeGraphic.orig_ApplyPalette orig, DaddyGraphics.DaddyTubeGraphic self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette) {
            orig(self, sLeaser, rCam, palette);
            if (self.owner.daddy.room.world.region != null && self.owner.daddy.room.world.region.name == "HW" && self.owner.daddy.colorClass) {
                for (int i = 0; i < (sLeaser.sprites[self.firstSprite] as TriangleMesh).vertices.Length; i++) {
                    (sLeaser.sprites[self.firstSprite] as TriangleMesh).verticeColors[i] = new(0.01f, 0.01f, 0.01f);
                }
            }
        }

        private static void DaddyLongLegsOnCtor(On.DaddyLongLegs.orig_ctor orig, DaddyLongLegs self, AbstractCreature abstractCreature, World world) {
            orig(self, abstractCreature, world);
            if (world.region != null && world.region.name == "HW" && self.colorClass) {
                self.eyeColor = new(0f, 0.6f, 0.3f);
                self.effectColor = new(0.7f, 0f, 0f);
            }
        }

        private static void DaddyGraphicsOnInitiateSprites(On.DaddyGraphics.orig_InitiateSprites orig, DaddyGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam) {
            orig(self, sLeaser, rCam);
            if (SantaHat.hats.TryGetValue(self, out List<SantaHat> selfHats) && selfHats.Count > 0 && self.daddy.room.world.region != null && self.daddy.room.world.region.name == "HW" && self.daddy.colorClass) {
                foreach (SantaHat hat in selfHats)
                    rCam.room.AddObject(hat);
            }
            else if (self.daddy.room.world.region != null && self.daddy.room.world.region.name == "HW" && self.daddy.colorClass)
            {
                /*int chunkCount = self.daddy.bodyChunks.Length;
                int firstChunkSprite = self.BodySprite(0);// (int)typeof(DaddyGraphics).GetMethod("BodySprite", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, new object[] { 0 });
                for (int i = 0; i < chunkCount; i++) {
                    if (Random.value <= 0.5f)*/
                new SantaHat(self, self.BodySprite(0), Random.value * 360f, self.daddy.bodyChunks[0].rad, false);
                //}
            }
        }

        private static void DaddyGraphicsOnDrawSprites(On.DaddyGraphics.orig_DrawSprites orig, DaddyGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos) {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            if (SantaHat.hats.TryGetValue(self, out List<SantaHat> selfHats) && self.daddy.room.world.region != null && self.daddy.room.world.region.name == "HW" && self.daddy.colorClass) {
                foreach (FSprite s in sLeaser.sprites.AsEnumerable().Where(x => x.color == new Color(0.7f, 0f, 0f)).ToList()) {
                    s.color = new(0f, 0.6f, 0.3f);
                }
                foreach (FSprite s in sLeaser.sprites.AsEnumerable().Where(x => x.color.r > 0.1f && x is not TriangleMesh).ToList()) {
                    s.color = new(0.8f, 0f, 0f);
                }
                foreach (SantaHat hat in selfHats)
                    hat.ParentDrawSprites(sLeaser);//, rCam, timeStacker, camPos);
            }
        }

        // Remove hats when a creature's sprites are removed
        private static void SpriteLeaserOnCleanSpritesAndRemove(On.RoomCamera.SpriteLeaser.orig_CleanSpritesAndRemove orig, RoomCamera.SpriteLeaser self) {
            orig(self);
            if (self.drawableObject is GraphicsModule gmod && SantaHat.hats.TryGetValue(gmod, out var hats)) {
                for (int i = hats.Count - 1; i >= 0; i--) {
                    var hat = hats[i];
                    hat.Destroy();
                    hats.Remove(hat);
                    if (hats.Count == 0)
                        SantaHat.hats.Remove(gmod);
                }
            }
        }
    }

    public class SantaHat : UpdatableAndDeletable, IDrawable {
        public static Dictionary<GraphicsModule, List<SantaHat>> hats = new();
        public GraphicsModule parent;
        public int anchorSprite;
        public float rotation;
        public float headRadius;
        public bool flipX;
        public bool flipY;
        public bool flipFlips;
        public Vector2 baseSpritePos;
        public float baseSpriteRot;
        public Vector2 tuftPos;
        private Vector2 lastTuftPos;
        public Vector2 tuftVel;
        private bool first = true;

        public SantaHat(GraphicsModule parent, int anchorSprite, float rotation, float headRadius, bool flipFlips) {
            this.parent = parent;
            this.anchorSprite = anchorSprite;
            this.rotation = rotation;
            this.headRadius = headRadius;
            this.flipFlips = flipFlips;
            if (!hats.TryGetValue(parent, out List<SantaHat> parentHats)) {
                parentHats = new();
                hats[parent] = parentHats;
            }
            parentHats.Add(this);
            parent.owner.room.AddObject(this);
        }

        public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam) {
            sLeaser.sprites = new FSprite[3];
            TriangleMesh.Triangle[] tris = new TriangleMesh.Triangle[] {
                new(0, 1, 2),
                new(1, 2, 3),
                new(2, 3, 4),
                new(3, 4, 5),
                new(4, 5, 6),
                new(5, 6, 7),
                new(6, 7, 8)
            };
            TriangleMesh triangleMesh = new("Futile_White", tris, false, false);
            sLeaser.sprites[0] = triangleMesh;
            sLeaser.sprites[1] = new("JetFishEyeA");
            sLeaser.sprites[2] = new("LizardScaleA6");
            AddToContainer(sLeaser, rCam, null);
        }

        public void ParentDrawSprites(RoomCamera.SpriteLeaser sLeaser) {//, RoomCamera rCam, float timeStacker, Vector2 camPos
            if (sLeaser.sprites.Length > anchorSprite) {
                baseSpritePos = sLeaser.sprites[anchorSprite].GetPosition();
                baseSpriteRot = sLeaser.sprites[anchorSprite].rotation;
                if (flipFlips) {
                    flipX = sLeaser.sprites[anchorSprite].scaleY > 0;
                    flipY = sLeaser.sprites[anchorSprite].scaleX < 0;
                }
                else {
                    flipX = sLeaser.sprites[anchorSprite].scaleX > 0;
                    flipY = sLeaser.sprites[anchorSprite].scaleY < 0;
                }
            }
            if (first) {
                first = false;
                tuftPos = baseSpritePos;
                lastTuftPos = baseSpritePos;
            }
        }

        public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos) {
            Vector2 drawPos = baseSpritePos;
            Vector2 upDir = new(Mathf.Cos((rotation + baseSpriteRot) * -Mathf.Deg2Rad), Mathf.Sin((rotation + baseSpriteRot) * -Mathf.Deg2Rad));
            Vector2 rightDir = -Custom.PerpendicularVector(upDir);
            if (flipY) upDir *= -1;
            if (flipX) rightDir *= -1;
            drawPos += upDir * headRadius;
            Vector2 targetTuftPos = drawPos + upDir * 20f;

            // Rim
            sLeaser.sprites[2].SetPosition(drawPos);
            sLeaser.sprites[2].rotation = rotation + baseSpriteRot;
            sLeaser.sprites[2].scaleY = flipX ? -1f : 1f;

            // Tuft
            if (!Custom.DistLess(tuftPos, targetTuftPos, 20f)) {
                tuftPos = targetTuftPos + (tuftPos - targetTuftPos).normalized * 20f;
                if (!Custom.DistLess(lastTuftPos, tuftPos, 20f))
                    lastTuftPos = tuftPos + (lastTuftPos - tuftPos).normalized * 20f;
            }

            // Cone
            sLeaser.sprites[1].SetPosition(Vector2.Lerp(lastTuftPos, tuftPos, timeStacker)); {
                TriangleMesh cone = (TriangleMesh)sLeaser.sprites[0];
                Vector2 coneTip = Vector2.Lerp(lastTuftPos, tuftPos, timeStacker);
                for (int i = 0, len = cone.vertices.Length; i < len; i++) {
                    bool r = i % 2 == 1;
                    float h = i / 2 / (float)(len - 1) * 2f;

                    Vector2 coneBase;
                    if (r)
                        coneBase = drawPos - rightDir * 7f;
                    else
                        coneBase = drawPos + rightDir * 7f;
                    Vector2 coneMid = Vector2.Lerp(coneBase, targetTuftPos, 0.5f);

                    Vector2 pos = Vector2.Lerp(Vector2.Lerp(coneBase, coneMid, h), Vector2.Lerp(coneMid, coneTip, h), h);
                    cone.MoveVertice(i, pos);
                }
            }
            if (parent.culled && !parent.lastCulled) {
                foreach (var sprite in sLeaser.sprites) sprite.isVisible = !parent.culled;
            }
            if (slatedForDeletetion || rCam.room != room || room != parent.owner.room) {
                sLeaser.CleanSpritesAndRemove();
            }
        }

        public override void Update(bool eu) {
            base.Update(eu);
            lastTuftPos = tuftPos; {
                if (!hats.TryGetValue(parent, out List<SantaHat> parentHats) || !parentHats.Contains(this))
                    Destroy();
            }
            if (parent?.owner == null || parent.owner.slatedForDeletetion || slatedForDeletetion)
                Destroy();
            else if (parent.owner.room != null) {
                Vector2 tipPos = baseSpritePos;
                Vector2 upDir = new(Mathf.Cos((rotation + baseSpriteRot) * -Mathf.Deg2Rad), Mathf.Sin((rotation + baseSpriteRot) * -Mathf.Deg2Rad));
                Vector2 rightDir = -Custom.PerpendicularVector(upDir);
                if (flipY) upDir *= -1;
                if (flipX) rightDir *= -1;
                tipPos += upDir * 20f;
                tuftVel.y -= parent.owner.gravity;
                tuftVel += rightDir * ((Vector2.Dot(rightDir, tuftPos - tipPos) > 0) ? 1.5f : -1.5f);
                tuftVel += (tipPos - tuftPos) * 0.2f;
                tuftVel *= 0.6f;
                tuftPos += tuftVel;
                if (!Custom.DistLess(tuftPos, tipPos, 13f))
                    tuftPos = tipPos + (tuftPos - tipPos).normalized * 13f;
            }
        }

        public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner) {
            if (newContatiner == null)
                newContatiner = rCam.ReturnFContainer("Items");
            for (int i = 0; i < sLeaser.sprites.Length; i++)
                sLeaser.sprites[i].RemoveFromContainer();
            for (int i = 0; i < sLeaser.sprites.Length; i++)
                newContatiner.AddChild(sLeaser.sprites[i]);
        }

        public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette) {
            sLeaser.sprites[0].color = new(0.9f, 0f, 0f);
            sLeaser.sprites[1].color = Color.white;
            sLeaser.sprites[2].color = Color.white;
        }
    }
}
