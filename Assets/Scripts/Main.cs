using SK.Utilities;
using UnityEngine;

namespace SK
{
    public class Main : MonoBehaviour
    {
        public Emulation.GameList GameList { get; private set; }

        private Model.GameModelSetup _gameModelSetup;

        private void Awake()
        {
            _gameModelSetup = FindObjectOfType<Model.GameModelSetup>();
            if (_gameModelSetup != null)
            {
                GameList = FileSystem.DeserializeFromJson<Emulation.GameList>("@Data~/Games.json");
                if (GameList != null && GameList.Games.Length > 0)
                {
                    _gameModelSetup.SetGame(GameList.Games[0]);
                }
            }
        }

        private void Start()
        {
            if (_gameModelSetup != null)
            {
                _gameModelSetup.StartGame();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_gameModelSetup != null)
                {
                    _gameModelSetup.StopGame();
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#endif
                }
            }
        }
    }
}
