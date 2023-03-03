using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Keyboard
{
    public class ChnKeySetting : KeySetting
    {
        public ChnKeySetting()
        {
            keyString =
                "qQwWeErRtTyYuUiIoOpP"
                + "aAsSdDfFgGhHjJkKlL"
                + "zZxXcCüÜbBnNmM"; //52
            num = "0123456789";
            language = "Chn";
            nextNum = "<>";
        }
    }

    public class ChnKeyMerge : KeyMerge
    {
        string waitString = "";

        ChnPinyinDatas pinyinData = null;
        int pinyinIndex = 0;

        public ChnKeyMerge()
        {
            key = new ChnKeySetting();
            PinyinDataLoad();
        }

        public ChnKeyMerge(ChnKeySetting engKey)
        {
            key = engKey;
            PinyinDataLoad();
        }

        public override void AddKey(int keyIndex)
        {
            if (keyIndex >= 0 && key.keyString.Length > keyIndex)
            {
                if (keyIndex % 2 == 1)
                {
                    PushTempChar();
                    text += key.keyString[keyIndex];
                }
                else
                {
                    waitString += key.keyString[keyIndex];
                    if (waitString.Length < 2)
                    {
                        UpdateSearchPinyin(ref waitString);
                    }
                    else
                    {
                        UpdateAddCharPinyin(ref waitString);
                    }
                    pinyinIndex = 0;
                }
            }
            else if (keyIndex >= 100 && keyIndex < 110)
            {
                if ("0123456789".Contains(key.num[keyIndex - 100]))
                {
                    PushTempChar();
                    text += (keyIndex - 100).ToString();
                }
                else
                {
                    waitString = key.num[keyIndex - 100].ToString();
                    pinyinData.pinyinTempList.Clear();
                    PushTempChar();
                    key.num = "0123456789";
                }
            }
            else if (keyIndex >= 110 && keyIndex < 112)
            {
                if(keyIndex == 110)
                {
                    pinyinIndex--;
                    if (pinyinIndex < 0) pinyinIndex = 0;
                }
                else if (pinyinData.pinyinTempList.Count > (pinyinIndex * 5))
                {
                    pinyinIndex++;
                }
                key.num = ShowPinyinNum(pinyinIndex);
                UIChanged = true;
            }
            else if (keyIndex == -3) // del
            {
                if (waitString.Length != 0)
                {
                    waitString = waitString.Remove(waitString.Length - 1);
                    if (waitString.Length != 0)
                    {
                        UpdateSearchPinyin(ref waitString);
                    }
                    else
                    {
                        key.num = "0123456789";
                        pinyinData.pinyinTempList.Clear();
                        UIChanged = true;
                    }
                }
                pinyinIndex = 0;
            }
            else if (keyIndex == -2) //shift
            {
                shift = !shift;
                UIChanged = true;
            }
            else if (keyIndex == -4) //space
            {
                //not this
                PushTempChar();
                text += " ";
                key.num = "0123456789";
                UIChanged = true;
            }

        }

        public override string GetString()
        {
            return text + waitString;
        }

        public override void ClearAll()
        {
            text = "";
            waitString = "";
        }

        public override void PushTempChar()
        {
            text += waitString;
            waitString = "";
        }

        public override bool CanDeleteText()
        {
            return waitString.Length == 0;
        }

        void PinyinDataLoad()
        {
            pinyinData = new ChnPinyinDatas();
            pinyinData.LoadJsonToClass();
        }

        void UpdateSearchPinyin(ref string engText)
        {
            pinyinData.UpdateSearch(engText);
            key.num = ShowPinyinNum(0);
            UIChanged = true;
        }

        void UpdateAddCharPinyin(ref string engText)
        {
            pinyinData.UpdateAddChar(engText);
            key.num = ShowPinyinNum(0);
            UIChanged = true;
        }

        string ShowPinyinNum(int numIndex)
        {
            string result = "01234";
            for (int index = 0; index < 5; index++)
            {
                if (pinyinData.pinyinTempList.Count > (index + numIndex * 5))
                {
                    result += pinyinData.pinyinTempList[index + numIndex * 5].literal;
                }
                else
                {
                    result += (index + 5).ToString();
                }
            }
            return result;
        }
    }
}

