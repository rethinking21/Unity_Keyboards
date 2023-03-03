using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Keyboard 
{
    public class JpnKanjiDatas
    {
        [Serializable]
        public class kanjiInfo
        {
            public string literal;
            public int freq;
            public string[] eng;
            public string[] jpn;
        }

        [Serializable]
        public class Kanji
        {
            public kanjiInfo[] list;
        }

        // worry about path..
        string jsonPath = "Keyboards/Data/Jpn/kanji_sorted";
        TextAsset textAsset = null;
        Kanji kanji = null;
        public List<kanjiInfo> kanjiTempList = new List<kanjiInfo>();

        #region declartion
        public JpnKanjiDatas()
        {
            if (textAsset == null && jsonPath != null)
            {
                textAsset = Resources.Load<TextAsset>(jsonPath);
            }
        }

        public JpnKanjiDatas(string path)
        {
            textAsset = Resources.Load<TextAsset>(path);
            jsonPath = path;
        }

        public JpnKanjiDatas(TextAsset asset)
        {
            textAsset = asset;
        }
        #endregion

        public void LoadJsonToClass()
        {
            if (textAsset == null) return;
            kanji = JsonUtility.FromJson<Kanji>(textAsset.text);
            textAsset = null;
        }

        public void UpdateSearch(string searchText)
        {
            StringBuilder result = new StringBuilder();
            kanjiTempList.Clear();
            foreach (var oneKanji in kanji.list)
            {
                //thinking
                bool isIncorrect = true;
                for (int engIndex = 0; isIncorrect && engIndex < oneKanji.eng.Length; engIndex++)
                {
                    if (oneKanji.eng[engIndex].StartsWith(searchText))
                    {
                        isIncorrect = false;
                        kanjiTempList.Add(oneKanji);
                    }
                }
                SortKanjiList();
            }
        }

        public void UpdateAddChar(string engText)
        {
            List<kanjiInfo> kanjiInfos = new List<kanjiInfo>();
            foreach(var oneKanji in kanjiTempList)
            {
                bool isIncorrect = true;
                for (int engIndex = 0; isIncorrect && engIndex < oneKanji.eng.Length; engIndex++)
                {
                    if (oneKanji.eng[engIndex].StartsWith(engText))
                    {
                        isIncorrect = false;
                        kanjiInfos.Add(oneKanji);
                    }
                }
            }
            kanjiTempList.Clear();
            kanjiTempList = kanjiInfos;
        }

        public void SortKanjiList()
        {
            //thinking
        }
    }
}
