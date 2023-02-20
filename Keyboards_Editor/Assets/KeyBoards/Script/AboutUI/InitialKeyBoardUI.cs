using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Keyboard
{
    public class InitialKeyBoardUI : MonoBehaviour
    {

        #region declaration
        [Serializable]
        public class KeyButton
        {
            public string name; // char? string?
            public int index;
            public Text text;
            public bool specialKey = false;

            [Space, Tooltip("key 'h' is position (0, 0), X=0 : 6, y, h, n")]
            public Vector2 UIPosition = Vector2.zero;
            public Vector2 UIScale = Vector2.one;
        }

        [Header("initial setting")]
        [SerializeField]
        GameObject keyCap;
        [SerializeField]
        bool createNewKeyButton = false;

        [Space, Header("Keyboard Setting")]
        [SerializeField, Range(20,50)]
        int keySize = 30;
        [SerializeField]
        KeySetting currentKeySetting;
        public List<KeyButton> keyButtons = new List<KeyButton>();
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            if (createNewKeyButton)
            {
                currentKeySetting = new EngKeySetting();
                CreateNewKeyButton();
                ChangeKeySetting(ref currentKeySetting);
            }
        }

        public void ChangeKeyUI(ref KeySetting key, bool shift)
        {
            ChangeKeySetting(ref key, shift);
        }

        #region Create Key
        public void CreateNewKeyButton()
        {
            KeySetting defaultKeySetting = new EngKeySetting();
            createNewKeyButton = false;

            foreach (KeyButton button in keyButtons)
            {
                GameObject newKeyButton = Instantiate(keyCap);
                newKeyButton.SetActive(true);
                //need to fix
                if(button.index >=0 && button.index <= 25)
                {
                    newKeyButton.name = "Button " + defaultKeySetting.keyString[button.index * 2].ToString();
                }
                else
                {
                    newKeyButton.name = "Button " + button.name;
                }
                newKeyButton.transform.SetParent(transform);

                RectTransform rect = newKeyButton.GetComponent<RectTransform>();
                ChangeButtonRect(ref rect, button);
            }
        }

        void ChangeButtonRect(ref RectTransform rect, KeyButton button)
        {
            Vector3 vec = rect.anchoredPosition3D;

            vec.z = 0;
            vec.y = keySize * button.UIPosition.y;
            vec.x = keySize * (button.UIPosition.x + -0.5f * button.UIPosition.y);
            rect.anchoredPosition3D = vec;
            rect.localScale = Vector3.one;
            rect.sizeDelta = button.UIScale * keySize;
        }
        #endregion

        public void ChangeKeySetting(ref KeySetting keySetting, bool shift = false)
        {
            foreach(var key in keyButtons)
            {
                if (!key.specialKey)
                {
                    //about name
                    if (key.index >= 0 && key.index <= 25)
                    {
                        //thinking
                        key.text.text = keySetting.keyString[(key.index * 2) + (shift ? 1 : 0)].ToString();

                    }
                    else if (key.index >= 100)
                    {
                        key.text.text = keySetting.num[key.index - 100].ToString();
                    }
                    else
                    {
                        key.text.text = key.name;
                    }
                }
                else if (key.index == -4)
                {
                    key.text.text = keySetting.space;
                }
                else
                {
                    key.text.text = key.name;
                }
            }
        }

    }

}
