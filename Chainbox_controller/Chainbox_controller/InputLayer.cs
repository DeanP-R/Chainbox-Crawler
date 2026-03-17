using System;
using System.Runtime.InteropServices;

namespace Chainbox_controller
{
    public class InputState
    {
        public double Forward { get; set; } = 0.0; // -1..1
        public double Turn { get; set; } = 0.0;    // -1..1
        public double Probe { get; set; } = 0.0;   // -1..1
    }

    public class InputLayer
    {
        public enum InputMode
        {
            Automatic,
            Keyboard,
            Gamepad
        }
        // Simple polling input layer: keyboard and XInput (gamepad) combined.

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private XInputWrapper.GamepadState gamepadState = null;
        public bool GamepadConnected { get; private set; } = false;
        public int GamepadIndex { get; private set; } = -1;

        private InputState manualOverride = null;

        public InputLayer()
        {
        }

        public void SetManualOverride(InputState s)
        {
            manualOverride = s;
        }

        public void ClearManualOverride()
        {
            manualOverride = null;
        }

        // Respect input mode: Automatic, Keyboard, Gamepad
        public InputState Update(InputMode mode = InputMode.Automatic)
        {
            var s = new InputState();
            // Keyboard controls (W/A/S/D for forward/turn, space for stop, left/right arrows for probe)
            bool w = false, sKey = false, a = false, d = false, space = false, left = false, right = false;
            if (mode != InputMode.Gamepad)
            {
                w = (GetAsyncKeyState((int)Keys.W) & 0x8000) != 0;
                sKey = (GetAsyncKeyState((int)Keys.S) & 0x8000) != 0;
                a = (GetAsyncKeyState((int)Keys.A) & 0x8000) != 0;
                d = (GetAsyncKeyState((int)Keys.D) & 0x8000) != 0;
                space = (GetAsyncKeyState((int)Keys.Space) & 0x8000) != 0;
                left = (GetAsyncKeyState((int)Keys.Left) & 0x8000) != 0;
                right = (GetAsyncKeyState((int)Keys.Right) & 0x8000) != 0;
            }

            if (space)
            {
                // immediate stop
                s.Forward = 0;
                s.Turn = 0;
                s.Probe = 0;
                return s;
            }

            // Simple digital forward/back
            if (w && !sKey) s.Forward = 1.0;
            else if (sKey && !w) s.Forward = -1.0;
            else s.Forward = 0.0;

            // Digital turning
            if (a && !d) s.Turn = -1.0;
            else if (d && !a) s.Turn = 1.0;
            else s.Turn = 0.0;

            // Probe left/right
            if (left && !right) s.Probe = -1.0;
            else if (right && !left) s.Probe = 1.0;
            else s.Probe = 0.0;

            // If a manual override is set (from on-screen buttons), use it and don't sample devices
            if (manualOverride != null)
            {
                return manualOverride;
            }

            // Scan gamepad slots 0-3 (XInput) and update connection index
            gamepadState = null;
            GamepadIndex = -1;
            // Only probe gamepads if mode is not Keyboard-only
            if (mode != InputMode.Keyboard)
            {
                for (int i = 0; i < 4; i++)
                {
                    try
                    {
                        var st = XInputWrapper.GetState(i);
                        if (st != null && st.connected)
                        {
                            gamepadState = st;
                            GamepadIndex = i;
                            break;
                        }
                    }
                    catch
                    {
                        // ignore any exceptions from XInput probing
                    }
                }
            }

            GamepadConnected = gamepadState != null && gamepadState.connected;

            if (gamepadState != null && gamepadState.connected && mode != InputMode.Keyboard)
            {
                // left thumb Y for forward, right thumb X for turn
                double f = gamepadState.leftThumbY / 32767.0;
                double t = gamepadState.rightThumbX / 32767.0;
                // dead zone
                if (Math.Abs(f) < 0.1) f = 0;
                if (Math.Abs(t) < 0.1) t = 0;

                // Use analog when non-zero - combine by taking max magnitude
                if (Math.Abs(f) > Math.Abs(s.Forward)) s.Forward = f;
                if (Math.Abs(t) > Math.Abs(s.Turn)) s.Turn = t;

                // Right shoulder buttons control probe
                if (gamepadState.rightShoulder) s.Probe = 1.0;
                else if (gamepadState.leftShoulder) s.Probe = -1.0;
            }

            return s;
        }
    }

    // Small enum copy for key codes to avoid taking System.Windows.Forms in this file
    internal enum Keys : int
    {
        W = 0x57,
        A = 0x41,
        S = 0x53,
        D = 0x44,
        Space = 0x20,
        Left = 0x25,
        Right = 0x27
    }
}
