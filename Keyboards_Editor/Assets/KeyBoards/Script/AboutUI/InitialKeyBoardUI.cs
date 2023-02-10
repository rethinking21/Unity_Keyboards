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
            public GameObject obj;
            public bool specialKey = false;
        }

        [Header("initial setting")]
        [SerializeField]
        GameObject keyCap;
        [SerializeField]
        bool createNewKeyButton = false;
        [SerializeField]
        bool createButtonEvent = false;

        [Space, Header("Keyboard Setting")]
        [SerializeField]
        KeyBoardManager keyBoardManager;
        [SerializeField, Range(20,50)]
        int keySize = 30;
        [SerializeField]
        KeySetting currentKeySetting;
        public List<KeyButton> keyButtons = new();

        bool shift = false;
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
            if (createButtonEvent)
            {
                CreateButtonEvent();
            }
            shift = keyBoardManager.shiftKey;
            ChangeKeySetting(ref keyBoardManager.keyMerge.key, shift);
        }

        private void Update()
        {
            //thinking...
            if(keyBoardManager != null || shift != keyBoardManager.shiftKey)
            {
                shift = keyBoardManager.shiftKey;
                ChangeKeySetting(ref keyBoardManager.keyMerge.key, shift);
            }
        }

        #region Create Key
        public void CreateNewKeyButton()
        {
            KeySetting defaultKeySetting = new EngKeySetting();
            createNewKeyButton = false;
            for (int index = 0; index < defaultKeySetting.keyString.Length; index+=2)
            {
                GameObject newKeyButton = Instantiate(keyCap);
                newKeyButton.SetActive(true);
                newKeyButton.name = "Button " + defaultKeySetting.keyString[index].ToString();
                newKeyButton.transform.parent = transform;

                KeyButton key = new();
                key.name = defaultKeySetting.keyString[index].ToString();
                key.index = index/2;
                key.obj = newKeyButton;

                keyButtons.Add(key);

                RectTransform rect = newKeyButton.GetComponent<RectTransform>();
                ChangeButtonRect(ref rect, index);
            }
        }

        void ChangeButtonRect(ref RectTransform rect, int index)
        {
            Vector3 vec = rect.anchoredPosition3D;

            vec.z = 0;
            if (index / 2 < 10)
            {
                vec.y = 0;
                vec.x = keySize * (index / 2);
            }
            else if (index / 2 < 19)
            {
                vec.y = -keySize;
                vec.x = keySize * (index / 2 - 10);
                vec.x += keySize / 2f;
            }
            else
            {
                vec.y = keySize * -2;
                vec.x = keySize * (index / 2 - 19);
                vec.x += keySize;
            }
            rect.anchoredPosition3D = vec;
            rect.localScale = Vector3.one;
            rect.sizeDelta = Vector3.one * keySize;
        }

        #endregion

        public void ChangeKeySetting(ref KeySetting keySetting, bool shift = false)
        {
            foreach(var key in keyButtons)
            {
                if (!key.specialKey)
                {
                    //thinking
                    key.obj.transform.GetChild(0).GetComponent<Text>().text =
                        keySetting.keyString[(key.index * 2) + (shift ? 1 : 0)].ToString();
                }
            }
        }

        public void CreateButtonEvent()
        {
            foreach(var key in keyButtons)
            {
                key.obj.GetComponent<Button>().onClick.RemoveAllListeners();
                key.obj.GetComponent<Button>().onClick.AddListener(() => keyBoardManager.AddKey(key.index));
            }
        }
    }

}
