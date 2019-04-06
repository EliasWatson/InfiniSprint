using Harmony;
using RoR2;
using UnityModManagerNet;
using System.Reflection;

namespace InfiniSprint
{
    static class Main
    {
        public static bool enabled;
        public static UnityModManager.ModEntry mod;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            mod = modEntry;
            modEntry.OnToggle = OnToggle;

            var harmony = HarmonyInstance.Create("dev.eliaswatson.infinisprint");
            if (harmony != null) harmony.PatchAll();
            else { mod.Logger.Error("Failed to initialize Harmony"); return false; }

            return true;
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;
            return true;
        }

        [HarmonyPatch(typeof(PlayerCharacterMasterController))]
        [HarmonyPatch("FixedUpdate")]
        static class SprintPatch
        {
            private static FieldInfo bodyInputsField;

            static bool Prepare()
            {
                bodyInputsField = typeof(PlayerCharacterMasterController).GetField("bodyInputs", BindingFlags.Instance | BindingFlags.NonPublic);

                return (bodyInputsField != null);
            }

            static void Postfix(PlayerCharacterMasterController __instance)
            {
                if (!Main.enabled) return;

                var bodyInputs = (InputBankTest)bodyInputsField.GetValue(__instance);
                bodyInputs.sprint.PushState(true);
            }
        }
    }
}
