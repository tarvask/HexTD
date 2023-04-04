using HexSystem;

namespace InputSystem
{
    public interface IPointerInputListener
    {
        void LmbClickHandle(Hex2d hex);
        void RmbClickHandle(Hex2d hex);
    }
}