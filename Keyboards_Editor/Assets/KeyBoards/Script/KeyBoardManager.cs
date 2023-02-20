using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Keyboard
{
    public class KeyBoardManager : MonoBehaviour
    {
        public enum TYPE
        {
            ENG = 0,
            KOR,
            JPN,
        }

        public string text;
        public KeyMerge keyMerge; // change it(wip)
        [SerializeField]
        InitialKeyBoardUI KeyBoardUI;
        [SerializeField]
        Text textUI;
        [SerializeField]
        TYPE[] keyboardType = { TYPE.ENG, TYPE.KOR, TYPE.JPN };

        List<KeyMerge> keyboardTypes = new List<KeyMerge>();
        int currentTypeIndex = 0;

        // Start is called before the first frame update
        void Start()
        {
            text = "";
            foreach (var type in keyboardType)
            {
                CreateType(type);
            }
            keyMerge = keyboardTypes[currentTypeIndex];
            KeyBoardUI.ChangeKeyUI(ref keyMerge.key, keyMerge.shift);
        }

        public void AddKey(int keyIndex)
        {
            #region keyIndex Check
            if (keyMerge == null) return;
            if (keyIndex >= 0)
            {
                if (keyIndex >= 0 && keyIndex <= 25)
                {
                    keyMerge.AddKey(keyIndex * 2 + (keyMerge.shift ? 1 : 0));
                }
                else if (keyIndex >= 100 && keyIndex < 110)
                {
                    keyMerge.AddKey(keyIndex);
                }
            }
            //special key
            else if (keyIndex == -2) // shift
            {
                keyMerge.AddKey(keyIndex);
            }
            else if (keyIndex == -1) // enter(test)
            {
                keyMerge.ClearAll();
            }
            else if (keyIndex == -3) // del
            {
                if (keyMerge.CanDeleteText())
                {
                    if(keyMerge.text.Length != 0)
                        keyMerge.text = keyMerge.text.Remove(keyMerge.text.Length - 1);
                }
                else
                {
                    keyMerge.AddKey(keyIndex);
                }
            }
            else if (keyIndex == -4) // space
            {
                keyMerge.AddKey(keyIndex);
            }
            else if (keyIndex == -5) // language
            {
                ChangeNextType();
                keyMerge.UIChanged = true;
            }

            #endregion

            if (keyMerge.UIChanged)
            {
                KeyBoardUI.ChangeKeyUI(ref keyMerge.key, keyMerge.shift);
                keyMerge.UIChanged = false;
            }

            text = keyMerge.GetString();
            if (textUI != null)
            {
                textUI.text = text;
            }
        }

        void CreateType(TYPE type)
        {
            if (type == TYPE.ENG) keyboardTypes.Add(new EngKeyMerge());
            else if (type == TYPE.KOR) keyboardTypes.Add(new KorKeyMerge());
            else if (type == TYPE.JPN) keyboardTypes.Add(new JpnKeyMerge());
        }

        void ChangeNextType()
        {
            int nextIndex = (currentTypeIndex + 1) % keyboardTypes.Count;
            keyboardTypes[nextIndex].ClearAll();
            keyboardTypes[nextIndex].text = keyMerge.GetString();
            keyMerge = keyboardTypes[nextIndex];
            keyboardTypes[currentTypeIndex].ClearAll();
            currentTypeIndex = nextIndex;
            keyMerge.shift = false; // thinking
        }
    }
}
