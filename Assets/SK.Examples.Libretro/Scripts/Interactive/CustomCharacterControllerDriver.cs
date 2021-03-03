using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public sealed class CustomCharacterControllerDriver : CharacterControllerDriver
{
    [SerializeField] private CameraTriggerObject _camera;

    private void Update()
    {
        if (_camera.HeadSync)
            UpdateCharacterController();
    }
}
