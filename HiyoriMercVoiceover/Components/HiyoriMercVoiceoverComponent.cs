using BaseVoiceoverLib;
using RoR2;
using UnityEngine;

namespace HiyoriMercVoiceover.Components
{
    public class HiyoriMercVoiceoverComponent : BaseVoiceoverComponent
    {
        public static NetworkSoundEventDef nseHurt, nseShout, nseTitle, nseBweh, nseItai,
            nseEX1, nseEX2, nseEX3, nseEXL1, nseEXL2, nseEXL3,
            nseOwari1, nseOwari2, nseOwari3,
            nseCommon, nseNigashimasen, nseArigatou, nseGrotesque;

        private bool acquiredScepter = false;
        private float levelCooldown = 0f;
        private float lowHealthCooldown = 0f;
        private float shrineFailCooldown = 0f;
        private float specialCooldown = 0f;

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (levelCooldown > 0f) levelCooldown -= Time.fixedDeltaTime;
            if (lowHealthCooldown > 0f) lowHealthCooldown -= Time.fixedDeltaTime;
            if (shrineFailCooldown > 0f) shrineFailCooldown -= Time.fixedDeltaTime;
            if (specialCooldown > 0f) specialCooldown -= Time.fixedDeltaTime;
        }

        protected override void Start()
        {
            base.Start();
            if (inventory && inventory.GetItemCount(scepterIndex) > 0) acquiredScepter = true;
        }

        public override void PlayUtilityAuthority(GenericSkill skill)
        {
            TryPlayNetworkSound(nseShout, 0f, false);
        }

        public override void PlaySpecialAuthority(GenericSkill skill)
        {
            if (specialCooldown > 0f) return;
            bool played = false;
            if (Util.CheckRoll(50f))
            {
                played = TryPlayNetworkSound(nseCommon, 3f, false);
            }
            else
            {
                played = TryPlayNetworkSound(nseNigashimasen, 1.2f, false);
            }
            if (played) specialCooldown = 7f;
        }

        public override void PlaySpawn()
        {
            TryPlaySound("Play_HiyoriMerc_Spawn", 6.2f, true);
        }

        public override void PlayDeath()
        {
            if (Util.CheckRoll(33.333333333f))
            {
                TryPlaySound("Play_HiyoriMerc_Owari_1", 6f, true);
            }
            else
            {
                if (Util.CheckRoll(50f))
                {
                    TryPlaySound("Play_HiyoriMerc_Owari_2", 3.7f, true);
                }
                else
                {
                    TryPlaySound("Play_HiyoriMerc_Owari_3", 4.6f, true);
                }
            }
        }

        public override void PlayShrineOfChanceFailServer()
        {
            if (shrineFailCooldown > 0f) return;
            if (Util.CheckRoll(15f))
            {
                bool played;
                if (Util.CheckRoll(33.333333333f))
                {
                    played = TryPlayNetworkSound(nseOwari1, 6f, false);
                }
                else
                {
                    if (Util.CheckRoll(50f))
                    {
                        played = TryPlayNetworkSound(nseOwari2, 3.7f, false);
                    }
                    else
                    {
                        played = TryPlayNetworkSound(nseOwari3, 4.6f, false);
                    }
                }
                if (played) shrineFailCooldown = 60f;
            }
        }

        public override void PlayLevelUp()
        {
            if (levelCooldown > 0f) return;
            bool played;
            if (Util.CheckRoll(50f))
            {
                if (Util.CheckRoll(50f))
                {
                    played = TryPlaySound("Play_HiyoriMerc_Growup_1", 10f, false);
                }
                else
                {
                    played = TryPlaySound("Play_HiyoriMerc_Growup_2", 12f, false);
                }
            }
            else
            {
                if (Util.CheckRoll(50f))
                {
                    played = TryPlaySound("Play_HiyoriMerc_Growup_3", 5.7f, false);
                }
                else
                {
                    played = TryPlaySound("Play_HiyoriMerc_Growup_4", 4.7f, false);
                }
            }
            if (played) levelCooldown = 60f;
        }

        public override void PlayHurt(float percentHPLost)
        {
            if (percentHPLost >= 0.1f)
            {
                TryPlaySound("Play_HiyoriMerc_Hurt", 0f, false);
            }
        }

        public override void PlayTeleporterStart()
        {
            if (Util.CheckRoll(33.333333333f))
            {
                TryPlaySound("Play_HiyoriMerc_ExSkill_1", 4.4f, false);
            }
            else
            {
                if (Util.CheckRoll(50f))
                {
                    TryPlaySound("Play_HiyoriMerc_ExSkill_2", 5.2f, false);
                }
                else
                {
                    TryPlaySound("Play_HiyoriMerc_ExSkill_3", 6f, false);
                }
            }
        }

        public override void PlayTeleporterFinish()
        {
            if (Util.CheckRoll(33.333333333f))
            {
                TryPlaySound("Play_HiyoriMerc_ExSkill_Level_1", 6.8f, false);
            }
            else
            {
                if (Util.CheckRoll(50f))
                {
                    TryPlaySound("Play_HiyoriMerc_ExSkill_Level_2", 4.2f, false);
                }
                else
                {
                    TryPlaySound("Play_HiyoriMerc_ExSkill_Level_3", 4f, false);
                }
            }
        }

        public override void PlayVictory()
        {
            TryPlaySound("Play_HiyoriMerc_RunVictory", 7.2f, true);
        }

