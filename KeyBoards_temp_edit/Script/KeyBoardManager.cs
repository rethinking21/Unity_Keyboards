using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



namespace Keyboard
{
    public class KeyBoardManager : MonoBehaviour
    {

        public UIKeyboardNameSetter keyboardNameSetter;
        
        public enum TYPE
        {
            ENG = 0,
            KOR,
            JPN,
            CHN,
            CHNT // Traditional
        }

        public string text;
        public KeyMerge keyMerge; // change it(wip)
        [SerializeField]
        InitialKeyBoardUI KeyBoardUI;
        [SerializeField]
        Text textUI;
        [SerializeField]
        TYPE[] keyboardType = { TYPE.ENG, TYPE.KOR, TYPE.JPN, TYPE.CHN, TYPE.CHNT };

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

            switch (GameManager.Instance.playerPrefController.GetLanguage())
            {
                case GameTextTypes.Language.kr:
                    ManualChangeKeyboard(TYPE.KOR);
                    break;
                case GameTextTypes.Language.ja:
                    ManualChangeKeyboard(TYPE.JPN);
                    break;
                case GameTextTypes.Language.zh_CHS:
                case GameTextTypes.Language.zh_CHT:
                    ManualChangeKeyboard(TYPE.CHN);
                    break;

                    // TODO : ADD CHNT(Traditional) type
                    // and add code ManualChangeKeyboard(TYPE.CHNT);
            }
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
                    if (keyMerge.shift)
                    {
                        keyMerge.shift = false;
                        keyMerge.UIChanged = true;
                    }
                }
                else if (keyIndex >= 100 && keyIndex < 112) // number
                {
                    keyMerge.AddKey(keyIndex);
                    keyMerge.shift = false;
                    keyMerge.UIChanged = true;
                }
            }
            //special key
            else if (keyIndex == -2) // shift
            {
                keyMerge.AddKey(keyIndex);
            }
            else if (keyIndex == -1) // enter(test)
            {
                if (textUI && textUI.text.Length > 0)
                    keyboardNameSetter.CB_SetName();
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
            else if (type == TYPE.CHN) keyboardTypes.Add(new ChnKeyMerge());
            else if (type == TYPE.CHNT) keyboardTypes.Add(new ChnTraditionalKeyMerge());
        }

        void ChangeNextType()
        {
            int nextIndex = (currentTypeIndex + 1) % keyboardTypes.Count;
            
// // TEMP: STEAM NEXT FEST TODO: remove once other languages are implemented
//             if (nextIndex == 2)
//             {
//                 nextIndex = 0;
//             }
            
            keyboardTypes[nextIndex].ClearAll();
            keyboardTypes[nextIndex].text = keyMerge.GetString();
            keyMerge = keyboardTypes[nextIndex];
            keyboardTypes[currentTypeIndex].ClearAll();
            currentTypeIndex = nextIndex;
            keyMerge.shift = false; // thinking
        }

        public void ManualChangeKeyboard(TYPE keyboardType)
        {
            switch (keyboardType)
            {
                case TYPE.ENG:
                    break;
                
                case TYPE.KOR:
                    ChangeNextType();
                    break;
                
                case TYPE.JPN:
                    ChangeNextType();
                    ChangeNextType();
                    break;

                case TYPE.CHN:
                    ChangeNextType();
                    ChangeNextType();
                    ChangeNextType();
                    break;

                case TYPE.CHNT:
                    ChangeNextType();
                    ChangeNextType();
                    ChangeNextType();
                    break;
            }
            
            keyMerge.UIChanged = true;
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
    }
}
