using HarmonyLib;
using Reactor.API.Attributes;
using Reactor.API.Interfaces.Systems;
using Reactor.API.Logging;
using Reactor.API.Runtime.Patching;
using UnityEngine;

namespace Distance.PlaySolo
{
    /// <summary>
    ///     The mod's main class containing its entry point
    /// </summary>
    [ModEntryPoint("Distance.Blu3.PlaySolo")]
    public sealed class Mod : MonoBehaviour
    {
        public static Mod Instance { get; private set; }

        public IManager Manager { get; private set; }

        public Log Logger { get; private set; }

        /// <summary>
        ///     Method called as soon as the mod is loaded.
        ///     WARNING:	Do not load asset bundles/textures in this function
        ///     The unity assets systems are not yet loaded when this
        ///     function is called. Loading assets here can lead to
        ///     unpredictable behaviour and crashes!
        /// </summary>
        public void Initialize(IManager manager)
        {
            // Do not destroy the current game object when loading a new scene
            DontDestroyOnLoad(this);

            Instance = this;

            Manager = manager;

            // Create a log file
            Logger = LogManager.GetForCurrentAssembly();

            RuntimePatcher.AutoPatch();
        }

        /// <summary>
        ///     Method called after
        ///     <c>GameManager.Start()</c>
        ///     This initialisation method is the same as
        ///     the Spectrum mod loader initialisation procedure.
        /// </summary>
        public void LateInitialize(IManager manager)
        {
            // Code here...

            var harmony = new Harmony("Blu3.PlaySolo.Patcher");
            harmony.PatchAll();

            // Disable all ghosts on launch.
            G.Sys.optionsManager_.Replay_.GhostsInArcadeType_ = ReplaySettings.GhostsInArcade.None;

            Initialize(manager);
        }
    }

    [HarmonyPatch(typeof(FinishMenuLogic), nameof(FinishMenuLogic.ShowMatchResults))]
    internal class LobbyLogicPatcher
    {
        /// <summary>
        ///     Remove the option to show default match results and override the method call with a call to the local leader board.
        /// </summary>
        /// <param name="__instance"></param>
        /// <returns></returns>
        [HarmonyPrefix]
        private static bool PrefixPatch(FinishMenuLogic __instance)
        {
            __instance.ShowLocalLeaderboards();
            return false;
        }
    }
}