        public void PlayAcquireScepter()
        {
            if (acquiredScepter) return;
            TryPlaySound("Play_HiyoriMerc_AcquireScepter", 13f, true);
            acquiredScepter = true;
        }

        public void PlayAcquireLegendary()
        {
            if (Util.CheckRoll(50f))
            {
                if (Util.CheckRoll(50f))
                {
                    TryPlaySound("Play_HiyoriMerc_Relationship_1", 11f, false);
                }
                else
                {
                    TryPlaySound("Play_HiyoriMerc_Relationship_2", 5.3f, false);
                }
            }
            else
            {
                if (Util.CheckRoll(50f))
                {
                    TryPlaySound("Play_HiyoriMerc_Relationship_3", 11f, false);
                }
                else
                {
                    TryPlaySound("Play_HiyoriMerc_Relationship_4", 7.4f, false);
                }
            }
        }

        public override void PlayLowHealth()
        {
            if (lowHealthCooldown > 0f) return;
            bool playedSound = false;

            if (Util.CheckRoll(50f))
            {
                playedSound = TryPlaySound("Play_HiyoriMerc_Bweh", 2.25f, false);
            }
            else
            {
                playedSound = TryPlaySound("Play_HiyoriMerc_Itai", 5.8f, false);
            }

            if (playedSound) lowHealthCooldown = 60f;
        }

        public void PlaySquid()
        {
            TryPlaySound("Play_HiyoriMerc_Grotesque", 1.5f, false);
        }

        protected override void Inventory_onItemAddedClient(ItemIndex itemIndex)
        {
            base.Inventory_onItemAddedClient(itemIndex);
            if (scepterIndex != ItemIndex.None && itemIndex == scepterIndex)
            {
                PlayAcquireScepter();
            }
            else
            {
                ItemDef id = ItemCatalog.GetItemDef(itemIndex);
                if (id == RoR2Content.Items.Squid || id == RoR2Content.Items.NovaOnLowHealth)
                {
                    PlaySquid();
                }
                else if (id && id.deprecatedTier == ItemTier.Tier3)
                {
                    PlayAcquireLegendary();
                }
            }
        }

        protected override void CheckInputs()
        {
            if (BaseVoiceoverLib.Utils.GetKeyPressed(HiyoriMercVoiceoverPlugin.buttonHurt))
            {
                TryPlayNetworkSound(nseHurt, 0.1f, false);
                return;
            }
            if (BaseVoiceoverLib.Utils.GetKeyPressed(HiyoriMercVoiceoverPlugin.buttonShout))
            {
                TryPlayNetworkSound(nseShout, 0.1f, false);
                return;
            }
            if (BaseVoiceoverLib.Utils.GetKeyPressed(HiyoriMercVoiceoverPlugin.buttonTitle))
            {
                TryPlayNetworkSound(nseTitle, 5.6f, false);
                return;
            }
            if (BaseVoiceoverLib.Utils.GetKeyPressed(HiyoriMercVoiceoverPlugin.buttonBweh))
            {
                TryPlayNetworkSound(nseBweh, 2.25f, false);
                return;
            }
            if (BaseVoiceoverLib.Utils.GetKeyPressed(HiyoriMercVoiceoverPlugin.buttonItai))
            {
                TryPlayNetworkSound(nseItai, 5.8f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(HiyoriMercVoiceoverPlugin.buttonEX1))
            {
                TryPlayNetworkSound(nseEX1, 4.4f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(HiyoriMercVoiceoverPlugin.buttonEX2))
            {
                TryPlayNetworkSound(nseEX2, 5.2f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(HiyoriMercVoiceoverPlugin.buttonEX3))
            {
                TryPlayNetworkSound(nseEX3, 6f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(HiyoriMercVoiceoverPlugin.buttonEXL1))
            {
                TryPlayNetworkSound(nseEXL1, 6.8f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(HiyoriMercVoiceoverPlugin.buttonEXL2))
            {
                TryPlayNetworkSound(nseEXL2, 4.2f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(HiyoriMercVoiceoverPlugin.buttonEXL3))
            {
                TryPlayNetworkSound(nseEXL3, 4f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(HiyoriMercVoiceoverPlugin.buttonOwari1))
            {
                TryPlayNetworkSound(nseOwari1, 6f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(HiyoriMercVoiceoverPlugin.buttonOwari2))
            {
                TryPlayNetworkSound(nseOwari2, 3.7f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(HiyoriMercVoiceoverPlugin.buttonOwari3))
            {
                TryPlayNetworkSound(nseOwari3, 4.6f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(HiyoriMercVoiceoverPlugin.buttonCommon))
            {
                TryPlayNetworkSound(nseCommon, 3f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(HiyoriMercVoiceoverPlugin.buttonNigashimasen))
            {
                TryPlayNetworkSound(nseNigashimasen, 1.2f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(HiyoriMercVoiceoverPlugin.buttonArigatou))
            {
                TryPlayNetworkSound(nseArigatou, 4f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(HiyoriMercVoiceoverPlugin.buttonGrotesque))
            {
                TryPlayNetworkSound(nseGrotesque, 1.5f, false);
                return;
            }
        }

        public override bool ComponentEnableVoicelines()
        {
            return HiyoriMercVoiceoverPlugin.enableVoicelines.Value;
        }
    }
}
