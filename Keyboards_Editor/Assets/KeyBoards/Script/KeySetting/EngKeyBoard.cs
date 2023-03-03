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
            language = "Eng";
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

        public override void AddKey(int keyIndex)
        {
            if (keyIndex >= 0 && key.keyString.Length > keyIndex)
            {
                if(key.keyString.Length > keyIndex)
                {
                    //thinking
                    text += key.keyString[keyIndex];
                }
            }
            else if (keyIndex >= 100 && keyIndex < 110)
            {
                text += (keyIndex - 100).ToString();
            }
            else if(keyIndex == -3 && text.Length != 0) // del
            {
                text = text.Remove(text.Length - 1);
            }
            else if(keyIndex == -2) //shift
            {
                shift = !shift;
                UIChanged = true;
            }
            else if (keyIndex == -4) //space
            {
                text += " ";
            }
        }
    }
}

