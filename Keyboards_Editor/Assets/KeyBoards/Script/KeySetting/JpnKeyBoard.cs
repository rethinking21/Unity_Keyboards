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
            space = "��";
        }

        public override void ChangeMode()
        {
            if(keyMode == JpnKeyMerge.KEYMODE.KANJI)
            {
                keyString = hiraganaKeyString;
                space = "��";
                keyMode = JpnKeyMerge.KEYMODE.HIRAGANA;
            }
            else if (keyMode == JpnKeyMerge.KEYMODE.KATAKANA)
            {
                keyString = engKeyString;
                space = "���";
                keyMode = JpnKeyMerge.KEYMODE.KANJI;
            }
            else if (keyMode == JpnKeyMerge.KEYMODE.HIRAGANA)
            {
                keyString = katakanaKeyString;
                space = "��";
                keyMode = JpnKeyMerge.KEYMODE.KATAKANA;
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

        public enum KEYMODE
        {
            HIRAGANA = 0,
            KATAKANA,
            KANJI,
        }

        public enum JPN_STATUS
        {
            NONE = 0,
            CONSONANT, // consonant
            CONSONANT_N,// n Key
            CONSONANT_SECOND, // tsu
            CONSONANT_SECOND_H,// ch sh (cha sha chu shu cho sho chi shi)
            CONSONANT_SECOND_Y, //_y
        }

        #region Jpn Letter Data

        static readonly string engs =
                "qQwWeErRtTyYuUiIoOpP"
                + "aAsSdDfFgGhHjJkKlL"
                + "zZxXcCvVbBnNmM"; //52

        static readonly string consonants = "aiueo"; //AIUEO
        static readonly string JpnShiftEnglish = "ZJAIUEO";
        static readonly string oneIndex = " kstnhmrywjgzdbqf";
        static readonly string twoIndex = "dkstnhmrgzjbq"; // no d(index 0) in katakana

        static readonly char hiraganaSokoun = '��';
        static readonly char hiraganaN = '��';
        static readonly string hiraganaShift = "ZJ����������";
        static readonly string hiraganaTwoAdd = "����"; // a, u, o
        static readonly string hiraganaTwo = "�ª������˪Ҫߪꪮ�����Ӫ�";
        static readonly string hiraganaOne =
            "����������" // none "aiueo"
            + "����������" // k
            + "����������" // s
            + "�����Īƪ�" // t
            + "�ʪ˪̪ͪ�" // n
            + "�ϪҪժت�" // h
            + "�ުߪ���" // m
            + "������" // r
            + "�� �� ��" // y
            + "��� ���" // w
            + "������ ��" //j (ja, ju, jo is two letter)

            + "����������" // g
            + "����������" // z
            + "���ªŪǪ�" // d
            + "�ЪӪ֪٪�" // b
            + "�ѪԪתڪ�" // p
            + "  ��  ";// f

        //some character is missing due to turning to "?" issue
        static readonly string hiraganas =
                "  w eEr t y uUiIoOp "
                + "aAs d f g h jJk ' "
                + "zZ  c   b n m "; //52


        static readonly char katakanaSokoun = '��';
        static readonly char katakanaN = '��';
        static readonly string katakanaShift = "ZJ����������";
        static readonly string katakanaTwoAdd = "����"; // a, u, o
        static readonly string katakanaTwo = " �������˫ҫ߫ꫮ�����ӫ�";
        static readonly string katakanaOne =
            "����������" //none "aiueo"
            + "����������" // k
            + "����������" // s
            + "�����īƫ�" // t
            + "�ʫ˫̫ͫ�" // n
            + "�ϫҫիث�" // h
            + "�ޫ߫���" // m
            + "������" // r
            + "�� �� ��" // y
            + "��� ���" // w
            + "������ ��" // j (ja, ju, jo is two letter)

            + "����������" // g
            + "����������" // z
            + "���«ūǫ�" // d
            + "�Ыӫ֫٫�" // b
            + "�ѫԫ׫ګ�" // p
            + "  ��  "; // f
        //some character is missing due to turning to "?" issue
        static readonly string katakanas =
                "  w eEr t y uUiIoOp "
                + "aAs d f g h jJk ' "
                + "zZ  c   b n m "; //52

        #endregion

        public JpnKeyMerge()
        {
            key = new JpnKeySetting();
        }

        public JpnKeyMerge(JpnKeySetting jpnKey)
        {
            key = jpnKey;
        }

        public override void AddKey(int keyIndex)
        {
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

                    #region waitStatus Switch
                    switch (waitStatus)
                    {
                        case JPN_STATUS.NONE:
                            {
                                if (vowelIndex >= 0)
                                {
                                    text += hiraganaOne[vowelIndex];
                                }
                                else if(keyChar == 'n')
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
                                if (waitString[0] == keyChar)
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
                                if (waitString[0] == keyChar)
                                {
                                    text += hiraganaSokoun;
                                    waitStatus = JPN_STATUS.CONSONANT;
                                    return;
                                }

                                if (keyChar == 'u')
                                {
                                    waitString = "��"; // tsu
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
                                if (waitString[0] == keyChar)
                                {
                                    text += hiraganaSokoun;
                                    waitStatus = JPN_STATUS.CONSONANT;
                                    return;
                                }

                                if (vowelIndex % 2 == 0)
                                {
                                    text += hiraganaTwo[(keyChar == 's') ? 2 : 3].ToString() + hiraganaTwoAdd[vowelIndex / 2];
                                    waitString = "";
                                    waitStatus = JPN_STATUS.NONE;
                                }
                                else if (vowelIndex == 1)
                                {
                                    text += hiraganaOne[(keyChar == 's') ? 2 : 3];
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
                                if(waitString[0] == keyChar)
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
                                if (waitString[0] == keyChar)
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
                                if (waitString[0] == keyChar)
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
                                if (waitString[0] == keyChar)
                                {
                                    text += katakanaSokoun;
                                    waitStatus = JPN_STATUS.CONSONANT;
                                    return;
                                }

                                if (keyChar == 'u')
                                {
                                    waitString = "��"; // tsu
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
                                if (waitString[0] == keyChar)
                                {
                                    text += katakanaSokoun;
                                    waitStatus = JPN_STATUS.CONSONANT;
                                    return;
                                }

                                if (vowelIndex % 2 == 0)
                                {
                                    text += katakanaTwo[(keyChar == 's') ? 2 : 3].ToString() + katakanaTwoAdd[vowelIndex / 2];
                                    waitString = "";
                                    waitStatus = JPN_STATUS.NONE;
                                }
                                else if (vowelIndex == 1)
                                {
                                    text += katakanaOne[(keyChar == 's') ? 2 : 3];
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
                                if (waitString[0] == keyChar)
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
                                else if (keyChar == 'h' && (waitString[0] == 's' || waitString[0] == 'c'))
                                {
                                    waitString += keyChar;
                                    waitStatus = JPN_STATUS.CONSONANT_SECOND_H;
                                }
                                else if (keyChar == 'y' && (twoIndex.IndexOf(waitString[0]) > 0))
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
                                if (waitString[0] == keyChar)
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
                else if (keyMode == KEYMODE.KANJI)
                {
                    //thinking better way (search)
                    #region waitStatus Switch


                    #endregion
                }
            }
            else if (keyIndex >= 100 && keyIndex < 110)
            {
                if(keyMode == KEYMODE.KANJI)
                {
                    waitStatus = JPN_STATUS.NONE;
                }
                else
                {
                    PushTempChar();
                    text += (keyIndex - 100).ToString();
                }
            }
            else if (keyIndex == -3) // del
            {
                if (keyMode != KEYMODE.KANJI)
                {
                    #region waitStatus Switch
                    switch (waitStatus)
                    {
                        case JPN_STATUS.CONSONANT:
                            {
                                waitString = "";
                                waitStatus = JPN_STATUS.NONE;
                            }
                            break;
                        case JPN_STATUS.CONSONANT_N:
                            {
                                waitString = "";
                                waitStatus = JPN_STATUS.NONE;
                            }
                            break;
                        case JPN_STATUS.CONSONANT_SECOND:
                            {
                                waitString = waitString[0].ToString();
                                waitStatus = JPN_STATUS.CONSONANT;
                            }
                            break;
                        case JPN_STATUS.CONSONANT_SECOND_H:
                            {
                                waitString = waitString[0].ToString();
                                waitStatus = JPN_STATUS.CONSONANT;
                            }
                            break;
                        case JPN_STATUS.CONSONANT_SECOND_Y:
                            {
                                waitString = waitString[0].ToString();
                                waitStatus = JPN_STATUS.CONSONANT;
                            }
                            break;
                    }
                    #endregion
                }
                else
                {

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
                keyMode = (key as JpnKeySetting).keyMode;
                UIChanged = true;
            }
        }

        public override string GetString()
        {
            return text + waitString;
            //thinking..
        }

        public override void ClearAll()
        {
            text = "";
            waitString = "";
            waitStatus = JPN_STATUS.NONE;
            keyMode = KEYMODE.HIRAGANA;
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

    }
}
