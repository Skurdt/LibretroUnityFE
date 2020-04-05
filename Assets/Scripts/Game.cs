using UnityEngine;

namespace SK
{
    [CreateAssetMenu(fileName = "New Game", menuName = "Libretro/Game")]
    public class Game : ScriptableObject
    {
        public string Core;
        public string Directory;
        public string Name;
    }
}
