using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Keyboard
{
    public class KeySetting
    {
        public string keyString = null;

    }

    public class KeyMerge
    {
        public string text = "";
        public KeySetting key;

        public virtual void AddKey(int keyIndex)
        {

        }

        public virtual string GetString()
        {
            return text;
        }

        public virtual void ClearAll()
        {
            text = "";
        }

        public virtual void PushTempChar()
        {

        }

        public virtual bool CanDeleteText()
        {
            return true;
        }
    }

}
