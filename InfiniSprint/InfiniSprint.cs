using System.Reflection;

namespace RoR2 {
    internal class patch_PlayerCharacterMasterController : PlayerCharacterMasterController {
        private FieldInfo _bodyInputsField;

        private extern void orig_Awake();

        private void Awake() {
            orig_Awake();

            _bodyInputsField = typeof(PlayerCharacterMasterController).GetField("bodyInputs",
                BindingFlags.Instance | BindingFlags.NonPublic);
        }

        private extern void orig_FixedUpdate();

        private void FixedUpdate() {
            orig_FixedUpdate();

            var player = NetworkUser.readOnlyLocalPlayersList[0];
            var bodyInputs = (InputBankTest) _bodyInputsField.GetValue(this);

            bodyInputs.sprint.PushState(!player.inputPlayer.GetButton("Sprint"));
        }
    }
}