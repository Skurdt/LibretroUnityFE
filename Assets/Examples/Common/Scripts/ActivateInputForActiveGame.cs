using System.Linq;
using UnityEngine;

namespace SK.Examples.Common
{
    [RequireComponent(typeof(Camera))]
    public class ActivateInputForActiveGame : MonoBehaviour
    {
        [SerializeField] private float _gameInputMaxDistance = 2.4f;

        private Camera _mainCamera;
        private GameModelSetup _currentGame;

        private void Awake()
        {
            _mainCamera = GetComponent<Camera>();
        }

        private void Update()
        {
            Ray ray = _mainCamera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, _gameInputMaxDistance))
            {
                if (hit.transform.gameObject.TryGetComponent(out GameModelSetup hitModelSetup))
                {
                    if (_currentGame != hitModelSetup)
                    {
                        _currentGame = hitModelSetup;
                        _currentGame.ActivateInput();

                        GameModelSetup[] modelSetups = FindObjectsOfType<GameModelSetup>().Where(x => x != _currentGame).ToArray();
                        for (int i = 0; i < modelSetups.Length; ++i)
                        {
                            modelSetups[i].DeactivateInput();
                        }
                    }
                }
            }
            else
            {
                if (_currentGame != null)
                {
                    _currentGame.DeactivateInput();
                    _currentGame = null;
                }
            }
        }
    }
}
