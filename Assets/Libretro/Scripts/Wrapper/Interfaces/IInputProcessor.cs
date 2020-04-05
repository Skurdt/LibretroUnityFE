namespace SK.Libretro
{
    public interface IInputProcessor
    {
        bool JoypadButton(int port, int button);

        float MouseDelta(int port, int axis);
        float MouseWheelDelta(int port, int axis);
        bool MouseButton(int port, int button);
    }
}
