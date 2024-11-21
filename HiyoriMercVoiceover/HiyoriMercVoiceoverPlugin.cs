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
        public static ConfigEntry<KeyboardShortcut> buttonHurt, buttonShout, buttonTitle, buttonBweh, buttonItai,
            buttonEX1, buttonEX2, buttonEX3, buttonEXL1, buttonEXL2, buttonEXL3,
            buttonOwari1, buttonOwari2, buttonOwari3,
            buttonCommon, buttonNigashimasen, buttonArigatou, buttonGrotesque;
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

            buttonTitle = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "Blue Archive"), KeyboardShortcut.Empty);
            buttonHurt = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "Hurt"), KeyboardShortcut.Empty);
            buttonShout = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "Shout"), KeyboardShortcut.Empty);
            buttonBweh = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "Bweh"), KeyboardShortcut.Empty);
            buttonItai = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "Itai Desu"), KeyboardShortcut.Empty);

            buttonEX1 = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "EX 1"), KeyboardShortcut.Empty);
            buttonEX2 = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "EX 2"), KeyboardShortcut.Empty);
            buttonEX3 = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "EX 3"), KeyboardShortcut.Empty);

            buttonEXL1 = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "EX Lv1"), KeyboardShortcut.Empty);
            buttonEXL2 = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "EX Lv2"), KeyboardShortcut.Empty);
            buttonEXL3 = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "EX Lv3"), KeyboardShortcut.Empty);

            buttonOwari1 = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "Owari 1"), KeyboardShortcut.Empty);
            buttonOwari2 = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "Owari 2"), KeyboardShortcut.Empty);
            buttonOwari3 = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "Owari 3"), KeyboardShortcut.Empty);

            buttonCommon = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "Common Skill"), KeyboardShortcut.Empty);
            buttonNigashimasen = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "Nigashimasen"), KeyboardShortcut.Empty);
            buttonArigatou = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "Arigatou"), KeyboardShortcut.Empty);
            buttonGrotesque = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "Grotesque"), KeyboardShortcut.Empty);

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
            RiskOfOptions.ModSettingsManager.AddOption(new KeyBindOption(buttonTitle));
            RiskOfOptions.ModSettingsManager.AddOption(new KeyBindOption(buttonHurt));
            RiskOfOptions.ModSettingsManager.AddOption(new KeyBindOption(buttonShout));
            RiskOfOptions.ModSettingsManager.AddOption(new KeyBindOption(buttonBweh));
            RiskOfOptions.ModSettingsManager.AddOption(new KeyBindOption(buttonItai));
            RiskOfOptions.ModSettingsManager.AddOption(new KeyBindOption(buttonEX1));
            RiskOfOptions.ModSettingsManager.AddOption(new KeyBindOption(buttonEX2));
            RiskOfOptions.ModSettingsManager.AddOption(new KeyBindOption(buttonEX3));
            RiskOfOptions.ModSettingsManager.AddOption(new KeyBindOption(buttonEXL1));
            RiskOfOptions.ModSettingsManager.AddOption(new KeyBindOption(buttonEXL2));
            RiskOfOptions.ModSettingsManager.AddOption(new KeyBindOption(buttonEXL3));
            RiskOfOptions.ModSettingsManager.AddOption(new KeyBindOption(buttonOwari1));
            RiskOfOptions.ModSettingsManager.AddOption(new KeyBindOption(buttonOwari2));
            RiskOfOptions.ModSettingsManager.AddOption(new KeyBindOption(buttonOwari3));
            RiskOfOptions.ModSettingsManager.AddOption(new KeyBindOption(buttonCommon));
            RiskOfOptions.ModSettingsManager.AddOption(new KeyBindOption(buttonNigashimasen));
            RiskOfOptions.ModSettingsManager.AddOption(new KeyBindOption(buttonArigatou));
            RiskOfOptions.ModSettingsManager.AddOption(new KeyBindOption(buttonGrotesque));
        }

        private void OnLoad()
        {
            SkinDef targetSkin = null;
            SkinDef targetSkin2 = null;
            SkinDef[] skins = SkinCatalog.FindSkinsForBody(BodyCatalog.FindBodyIndex("MercBody"));
            foreach (SkinDef skinDef in skins)
            {
                if (skinDef.name == "HiyoriMerc")
                {
                    targetSkin = skinDef;
                }
                else if (skinDef.name == "HiyoriMercNoCase")
                {
                    targetSkin2 = skinDef;
                }
            }

            if (targetSkin)
            {
                VoiceoverInfo voiceoverInfo = new VoiceoverInfo(typeof(HiyoriMercVoiceoverComponent), targetSkin, "MercBody");
                voiceoverInfo.selectActions += HiyoriSelect;
            }
            else
            {
                Debug.LogError("HiyoriMercVoiceover: Hiyori Merc SkinDef not found. Voicelines will not work!");
            }

            if (targetSkin2)
            {
                VoiceoverInfo voiceoverInfo = new VoiceoverInfo(typeof(HiyoriMercVoiceoverComponent), targetSkin2, "MercBody");
                voiceoverInfo.selectActions += HiyoriSelect;
            }
            else
            {
                Debug.LogError("HiyoriMercVoiceover: Hiyori Merc (No Case) SkinDef not found. Voicelines will not work!");
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
            HiyoriMercVoiceoverComponent.nseHurt = RegisterNSE("Play_HiyoriMerc_Hurt");
            HiyoriMercVoiceoverComponent.nseShout = RegisterNSE("Play_HiyoriMerc_Shout");
            HiyoriMercVoiceoverComponent.nseTitle = RegisterNSE("Play_HiyoriMerc_Title");
            HiyoriMercVoiceoverComponent.nseBweh = RegisterNSE("Play_HiyoriMerc_Bweh");
            HiyoriMercVoiceoverComponent.nseItai = RegisterNSE("Play_HiyoriMerc_Itai");

            HiyoriMercVoiceoverComponent.nseEX1 = RegisterNSE("Play_HiyoriMerc_ExSkill_1");
            HiyoriMercVoiceoverComponent.nseEX2 = RegisterNSE("Play_HiyoriMerc_ExSkill_2");
            HiyoriMercVoiceoverComponent.nseEX3 = RegisterNSE("Play_HiyoriMerc_ExSkill_3");

            HiyoriMercVoiceoverComponent.nseEXL1 = RegisterNSE("Play_HiyoriMerc_ExSkill_Level_1");
            HiyoriMercVoiceoverComponent.nseEXL2 = RegisterNSE("Play_HiyoriMerc_ExSkill_Level_2");
            HiyoriMercVoiceoverComponent.nseEXL3 = RegisterNSE("Play_HiyoriMerc_ExSkill_Level_3");

            HiyoriMercVoiceoverComponent.nseOwari1 = RegisterNSE("Play_HiyoriMerc_Owari_1");
            HiyoriMercVoiceoverComponent.nseOwari2 = RegisterNSE("Play_HiyoriMerc_Owari_2");
            HiyoriMercVoiceoverComponent.nseOwari3 = RegisterNSE("Play_HiyoriMerc_Owari_3");

            HiyoriMercVoiceoverComponent.nseCommon = RegisterNSE("Play_HiyoriMerc_CommonSkill");
            HiyoriMercVoiceoverComponent.nseNigashimasen = RegisterNSE("Play_HiyoriMerc_Special");
            HiyoriMercVoiceoverComponent.nseArigatou = RegisterNSE("Play_HiyoriMerc_Arigatou");

            HiyoriMercVoiceoverComponent.nseGrotesque = RegisterNSE("Play_HiyoriMerc_Grotesque");
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
