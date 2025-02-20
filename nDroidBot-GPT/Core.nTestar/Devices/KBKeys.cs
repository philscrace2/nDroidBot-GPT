using System;
using System.Collections.Generic;

namespace Core.nTestar.Devices
{


    public enum KBKeys
    {
        KEY_FIRST = 2400,
        KEY_LAST = 2402,
        KEY_TYPED = 2400,
        KEY_PRESSED = 2401,
        KEY_RELEASED = 2402,
        KEY_LOCATION_UNKNOWN = 0,
        KEY_LOCATION_STANDARD = 1,
        KEY_LOCATION_LEFT = 2,
        KEY_LOCATION_RIGHT = 3,
        KEY_LOCATION_NUMPAD = 4,
        VK_ESCAPE = 27,
        VK_F1 = 112,
        VK_F2 = 113,
        VK_F3 = 114,
        VK_F4 = 115,
        VK_F5 = 116,
        VK_F6 = 117,
        VK_F7 = 118,
        VK_F8 = 119,
        VK_F9 = 120,
        VK_F10 = 121,
        VK_F11 = 122,
        VK_F12 = 123,
        VK_BACKSPACE = 8,
        VK_TAB = 9,
        VK_CAPS_LOCK = 20,
        VK_A = 65,
        VK_B = 66,
        VK_C = 67,
        VK_D = 68,
        VK_E = 69,
        VK_F = 70,
        VK_G = 71,
        VK_H = 72,
        VK_I = 73,
        VK_J = 74,
        VK_K = 75,
        VK_L = 76,
        VK_M = 77,
        VK_N = 78,
        VK_O = 79,
        VK_P = 80,
        VK_Q = 81,
        VK_R = 82,
        VK_S = 83,
        VK_T = 84,
        VK_U = 85,
        VK_V = 86,
        VK_W = 87,
        VK_X = 88,
        VK_Y = 89,
        VK_Z = 90,
        VK_0 = 0,
        VK_SPACE = 32,
        VK_SHIFT = 16,
        VK_CONTROL = 17,
        VK_ALT = 18,
        VK_LEFT = 37,
        VK_UP = 38,
        VK_RIGHT = 39,
        VK_DOWN = 40,
        VK_NUMPAD0 = 96,
        VK_NUMPAD1 = 97,
        VK_NUMPAD2 = 98,
        VK_NUMPAD3 = 99,
        VK_NUMPAD4 = 100,
        VK_NUMPAD5 = 101,
        VK_NUMPAD6 = 102,
        VK_NUMPAD7 = 103,
        VK_NUMPAD8 = 104,
        VK_NUMPAD9 = 105
    }

    public static class KBKeysExtensions
    {
        public static bool Contains(string key)
        {
            return Enum.TryParse(typeof(KBKeys), key, out _);
        }
    }

}
