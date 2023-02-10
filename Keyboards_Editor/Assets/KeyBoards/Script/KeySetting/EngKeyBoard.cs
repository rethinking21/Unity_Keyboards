using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Keyboard
{
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

    public class EngKeyMerge : KeyMerge
    {
        public EngKeyMerge()
        {
            key = new EngKeySetting();
        }

        public EngKeyMerge(EngKeySetting engKey)
        {
            key = engKey;
        }

        public virtual void Addkey(int keyIndex)
        {
            if (keyIndex >= 0)
            {
                if(key.keyString.Length > keyIndex)
                {
                    //thinking
                    text += key.keyString[keyIndex];
                }
            }
            else if(keyIndex == -3 || text.Length != 0) // del
            {
                text = text.Remove(text.Length - 1);
            }
        }
    }
}

