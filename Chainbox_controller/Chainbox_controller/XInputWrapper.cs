using System;
using System.Runtime.InteropServices;

namespace Chainbox_controller
{
    // Minimal XInput wrapper for one gamepad.
    public class XInputWrapper
    {
        [StructLayout(LayoutKind.Sequential)]
        public class GamepadState
        {
            public bool connected;
            public short leftThumbX;
            public short leftThumbY;
            public short rightThumbX;
            public short rightThumbY;
            public bool leftShoulder;
            public bool rightShoulder;
        }

        [DllImport("xinput1_4.dll")]
        private static extern int XInputGetState(int dwUserIndex, out XINPUT_STATE pState);

        [StructLayout(LayoutKind.Sequential)]
        private struct XINPUT_STATE
        {
            public uint dwPacketNumber;
            public XINPUT_GAMEPAD Gamepad;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct XINPUT_GAMEPAD
        {
            public ushort wButtons;
            public byte bLeftTrigger;
            public byte bRightTrigger;
            public short sThumbLX;
            public short sThumbLY;
            public short sThumbRX;
            public short sThumbRY;
        }

        public static GamepadState GetState(int index)
        {
            XINPUT_STATE xs;
            int r = XInputGetState(index, out xs);
            if (r != 0) return null;
            var g = new GamepadState();
            g.connected = true;
            g.leftThumbX = xs.Gamepad.sThumbLX;
            g.leftThumbY = xs.Gamepad.sThumbLY;
            g.rightThumbX = xs.Gamepad.sThumbRX;
            g.rightThumbY = xs.Gamepad.sThumbRY;
            g.leftShoulder = (xs.Gamepad.wButtons & 0x0100) != 0;
            g.rightShoulder = (xs.Gamepad.wButtons & 0x0200) != 0;
            return g;
        }
    }
}
