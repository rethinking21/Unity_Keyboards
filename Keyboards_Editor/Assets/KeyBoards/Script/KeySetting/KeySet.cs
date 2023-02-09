using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Keyboard
{
    public class KeySetting
    {
        public string keyString = null;

    }

    public class EngKeySetting : KeySetting
    {
        public EngKeySetting()
        {
            keyString =
                "qQwWeErRtTyYuUiIoOpP"
                + "aAsSdDfFgGhHjJkKlL"
                + "zZxXcCvVbBnNmM"; //52
        }
    }
}
