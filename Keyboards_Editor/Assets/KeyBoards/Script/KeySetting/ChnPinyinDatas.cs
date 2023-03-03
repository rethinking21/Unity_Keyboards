using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Keyboard
{
    public class ChnPinyinDatas
    {
        [Serializable]
        public class PinyinInfo
        {
            public string literal;
            public int freq;
            public string[] eng;
        }

        [Serializable]
        public class Pinyin
        {
            public PinyinInfo[] list;
        }

        // worry about path..
        string jsonPath = "Keyboards/Data/Chn/pinyin_sorted";
        TextAsset textAsset = null;
        Pinyin pinyin = null;
        public List<PinyinInfo> pinyinTempList = new List<PinyinInfo>();

        #region declartion
        public ChnPinyinDatas()
        {
            if (textAsset == null && jsonPath != null)
            {
                textAsset = Resources.Load<TextAsset>(jsonPath);
            }
        }

        public ChnPinyinDatas(string path)
        {
            textAsset = Resources.Load<TextAsset>(path);
            jsonPath = path;
        }

        public ChnPinyinDatas(TextAsset asset)
        {
            textAsset = asset;
        }
        #endregion

        public void LoadJsonToClass()
        {
            if (textAsset == null) return;
            pinyin = JsonUtility.FromJson<Pinyin>(textAsset.text);
            textAsset = null;
        }

        public void UpdateSearch(string searchText)
        {
            StringBuilder result = new StringBuilder();
            pinyinTempList.Clear();
            foreach (var onePinyin in pinyin.list)
            {
                //thinking
                bool isIncorrect = true;
                for (int engIndex = 0; isIncorrect && engIndex < onePinyin.eng.Length; engIndex++)
                {
                    if (onePinyin.eng[engIndex].StartsWith(searchText))
                    {
                        isIncorrect = false;
                        pinyinTempList.Add(onePinyin);
                    }
                }
                SortKanjiList();
            }
        }

        public void UpdateAddChar(string engText)
        {
            List<PinyinInfo> pinyinInfos = new List<PinyinInfo>();
            foreach (var onePinyin in pinyinTempList)
            {
                bool isIncorrect = true;
                for (int engIndex = 0; isIncorrect && engIndex < onePinyin.eng.Length; engIndex++)
                {
                    if (onePinyin.eng[engIndex].StartsWith(engText))
                    {
                        isIncorrect = false;
                        pinyinInfos.Add(onePinyin);
                    }
                }
            }
            pinyinTempList.Clear();
            pinyinTempList = pinyinInfos;
        }

        public void SortKanjiList()
        {
            //thinking
        }
    }
}
