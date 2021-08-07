using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SK.Examples
{
    public sealed class CustomCharacterControllerDriver : CharacterControllerDriver
    {
        [SerializeField] private CameraTriggerObject _camera;

        private void Update()
        {
            if (_camera.HeadSync)
                UpdateCharacterController();
        }
    }
}
