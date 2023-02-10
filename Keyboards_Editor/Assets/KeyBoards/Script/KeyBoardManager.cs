using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Keyboard
{
    public class KeyBoardManager : MonoBehaviour
    {
        public string text;
        public KeyMerge keyMerge = new KorKeyMerge();
        public bool shiftKey = false;
        [SerializeField]
        Text textUI;

        // Start is called before the first frame update
        void Start()
        {
            shiftKey = false;
            text = "";
        }

        public void AddKey(int keyIndex)
        {
            if (keyMerge == null) return;
            if (keyIndex >= 0)
            {
                keyMerge.AddKey(keyIndex * 2 + (shiftKey ? 1 : 0));
            }
            else if (keyIndex == -2) // shift
            {
                shiftKey = !shiftKey;
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

            text = keyMerge.GetString();
            if (textUI != null)
            {
                textUI.text = text;
            }
        }
    }
}
