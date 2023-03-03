using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Keyboard
{
    public class KeySetting
    {
        public string keyString = null;

        public string space = "space";
        public string num = "0123456789";
        public string nextNum = "  "; // -, =
        public string language = "ln";

        public virtual void ChangeMode()
        {

        }
    }

    public class KeyMerge
    {
        public string text = "";
        public KeySetting key;
        public bool shift = false;
        public bool UIChanged = true;

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
