using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Keyboard
{
    public class JpnKeySetting : KeySetting
    {
        public JpnKeyMerge.KEYMODE keyMode = JpnKeyMerge.KEYMODE.HIRAGANA;

        public JpnKeySetting()
        {
            keyString = hiraganaKeyString;
            space = "ひらがな(ローマ字入力)";
            nextNum = "  ";
            language = "Jpn";
            shift = "(小文字入力)";
        }

        public override void ChangeMode()
        {
            if(keyMode == JpnKeyMerge.KEYMODE.KATAKANA)
            {
                keyString = hiraganaKeyString;
                space = "ひらがな(ローマ字入力)";
                nextNum = "  ";
                keyMode = JpnKeyMerge.KEYMODE.HIRAGANA;
                shift = "(小文字入力)";
            }
            // else if (keyMode == JpnKeyMerge.KEYMODE.KATAKANA)
            // {
            //     keyString = engKeyString;
            //     space = "漢字";
            //     nextNum = "<>";
            //     keyMode = JpnKeyMerge.KEYMODE.KANJI;
            // }
            else if (keyMode == JpnKeyMerge.KEYMODE.HIRAGANA)
            {
                keyString = katakanaKeyString;
                space = "カタカナ(ローマ字入力)";
                nextNum = "  ";
                keyMode = JpnKeyMerge.KEYMODE.KATAKANA;
                shift = "(小文字入力)";
            }
        }

        //some character is missing due to turning to "?" issue
        string hiraganaKeyString =
                "  w eEr t y uUiIoOp "
                + "aAs d f g h jJk ' "
                + "zZ  c   b n m "; //52

        //some character is missing due to turning to "?" issue
        string katakanaKeyString =
                "  w eEr t y uUiIoOp "
                + "aAs d f g h jJk ' "
                + "zZ  c   b n m "; //52 

        string engKeyString =
                "qQwWeErRtTyYuUiIoOpP"
                + "aAsSdDfFgGhHjJkKlL"
                + "zZxXcCvVbBnNmM"; //52
    }

    public class JpnKeyMerge : KeyMerge
    {
        string waitString = "";
        JPN_STATUS waitStatus = JPN_STATUS.NONE;
        KEYMODE keyMode = KEYMODE.HIRAGANA;

        JpnKanjiDatas kanjiData = null;
        int kanjiIndex = 0;

        public enum KEYMODE
        {
            HIRAGANA = 0,
            KATAKANA,
            //KANJI,
        }

        public enum JPN_STATUS
        {
            NONE = 0,
            CONSONANT, // consonant
            CONSONANT_N,// n Key
            CONSONANT_SECOND, // tsu
            CONSONANT_SECOND_H,// ch sh (cha sha chu shu cho sho chi shi)
            CONSONANT_SECOND_Y, //_y
            KANJI_SEARCH
        }

        #region Jpn Letter Data

        static readonly string engs =
                "qQwWeErRtTyYuUiIoOpP"
                + "aAsSdDfFgGhHjJkKlL"
                + "zZxXcCvVbBnNmM"; //52

        static readonly string consonants = "aiueo"; //AIUEO
        static readonly string JpnShiftEnglish = "ZJAIUEO";
        static readonly string oneIndex = " kstnhmrywjcgzdbpf";
        static readonly string twoIndex = "dkstnhmrgzjbp"; // no d(index 0) in katakana

        static readonly char hiraganaSokoun = 'っ';
        static readonly char hiraganaN = 'ん';
        static readonly string hiraganaShift = "ZJぁぃぅぇぉ";
        static readonly string hiraganaTwoAdd = "ゃゅょ"; // a, u, o
        static readonly string hiraganaTwo = "ぢきしちにひみりぎじじびぴ";
        static readonly string hiraganaOne =
            "あいうえお" // none "aiueo"
            + "かきくけこ" // k
            + "さしすせそ" // s
            + "たちつてと" // t
            + "なにぬねの" // n
            + "はひふへほ" // h
            + "まみむめも" // m
            + "らりるれろ" // r
            + "や ゆ よ" // y
            + "わゐ ゑを" // w
            + "じじじ じ" //j (ja, ju, jo is two letter)
            + "かきくけこ" // c
            + "がぎぐげご" // g
            + "ざじずぜぞ" // z
            + "だぢづでど" // d
            + "ばびぶべぼ" // b
            + "ぱぴぷぺぽ" // p
            + "  ふ  ";// f

        //some character is missing due to turning to "?" issue
        static readonly string hiraganas =
                "  w eEr t y uUiIoOp "
                + "aAs d f g h jJk ' "
                + "zZ  c   b n m "; //52


        static readonly char katakanaSokoun = 'ッ';
        static readonly char katakanaN = 'ン';
        static readonly string katakanaShift = "ZJァィゥェォ";
        static readonly string katakanaTwoAdd = "ャュョ"; // a, u, o
        static readonly string katakanaTwo = " キシチニヒミリギジジビピ";
        static readonly string katakanaOne =
            "アイウエオ" //none "aiueo"
            + "カキクケコ" // k
            + "サシスセソ" // s
            + "タチツテト" // t
            + "ナニヌネノ" // n
            + "ハヒフヘホ" // h
            + "マミムメモ" // m
            + "ラリルレロ" // r
            + "ヤ ユ ヨ" // y
            + "ワヰ ヱヲ" // w
            + "ジジジ ジ" // j (ja, ju, jo is two letter)
            + "カキクケコ" // c
            + "ガギグゲゴ" // g
            + "ザジズゼゾ" // z
            + "ダヂヅデド" // d
            + "バビブベボ" // b
            + "パピプペポ" // p
            + "  フ  "; // f
        //some character is missing due to turning to "?" issue
        static readonly string katakanas =
                "  w eEr t y uUiIoOp "
                + "aAs d f g h jJk ' "
                + "zZ  c   b n m "; //52

        #endregion

        public JpnKeyMerge()
        {
            key = new JpnKeySetting();
            KanjiDataLoad();
        }

        public JpnKeyMerge(JpnKeySetting jpnKey)
        {
            key = jpnKey;
            KanjiDataLoad();
        }

        public override void AddKey(int keyIndex)
        {
            if (IsTextLimited(keyIndex)) return;

            if (keyIndex >= 0 && key.keyString.Length > keyIndex)
            {
                if (keyMode == KEYMODE.HIRAGANA)
                {
                    char keyChar = hiraganas[keyIndex];
                    int vowelIndex = consonants.IndexOf(keyChar);

                    if (keyChar == ' ')
                        return;
                    else if(keyChar == '\'')
                    {
                        PushTempChar();
                        text += "\'";
                        return;
                    }
                    else if(keyIndex % 2 == 1) // shift
                    {
                        PushTempChar();
                        text += hiraganaShift[JpnShiftEnglish.IndexOf(hiraganas[keyIndex])];
                        return;
                    }

                    try
                    {
                        #region waitStatus Switch

                        switch (waitStatus)
                        {
                            case JPN_STATUS.NONE:
                            {
                                if (vowelIndex >= 0)
                                {
                                    text += hiraganaOne[vowelIndex];
                                }
                                else if (keyChar == 'n')
                                {
                                    waitString += hiraganaN;
                                    waitStatus = JPN_STATUS.CONSONANT_N;
                                }
                                else
                                {
                                    waitString += keyChar;
                                    waitStatus = JPN_STATUS.CONSONANT;
                                }
                            }
                                break;
                            case JPN_STATUS.CONSONANT_N:
                            {
                                if (waitString.Length > 0 && waitString[0] == keyChar)
                                {
                                    text += hiraganaSokoun;
                                    return;
                                }

                                if (vowelIndex >= 0)
                                {
                                    waitString = hiraganaOne[(5 * 4) + vowelIndex].ToString();
                                }
                                else if (keyChar == 'y')
                                {
                                    waitString = "ny";
                                    waitStatus = JPN_STATUS.CONSONANT_SECOND_Y;
                                }
                                else
                                {
                                    PushTempChar();
                                    AddKey(keyIndex);
                                }
                            }
                                break;
                            case JPN_STATUS.CONSONANT_SECOND:
                            {
                                if (waitString.Length > 0 && waitString[0] == keyChar)
                                {
                                    text += hiraganaSokoun;
                                    waitStatus = JPN_STATUS.CONSONANT;
                                    return;
                                }

                                if (keyChar == 'u')
                                {
                                    waitString = "つ"; // tsu
                                    PushTempChar();
                                }
                                else
                                {
                                    text += 't';
                                    waitString = "s";
                                    waitStatus = JPN_STATUS.CONSONANT;
                                    AddKey(keyIndex);
                                }
                            }
                                break;
                            case JPN_STATUS.CONSONANT_SECOND_H:
                            {
                                if (waitString.Length > 0 && waitString[0] == keyChar)
                                {
                                    text += hiraganaSokoun;
                                    waitStatus = JPN_STATUS.CONSONANT;
                                    return;
                                }

                                if (vowelIndex % 2 == 0)
                                {
                                    text += hiraganaTwo[(keyChar == 's') ? 2 : 3].ToString() +
                                            hiraganaTwoAdd[vowelIndex / 2];
                                    waitString = "";
                                    waitStatus = JPN_STATUS.NONE;
                                }
                                else if (vowelIndex == 1)
                                {
                                    text += hiraganaOne[((keyChar == 's') ? 2 : 3) * 5 + 1];
                                    waitString = "";
                                    waitStatus = JPN_STATUS.NONE;
                                }
                                else
                                {
                                    if (waitString.Length > 0)
                                    {
                                        text += waitString[0];
                                        waitString = "h";
                                        waitStatus = JPN_STATUS.CONSONANT;
                                        AddKey(keyIndex);
                                    }
                                }
                            }
                                break;
                            case JPN_STATUS.CONSONANT:
                            {
                                if (waitString.Length > 0 && waitString[0] == keyChar)
                                {
                                    text += hiraganaSokoun;
                                    return;
                                }

                                if (vowelIndex >= 0)
                                {
                                    int consonantIndex = oneIndex.IndexOf(waitString[0]);
                                    char mergeChar = hiraganaOne[consonantIndex * 5 + vowelIndex];
                                    if (mergeChar != ' ')
                                    {
                                        text += mergeChar;
                                        if (waitString[0] == 'j' && vowelIndex % 2 == 0) // ja ju jo
                                        {
                                            text += hiraganaTwoAdd[vowelIndex / 2];
                                        }

                                        waitString = "";
                                        waitStatus = JPN_STATUS.NONE;
                                    }
                                    else
                                    {
                                        PushTempChar();
                                        AddKey(keyIndex);
                                    }
                                }
                                else if (keyChar == 'h' && (waitString[0] == 's' || waitString[0] == 'c'))
                                {
                                    waitString += keyChar;
                                    waitStatus = JPN_STATUS.CONSONANT_SECOND_H;
                                }
                                else if (keyChar == 'y' && (twoIndex.IndexOf(waitString[0]) >= 0))
                                {
                                    waitString += keyChar;
                                    waitStatus = JPN_STATUS.CONSONANT_SECOND_Y;
                                }
                                else if (keyChar == 's' && waitString[0] == 't') // tsu
                                {
                                    waitString += keyChar;
                                    waitStatus = JPN_STATUS.CONSONANT_SECOND;
                                }
                                else
                                {
                                    PushTempChar();
                                    AddKey(keyIndex);
                                }
                            }
                                break;
                            case JPN_STATUS.CONSONANT_SECOND_Y:
                            {
                                if (waitString.Length > 0 && waitString[0] == keyChar)
                                {
                                    text += hiraganaSokoun;
                                    waitStatus = JPN_STATUS.CONSONANT;
                                    return;
                                }

                                if (vowelIndex >= 0 && vowelIndex % 2 == 0) // a, u, o
                                {
                                    waitString =
                                        hiraganaTwo[twoIndex.IndexOf(waitString[0])].ToString()
                                        + hiraganaTwoAdd[vowelIndex / 2];
                                    PushTempChar();
                                }
                                else
                                {
                                    text += waitString[0];
                                    waitString = waitString[1].ToString();
                                    waitStatus = JPN_STATUS.CONSONANT;
                                    AddKey(keyIndex);
                                }
                            }
                                break;
                        }

                        #endregion
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                }
                else if (keyMode == KEYMODE.KATAKANA)
                {
                    char keyChar = katakanas[keyIndex];
                    int vowelIndex = consonants.IndexOf(keyChar);

                    if (katakanas[keyIndex] == ' ')
                        return;
                    else if (katakanas[keyIndex] == '\'')
                    {
                        PushTempChar();
                        text += "\'";
                        return;
                    }
                    else if (keyIndex % 2 == 1) // shift
                    {
                        PushTempChar();
                        text += katakanaShift[JpnShiftEnglish.IndexOf(katakanas[keyIndex])];
                        return;
                    }

                    try
                    {
                        #region waitStatus Switch

                        switch (waitStatus)
                        {
                            case JPN_STATUS.NONE:
                            {
                                if (vowelIndex >= 0)
                                {
                                    text += katakanaOne[vowelIndex];
                                }
                                else if (keyChar == 'n')
                                {
                                    waitString += katakanaN;
                                    waitStatus = JPN_STATUS.CONSONANT_N;
                                }
                                else
                                {
                                    waitString += keyChar;
                                    waitStatus = JPN_STATUS.CONSONANT;
                                }
                            }
                                break;
                            case JPN_STATUS.CONSONANT_N:
                            {
                                if (waitString.Length > 0 && waitString[0] == keyChar)
                                {
                                    text += katakanaSokoun;
                                    return;
                                }

                                if (vowelIndex >= 0)
                                {
                                    waitString = katakanaOne[(5 * 4) + vowelIndex].ToString();
                                }
                                else if (keyChar == 'y')
                                {
                                    waitString = "ny";
                                    waitStatus = JPN_STATUS.CONSONANT_SECOND_Y;
                                }
                                else
                                {
                                    PushTempChar();
                                    AddKey(keyIndex);
                                }
                            }
                                break;
                            case JPN_STATUS.CONSONANT_SECOND:
                            {
                                if (waitString.Length > 0 && waitString[0] == keyChar)
                                {
                                    text += katakanaSokoun;
                                    waitStatus = JPN_STATUS.CONSONANT;
                                    return;
                                }

                                if (keyChar == 'u')
                                {
                                    waitString = "ツ"; // tsu
                                    PushTempChar();
                                }
                                else
                                {
                                    text += 't';
                                    waitString = "s";
                                    waitStatus = JPN_STATUS.CONSONANT;
                                    AddKey(keyIndex);
                                }
                            }
                                break;
                            case JPN_STATUS.CONSONANT_SECOND_H:
                            {
                                if (waitString.Length > 0 && waitString[0] == keyChar)
                                {
                                    text += katakanaSokoun;
                                    waitStatus = JPN_STATUS.CONSONANT;
                                    return;
                                }

                                if (vowelIndex % 2 == 0)
                                {
                                    text += katakanaTwo[(keyChar == 's') ? 2 : 3].ToString() +
                                            katakanaTwoAdd[vowelIndex / 2];
                                    waitString = "";
                                    waitStatus = JPN_STATUS.NONE;
                                }
                                else if (vowelIndex == 1)
                                {
                                    text += katakanaOne[((keyChar == 's') ? 2 : 3) * 5 + 1];
                                    waitString = "";
                                    waitStatus = JPN_STATUS.NONE;
                                }
                                else
                                {
                                    text += waitString[0];
                                    waitString = "h";
                                    waitStatus = JPN_STATUS.CONSONANT;
                                    AddKey(keyIndex);
                                }
                            }
                                break;
                            case JPN_STATUS.CONSONANT:
                            {
                                if (waitString.Length > 0 && waitString[0] == keyChar)
                                {
                                    text += katakanaSokoun;
                                    return;
                                }

                                if (vowelIndex >= 0)
                                {
                                    int consonantIndex = oneIndex.IndexOf(waitString[0]);
                                    char mergeChar = katakanaOne[consonantIndex * 5 + vowelIndex];
                                    if (mergeChar != ' ')
                                    {
                                        text += mergeChar;
                                        if (waitString[0] == 'j' && vowelIndex % 2 == 0) // ja ju jo
                                        {
                                            text += katakanaTwoAdd[vowelIndex / 2];
                                        }

                                        waitString = "";
                                        waitStatus = JPN_STATUS.NONE;
                                    }
                                    else
                                    {
                                        PushTempChar();
                                        AddKey(keyIndex);
                                    }
                                }
                                else if (keyChar == 'h' && waitString.Length > 0 &&
                                         (waitString[0] == 's' || waitString[0] == 'c'))
                                {
                                    waitString += keyChar;
                                    waitStatus = JPN_STATUS.CONSONANT_SECOND_H;
                                }
                                else if (keyChar == 'y' && waitString.Length > 0 &&
                                         (twoIndex.IndexOf(waitString[0]) > 0))
                                {
                                    waitString += keyChar;
                                    waitStatus = JPN_STATUS.CONSONANT_SECOND_Y;
                                }
                                else if (keyChar == 's' && waitString.Length > 0 && waitString[0] == 't') // tsu
                                {
                                    waitString += keyChar;
                                    waitStatus = JPN_STATUS.CONSONANT_SECOND;
                                }
                                else
                                {
                                    PushTempChar();
                                    AddKey(keyIndex);
                                }
                            }
                                break;
                            case JPN_STATUS.CONSONANT_SECOND_Y:
                            {
                                if (waitString.Length > 0 && waitString[0] == keyChar)
                                {
                                    text += katakanaSokoun;
                                    waitStatus = JPN_STATUS.CONSONANT;
                                    return;
                                }

                                if (vowelIndex >= 0 && vowelIndex % 2 == 0) // a, u, o
                                {
                                    waitString =
                                        katakanaTwo[twoIndex.IndexOf(waitString[0])].ToString()
                                        + katakanaTwoAdd[vowelIndex / 2];
                                    PushTempChar();
                                }
                                else
                                {
                                    text += waitString[0];
                                    waitString = waitString[1].ToString();
                                    waitStatus = JPN_STATUS.CONSONANT;
                                    AddKey(keyIndex);
                                }
                            }
                                break;
                        }

                        #endregion
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                }
                // else if (keyMode == KEYMODE.KANJI)
                // {
                //     if (keyIndex % 2 == 1)
                //     {
                //         PushTempChar();
                //         text += engs[keyIndex];
                //         waitStatus = JPN_STATUS.NONE;
                //     }
                //     else
                //     {
                //         waitString += engs[keyIndex];
                //         if(waitString.Length < 2)
                //         {
                //             UpdateSearchKanji(ref waitString);
                //         }
                //         else
                //         {
                //             UpdateAddCharKanji(ref waitString);
                //         }
                //         kanjiIndex = 0;
                //     }
                // }
            }
            else if (keyIndex >= 100 && keyIndex < 110)
            {
                // if(keyMode == KEYMODE.KANJI)
                // {
                //     if (!"0123456789".Contains(key.num[keyIndex - 100].ToString()))
                //     {
                //         waitString = key.num[keyIndex - 100].ToString();
                //         kanjiData.kanjiTempList.Clear();
                //         PushTempChar();
                //         key.num = "0123456789";
                //     }
                //     else
                //     {
                //         PushTempChar();
                //         text += (keyIndex - 100).ToString();
                //     }
                //     waitStatus = JPN_STATUS.NONE;
                // }
                // else
                {
                    PushTempChar();
                    text += (keyIndex - 100).ToString();
                }
            }
            // else if (keyIndex >= 110 && keyIndex < 112)
            // {
            //     if (keyMode == KEYMODE.KANJI)
            //     {
            //         if(keyIndex == 110)
            //         {
            //             kanjiIndex--;
            //             if (kanjiIndex < 0) kanjiIndex = 0;
            //         }
            //         else if (kanjiData.kanjiTempList.Count > (kanjiIndex * 10))
            //         {
            //             kanjiIndex++;
            //         }
            //         key.num = ShowKanjiNum(kanjiIndex);
            //         UIChanged = true;
            //     }
            // }
            else if (keyIndex == -3) // del
            {
                // if (keyMode != KEYMODE.KANJI)
                // {
                //     #region waitStatus Switch
                //     switch (waitStatus)
                //     {
                //         case JPN_STATUS.CONSONANT:
                //             {
                //                 waitString = "";
                //                 waitStatus = JPN_STATUS.NONE;
                //             }
                //             break;
                //         case JPN_STATUS.CONSONANT_N:
                //             {
                //                 waitString = "";
                //                 waitStatus = JPN_STATUS.NONE;
                //             }
                //             break;
                //         case JPN_STATUS.CONSONANT_SECOND:
                //             {
                //                 waitString = waitString[0].ToString();
                //                 waitStatus = JPN_STATUS.CONSONANT;
                //             }
                //             break;
                //         case JPN_STATUS.CONSONANT_SECOND_H:
                //             {
                //                 waitString = waitString[0].ToString();
                //                 waitStatus = JPN_STATUS.CONSONANT;
                //             }
                //             break;
                //         case JPN_STATUS.CONSONANT_SECOND_Y:
                //             {
                //                 waitString = waitString[0].ToString();
                //                 waitStatus = JPN_STATUS.CONSONANT;
                //             }
                //             break;
                //     }
                //     #endregion
                // }
                // else
                {
                    if(waitString.Length != 0)
                    {
                        waitString = waitString.Remove(waitString.Length - 1);
                        if (waitString.Length != 0)
                        {
                            UpdateSearchKanji(ref waitString);
                        }
                        else
                        {
                            key.num = "0123456789";
                            kanjiData.kanjiTempList.Clear();
                            UIChanged = true;
                        }
                    }
                    kanjiIndex = 0;
                }
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
                key.ChangeMode();
                key.num = "0123456789";
                keyMode = (key as JpnKeySetting).keyMode;
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
            waitStatus = JPN_STATUS.NONE;
        }

        public override void PushTempChar()
        {
            text += waitString;
            waitString = "";
            waitStatus = JPN_STATUS.NONE;
        }

        public override bool CanDeleteText()
        {
            return waitString.Length == 0;
        }

        #region Kanji Search
        void KanjiDataLoad()
        {
            kanjiData = new JpnKanjiDatas();
            kanjiData.LoadJsonToClass();
        }

        void UpdateSearchKanji(ref string engText)
        {
            kanjiData.UpdateSearch(engText);
            key.num = ShowKanjiNum(0);
            UIChanged = true;
        }

        void UpdateAddCharKanji(ref string engText)
        {
            kanjiData.UpdateAddChar(engText);
            key.num = ShowKanjiNum(0);
            UIChanged = true;
        }

        string ShowKanjiNum(int numIndex)
        {
            string result = "";
            for (int index = 0; index < 10 ; index++)
            {
                if (kanjiData.kanjiTempList.Count > (index + numIndex * 10))
                {
                    if (index == 9)
                    {
                        result = kanjiData.kanjiTempList[index + numIndex * 10].literal + result;
                    }
                    else
                    {
                        result += kanjiData.kanjiTempList[index + numIndex * 10].literal;
                    }
                }
                else
                {
                    if (index == 9)
                    {
                        result = ((index + 1) % 10).ToString() + result;
                    }
                    else
                    {
                        result += ((index + 1) % 10).ToString();
                    }
                }
            }
            return result;
        }

        #endregion

    }
}
