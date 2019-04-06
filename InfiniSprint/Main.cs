using System.Reflection;
using Harmony;
using RoR2;
using UnityModManagerNet;

namespace InfiniSprint {
    internal static class Main {
        private static bool _enabled;

        private static bool Load(UnityModManager.ModEntry modEntry) {
            modEntry.OnToggle = OnToggle;

            var harmony = HarmonyInstance.Create("dev.eliaswatson.infinisprint");
            if (harmony != null) {
                harmony.PatchAll();
            }
            else {
                modEntry.Logger.Error("Failed to initialize Harmony");
                return false;
            }

            return true;
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value) {
            _enabled = value;
            return true;
        }

        [HarmonyPatch(typeof(PlayerCharacterMasterController))]
        [HarmonyPatch("FixedUpdate")]
        private static class SprintPatch {
            private static FieldInfo _bodyInputsField;

            private static bool Prepare() {
                _bodyInputsField = typeof(PlayerCharacterMasterController).GetField("bodyInputs",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                return _bodyInputsField != null;
            }

            private static void Postfix(PlayerCharacterMasterController __instance) {
                if (!_enabled) return;

                var player = NetworkUser.readOnlyLocalPlayersList[0];
                var bodyInputs = (InputBankTest) _bodyInputsField.GetValue(__instance);

                bodyInputs.sprint.PushState(!player.inputPlayer.GetButton("Sprint"));
            }
        }
    }
}