using BepInEx;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Utilla;

namespace BananaPlatforms
{
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        bool inRoom;
        private GameObject LBanana;
        private GameObject RBanana;
        public bool LGrip;
        public bool RGrip;
        private bool WasRPressed;
        private bool WasLPressed;
        public Vector3 LHand;
        public Vector3 RHand;
        public bool LButton;
        public bool RButton;
        public bool LButton2;
        public bool RButton2;

        void Start()
        {
            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        void OnEnable()
        {
            HarmonyPatches.ApplyHarmonyPatches();
        }

        void OnDisable()
        {
            HarmonyPatches.RemoveHarmonyPatches();
        }
        public AssetBundle LoadAssetBundle(string path)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            AssetBundle bundle = AssetBundle.LoadFromStream(stream);
            stream.Close();
            return bundle;
        }
        void OnGameInitialized(object sender, EventArgs e)
        {
                var bundle = LoadAssetBundle("BananaPlatforms.BananaBundle.bananabundle");
                var asset = bundle.LoadAsset<GameObject>("bananaplat.prefab");
            RBanana = Instantiate(asset);
            RBanana.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            RBanana.transform.position = new Vector3(0f, 0f, 0f);
            RBanana.SetActive(false);

            LBanana = Instantiate(asset);
            LBanana.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            LBanana.transform.position = new Vector3(0f, 0f, 0f);
            RBanana.SetActive(false);
        }

        void Update()
        {
            if (RBanana == null)
            {
                return;
            }
            if (LBanana == null)
            {
                return;
            }
            if (!inRoom)
            {
                RBanana.SetActive(false);
                LBanana.SetActive(false);
                return;
            }
            LHand = GorillaLocomotion.Player.Instance.leftControllerTransform.position;
            RHand = GorillaLocomotion.Player.Instance.rightControllerTransform.position;
            LGrip = ControllerInputPoller.instance.leftGrab;
            RGrip = ControllerInputPoller.instance.rightGrab;
            RButton = ControllerInputPoller.instance.rightControllerPrimaryButton;
            LButton = ControllerInputPoller.instance.leftControllerPrimaryButton;
            RButton2 = ControllerInputPoller.instance.rightControllerSecondaryButton;
            LButton2 = ControllerInputPoller.instance.leftControllerSecondaryButton;
            if (inRoom)
            {
                if (!WasRPressed && RGrip) { Appear(RBanana, RHand); } else if (!RGrip) { Disappear(RBanana); }
                if (!WasLPressed && LGrip) { Appear(LBanana, LHand); } else if (!LGrip) { Disappear(LBanana); }
                if (RButton && RGrip) { RBanana.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f); }
                if (LButton && LGrip) { LBanana.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f); }
                if (RButton2 && RGrip) { RBanana.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f); }
                if (LButton2 && LGrip) { LBanana.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f); }
            }
            WasLPressed = LGrip;
            WasRPressed = RGrip;
        }
        void Appear(GameObject banana, Vector3 position)
        {
            banana.SetActive(true);
            banana.transform.position = position;
        }
        void Disappear(GameObject bananabegone)
        {
            bananabegone.transform.position = new Vector3 (0f, 0f, 0f);
            bananabegone.SetActive(false);
        }
        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            inRoom = true;
        }

        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            inRoom = false;
            LBanana.SetActive(false);
            RBanana.SetActive(false);
        }
    }
}
