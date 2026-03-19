using System;
using System.Runtime.InteropServices;

namespace Chainbox_controller
{
    public class InputLayer
    {
        public enum InputMode
        {
            //Automatic,
            Gamepad,
            Keyboard
        }

        public bool GamepadConnected { get; private set; }
        public int GamepadIndex { get; private set; } = -1;

        private InputState _manualOverride = new InputState();
        private bool _hasManualOverride = false;

        public void SetManualOverride(InputState state)
        {
            _manualOverride = state;
            _hasManualOverride = true;
        }

        public void ClearManualOverride()
        {
            _manualOverride = new InputState();
            _hasManualOverride = false;
        }

        public InputState Update(InputMode mode)
        {
            if (_hasManualOverride)
                return _manualOverride;

            var gamepadState = PollGamepad();

            bool useGamepad =
                mode == InputMode.Gamepad;

            if (useGamepad && gamepadState != null)
                return gamepadState;

            return new InputState();
        }

        private InputState? PollGamepad()
        {
            for (int i = 0; i < 4; i++)
            {
                if (XInputGetState(i, out XINPUT_STATE state) == 0)
                {
                    GamepadConnected = true;
                    GamepadIndex = i;
                    return ParseGamepad(state.Gamepad);
                }
            }

            GamepadConnected = false;
            GamepadIndex = -1;
            return null;
        }

        private InputState ParseGamepad(XINPUT_GAMEPAD gp)
        {
            const short deadzone = 8000;

            double forward = ScaleStick(gp.sThumbLY, deadzone);
            double turn = ScaleStick(gp.sThumbRX, deadzone);

            double probe;
            if (gp.bRightTrigger > 10 || gp.bLeftTrigger > 10)
            {
                probe = (gp.bRightTrigger - gp.bLeftTrigger) / 255.0;
            }
            else
            {
                probe = ScaleStick(gp.sThumbLX, deadzone);
            }

            return new InputState
            {
                Forward = Clamp(forward),
                Turn = Clamp(turn),
                Probe = Clamp(probe)
            };
        }

        private static double ScaleStick(short value, short deadzone)
        {
            if (Math.Abs(value) < deadzone)
                return 0.0;

            double sign = value > 0 ? 1.0 : -1.0;
            double range = 32767.0 - deadzone;

            return sign * (Math.Abs(value) - deadzone) / range;
        }

        private static double Clamp(double value)
        {
            if (value > 1.0) return 1.0;
            if (value < -1.0) return -1.0;
            return value;
        }

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

        [DllImport("xinput1_4.dll")]
        private static extern uint XInputGetState(int dwUserIndex, out XINPUT_STATE pState);
    }
}