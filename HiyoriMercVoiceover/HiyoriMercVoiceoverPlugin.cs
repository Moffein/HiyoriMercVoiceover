using BepInEx;
using UnityEngine.AddressableAssets;
using UnityEngine;
using BepInEx.Configuration;
using RoR2;
using HiyoriMercVoiceover.Modules;
using System.Reflection;
using System;
using Rewired.UI.ControlMapper;
using System.Runtime.CompilerServices;
using BaseVoiceoverLib;
using System.Collections.Generic;
using RiskOfOptions.Options;
using HiyoriMercVoiceover.Components;
using System.Security.Permissions;
using System.Security;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
namespace HiyoriMercVoiceover
{
    [BepInDependency(R2API.SoundAPI.PluginGUID)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.BaseVoiceoverLib", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.Schale.HiyoriMercVoiceover", "HiyoriMercVoiceover", "1.0.0")]
    public class HiyoriMercVoiceoverPlugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool> enableVoicelines;
        public static bool playedSeasonalVoiceline = false;
        public static AssetBundle assetBundle;
        public static SurvivorDef survivorDef = Addressables.LoadAssetAsync<SurvivorDef>("RoR2/Base/Merc/Merc.asset").WaitForCompletion();

        private void Awake()
        {
            Files.PluginInfo = this.Info;
            RoR2.RoR2Application.onLoad += OnLoad;
            new Content().Initialize();

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("HiyoriMercVoiceover.hiyorimercbundle"))
            {
                assetBundle = AssetBundle.LoadFromStream(stream);
            }

            SoundBanks.Init();

            InitNSE();

            enableVoicelines = base.Config.Bind<bool>(new ConfigDefinition("Settings", "Enable Voicelines"), true, new ConfigDescription("Enable voicelines when using the Hiyori Mercenary Skin."));
            enableVoicelines.SettingChanged += EnableVoicelines_SettingChanged;

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions"))
            {
                RiskOfOptionsCompat();
            }
        }

        private void EnableVoicelines_SettingChanged(object sender, EventArgs e)
        {
            RefreshNSE();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void RiskOfOptionsCompat()
        {
            RiskOfOptions.ModSettingsManager.SetModIcon(assetBundle.LoadAsset<Sprite>("hiyori"));
            RiskOfOptions.ModSettingsManager.AddOption(new CheckBoxOption(enableVoicelines));
        }

        private void OnLoad()
        {
            SkinDef targetSkin = null;
            targetSkin = Addressables.LoadAssetAsync<SkinDef>("RoR2/Base/Merc/skinMercAltPrisoner.asset").WaitForCompletion();
            /*SkinDef[] skins = SkinCatalog.FindSkinsForBody(BodyCatalog.FindBodyIndex("MercBody"));
            foreach (SkinDef skinDef in skins)
            {
                if (skinDef.name == "HiyoriSkinDef")
                {
                    targetSkin = skinDef;
                    break;
                }
            }*/

            if (!targetSkin)
            {
                Debug.LogError("HiyoriMercVoiceover: Hiyori Merc SkinDef not found. Voicelines will not work!");
            }
            else
            {
                VoiceoverInfo voiceoverInfo = new VoiceoverInfo(typeof(HiyoriMercVoiceoverComponent), targetSkin, "MercBody");
                voiceoverInfo.selectActions += HiyoriSelect;
            }
            RefreshNSE();
        }

        private void HiyoriSelect(GameObject mannequinObject)
        {
            if (!enableVoicelines.Value) return;

            bool played = false;
            if (!playedSeasonalVoiceline)
            {
                if (System.DateTime.Today.Month == 1 && System.DateTime.Today.Day == 1)
                {
                    Util.PlaySound("Play_HiyoriMerc_Lobby_Newyear", mannequinObject);
                    played = true;
                }
                else if (System.DateTime.Today.Month == 5 && System.DateTime.Today.Day == 24)
                {
                    Util.PlaySound("Play_HiyoriMerc_Lobby_bday", mannequinObject);
                    played = true;
                }
                else if (System.DateTime.Today.Month == 10 && System.DateTime.Today.Day == 31)
                {
                    Util.PlaySound("Play_HiyoriMerc_Lobby_Halloween", mannequinObject);
                    played = true;
                }
                else if (System.DateTime.Today.Month == 12 && System.DateTime.Today.Day == 25)
                {
                    Util.PlaySound("Play_HiyoriMerc_Lobby_xmas", mannequinObject);
                    played = true;
                }

                if (played) playedSeasonalVoiceline = true;
            }
            if (!played)
            {
                if (Util.CheckRoll(5f))
                {
                    Util.PlaySound("Play_HiyoriMerc_Title", mannequinObject);
                }
                else
                {
                    Util.PlaySound("Play_HiyoriMerc_Lobby", mannequinObject);
                }
            }
        }

        private void InitNSE()
        {

        }

        public void RefreshNSE()
        {
            foreach (NSEInfo nse in nseList)
            {
                nse.ValidateParams();
            }
        }

        private NetworkSoundEventDef RegisterNSE(string eventName)
        {
            NetworkSoundEventDef nse = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            nse.eventName = eventName;
            Content.networkSoundEventDefs.Add(nse);
            nseList.Add(new NSEInfo(nse));
            return nse;
        }

        public static List<NSEInfo> nseList = new List<NSEInfo>();
        public class NSEInfo
        {
            public NetworkSoundEventDef nse;
            public uint akId = 0u;
            public string eventName = string.Empty;

            public NSEInfo(NetworkSoundEventDef source)
            {
                this.nse = source;
                this.akId = source.akId;
                this.eventName = source.eventName;
            }

            private void DisableSound()
            {
                nse.akId = 0u;
                nse.eventName = string.Empty;
            }

            private void EnableSound()
            {
                nse.akId = this.akId;
                nse.eventName = this.eventName;
            }

            public void ValidateParams()
            {
                if (this.akId == 0u) this.akId = nse.akId;
                if (this.eventName == string.Empty) this.eventName = nse.eventName;

                if (!enableVoicelines.Value)
                {
                    DisableSound();
                }
                else
                {
                    EnableSound();
                }
            }
        }
    }
}
