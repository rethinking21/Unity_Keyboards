using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Keyboard
{
    public class KorKeySetting : KeySetting
    {
        public KorKeySetting()
        {
            keyString =
            "ㅂㅃㅈㅉㄷㄸㄱㄲㅅㅆㅛㅛㅕㅕㅑㅑㅐㅒㅔㅖ" // 0 - 19
            + "ㅁㅁㄴㄴㅇㅇㄹㄹㅎㅎㅗㅗㅓㅓㅏㅏㅣㅣ" // 20 - 37
            + "ㅋㅋㅌㅌㅊㅊㅍㅍㅠㅠㅜㅜㅡㅡ"; // 38 - 51
            language = "Kor";
        }
    }

    public class KorKeyMerge : KeyMerge
    {
        string waitString = "";
        KOR_STATUS waitStatus = KOR_STATUS.NONE;

        public enum KOR_STATUS
        {
            NONE = 0,
            FIRST_V, // 초성 (모음)
            FIRST_C, // 초성 (자음)
            FIRST_VV, // 초성 (모음 + 모음)
            FIRST_CC, // 초성 (자음 + 자음) (여기선 비사용)
            MIDDLE, // 자음 + 모음
            MIDDLE_VV, // 자음 + 모음 + 모음
            END, // 자음 + 모음 + 자음
            END_CC // 자음 + 모음 + 자음 + 자음
        }

        #region Kor Letter Data
        static readonly int basic = 0xAC00;
        static readonly int[] Vowels =   // vowel keycode
            {10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
            30, 31, 32, 33, 34, 35, 36, 37,
            46, 47, 48, 49, 50, 51};
        static readonly int[] Vowels_VV = 
            {28, 29, 30, 33, 34, 35, 38};

        // 초성, 중성, 종성 테이블.
        static readonly string SOUND_TABLE =
        // 초성 19자 0 ~ 18
        "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ" +
        // 중성 21자 19 ~ 39
        "ㅏㅐㅑㅒㅓㅔㅕㅖㅗㅘㅙㅚㅛㅜㅝㅞㅟㅠㅡㅢㅣ" +
        // 종성 28자 40 ~ 67
        " ㄱㄲㄳㄴㄵㄶㄷㄹㄺㄻㄼㄽㄾㄿㅀㅁㅂㅄㅅㅆㅇㅈㅊㅋㅌㅍㅎ";

        readonly Dictionary<int, int> keyToTable = new Dictionary<int, int>()
        {
            {0, 7}, //ㅂ
            {1, 8}, //ㅃ
            {2, 12}, //ㅈ
            {3, 13}, //ㅉ
            {4, 3}, //ㄷ
            {5, 4}, //ㄸ
            {6, 0}, //ㄱ
            {7, 1}, //ㄲ
            {8, 9}, //ㅅ
            {9, 10}, //ㅆ
            
            {10, 31}, //ㅛ
            {11, 31}, //ㅛ
            {12, 25}, //ㅕ
            {13, 25}, //ㅕ
            {14, 21}, //ㅑ
            {15, 21}, //ㅑ
            {16, 20}, //ㅐ
            {17, 22}, //ㅒ
            {18, 24}, //ㅔ
            {19, 26}, //ㅖ
             
            {20, 6}, //ㅁ
            {21, 6}, //ㅁ
            {22, 2}, //ㄴ
            {23, 2}, //ㄴ
            {24, 11}, //ㅇ
            {25, 11}, //ㅇ
            {26, 5}, //ㄹ
            {27, 5}, //ㄹ
            {28, 18}, //ㅎ
            {29, 18}, //ㅎ
             
            {30, 27}, //ㅗ
            {31, 27}, //ㅗ
            {32, 23}, //ㅓ
            {33, 23}, //ㅓ
            {34, 19}, //ㅏ
            {35, 19}, //ㅏ
            {36, 39}, //ㅣ
            {37, 39}, //ㅣ
            {38, 15}, //ㅋ
            {39, 15}, //ㅋ
             
            {40, 16}, //ㅌ
            {41, 16}, //ㅌ
            {42, 14}, //ㅊ
            {43, 14}, //ㅊ
            {44, 17}, //ㅍ
            {45, 17}, //ㅍ
            {46, 36}, //ㅠ
            {47, 36}, //ㅠ
            {48, 32}, //ㅜ
            {49, 32}, //ㅜ
             
            {50, 37}, //ㅡ
            {51, 37}, //ㅡ
        };
        readonly Dictionary<char, int> endTable = new Dictionary<char, int>()
        {
            {' ', 0},
            {'ㄱ', 1},
            {'ㄲ', 2},
            {'ㄳ', 3},
            {'ㄴ', 4},
            {'ㄵ', 5},
            {'ㄶ', 6},
            {'ㄷ', 7},
            {'ㄹ', 8},
            {'ㄺ', 9},

            {'ㄻ', 10},
            {'ㄼ', 11},
            {'ㄽ', 12},
            {'ㄾ', 13},
            {'ㄿ', 14},
            {'ㅀ', 15},
            {'ㅁ', 16},
            {'ㅂ', 17},
            {'ㅄ', 18},
            {'ㅅ', 19},

            {'ㅆ', 20},
            {'ㅇ', 21},
            {'ㅈ', 22},
            {'ㅊ', 23},
            {'ㅋ', 24},
            {'ㅌ', 25},
            {'ㅍ', 26},
            {'ㅎ', 27},

        };
        readonly Dictionary<(char, int), int> mergeDict = new Dictionary<(char, int), int>() // (char, key) Table
        {
            { ('ㅗ', 19), 28 }, // ㅘ
            { ('ㅗ', 20), 29 }, // ㅙ
            { ('ㅗ', 39), 30 }, // ㅚ
            { ('ㅜ', 23), 33 }, // ㅝ
            { ('ㅜ', 24), 34 }, // ㅞ
            { ('ㅜ', 39), 35 }, // ㅟ
            { ('ㅡ', 39), 38 }, // ㅢ

            { ('ㄱ', 19), 3 }, // ㄳ
            { ('ㄴ', 22), 5 }, // ㄵ
            { ('ㄴ', 27), 6 }, // ㄶ
            { ('ㄹ', 1), 9 }, // ㄺ
            { ('ㄹ', 16), 10 }, // ㄻ
            { ('ㄹ', 17), 11 }, // ㄼ
            { ('ㄹ', 19), 12 }, // ㄽ
            { ('ㄹ', 25), 13 }, // ㄾ
            { ('ㄹ', 26), 14 }, // ㄿ
            { ('ㄹ', 27), 15 }, // ㅀ
            { ('ㅂ', 19), 18 }, // ㅄ
        };
        readonly Dictionary<int, (int, int)> UnMergeDict = new Dictionary<int, (int, int)>()
        {
            {28, (27, 19)}, // ㅘ
            {29, (27, 20)}, // ㅙ
            {30, (27, 39)}, // ㅚ
            {33, (32, 23)}, // ㅝ
            {34, (32, 24)}, // ㅞ
            {35, (32, 39)}, // ㅟ
            {38, (37, 39)}, // ㅢ

            {3, (1, 19)}, // ㄳ
            {5, (4, 22)}, // ㄵ
            {6, (4, 27)}, // ㄶ
            {9 , (8, 1)}, // ㄺ
            {10, (8, 16)}, // ㄻ
            {11, (8, 17)}, // ㄼ
            {12, (8, 19)}, // ㄽ
            {13, (8, 25)}, // ㄾ
            {14, (8, 26)}, // ㄿ
            {15, (8, 27)}, // ㅀ
            {18, (8, 19)}, // ㅄ
        };
        #endregion

        public KorKeyMerge()
        {
            key = new KorKeySetting();
        }

        public KorKeyMerge(KorKeySetting korKey)
        {
            key = korKey;
        }


        public override void AddKey(int keyIndex)
        {
            if (keyIndex >= 0 && key.keyString.Length > keyIndex)
            {
                //add key
                (int first, int mid, int last) disuniteChar;
                #region waitStatus Switch
                switch (waitStatus)
                {
                    case KOR_STATUS.NONE: // 초기상태
                        {
                            waitString += key.keyString[keyIndex];
                            if (Array.IndexOf(Vowels, keyIndex) != -1) // vowel check
                            {
                                waitStatus = KOR_STATUS.FIRST_V;
                            }
                            else
                            {
                                waitStatus = KOR_STATUS.FIRST_C;
                            }
                            
                        }
                        break;

                    case KOR_STATUS.FIRST_V: // 초성 (모음)
                        {
                            if (Array.IndexOf(Vowels, keyIndex) != -1) // vowel check
                            {
                                if (mergeDict.ContainsKey(((char)waitString[waitString.Length - 1], keyToTable[keyIndex])))
                                {
                                    EditLastLetter(ref waitString,
                                        SOUND_TABLE[mergeDict[((char)waitString[waitString.Length - 1], keyToTable[keyIndex])]]);
                                    waitStatus = KOR_STATUS.FIRST_VV;
                                }
                                else
                                {
                                    PushTempChar();
                                    waitString += key.keyString[keyIndex];
                                    waitStatus = KOR_STATUS.FIRST_V;
                                }
                            }
                            else
                            {
                                PushTempChar();
                                waitString += key.keyString[keyIndex];
                                waitStatus = KOR_STATUS.FIRST_C;
                            }
                        }
                        break;

                    case KOR_STATUS.FIRST_C: // 초성 (자음)
                        {
                            if (Array.IndexOf(Vowels, keyIndex) != -1) // vowel check
                            {
                                char uniteChar = Unite(SOUND_TABLE.IndexOf(waitString[waitString.Length - 1]), keyToTable[keyIndex] - 19);
                                EditLastLetter(ref waitString,  uniteChar);
                                waitStatus = KOR_STATUS.MIDDLE;
                            }
                            else
                            {
                                PushTempChar();
                                waitString += key.keyString[keyIndex];
                                waitStatus = KOR_STATUS.FIRST_C;
                            }
                        }
                        break;

                    case KOR_STATUS.FIRST_VV: // 초성 (모음 + 모음)
                        {
                            PushTempChar();
                            waitString += key.keyString[keyIndex];
                            if (Array.IndexOf(Vowels, keyIndex) != -1) // vowel check
                            {
                                waitStatus = KOR_STATUS.FIRST_V;
                            }
                            else
                            {
                                waitStatus = KOR_STATUS.FIRST_C;
                            }
                        }
                        break;

                    case KOR_STATUS.MIDDLE: // 자음 + 모음
                        {
                            disuniteChar = Disunite(waitString[waitString.Length - 1]);
                            if (Array.IndexOf(Vowels, keyIndex) != -1) // vowel check
                            {
                                if (mergeDict.ContainsKey(((char)SOUND_TABLE[disuniteChar.mid + 19], keyToTable[keyIndex])))
                                {
                                    disuniteChar.mid = mergeDict[((char)SOUND_TABLE[disuniteChar.mid + 19], keyToTable[keyIndex])] - 19;
                                    EditLastLetter(ref waitString, Unite(disuniteChar));
                                    waitStatus = KOR_STATUS.MIDDLE_VV;
                                }
                                else
                                {
                                    PushTempChar();
                                    waitString += key.keyString[keyIndex];
                                    waitStatus = KOR_STATUS.FIRST_V;
                                }
                            }
                            else
                            {
                                if ((keyIndex < 10 && keyIndex % 2 == 1) && !(keyIndex == 7 || keyIndex == 9))
                                {
                                    PushTempChar();
                                    waitString += key.keyString[keyIndex];
                                    waitStatus = KOR_STATUS.FIRST_C;
                                }
                                else
                                {
                                    //thinking
                                    disuniteChar.last = endTable[SOUND_TABLE[keyToTable[keyIndex]]];
                                    EditLastLetter(ref waitString, Unite(disuniteChar));
                                    waitStatus = KOR_STATUS.END;
                                }
                            }
                        }
                        break;

                    case KOR_STATUS.MIDDLE_VV: // 자음 + 모음 + 모음
                        {
                            disuniteChar = Disunite(waitString[waitString.Length - 1]);
                            if (Array.IndexOf(Vowels, keyIndex) != -1) // vowel check
                            {
                                PushTempChar();
                                waitString += key.keyString[keyIndex];
                                waitStatus = KOR_STATUS.FIRST_V;
                            }
                            else
                            {
                                if ((keyIndex < 10 && keyIndex % 2 == 1) && !(keyIndex == 7 || keyIndex == 9))
                                {
                                    PushTempChar();
                                    waitString += key.keyString[keyIndex];
                                    waitStatus = KOR_STATUS.FIRST_C;
                                }
                                else
                                {
                                    //thinking
                                    disuniteChar.last = endTable[SOUND_TABLE[keyToTable[keyIndex]]];
                                    EditLastLetter(ref waitString, Unite(disuniteChar));
                                    waitStatus = KOR_STATUS.END;
                                }
                            }
                        }
                        break;

                    case KOR_STATUS.END: // 자음 + 모음 + 자음
                        {
                            disuniteChar = Disunite(waitString[waitString.Length - 1]);
                            if (Array.IndexOf(Vowels, keyIndex) != -1) // vowel check
                            {
                                (int first, int mid, int last) tempChar = (0, 0, 0);
                                tempChar.mid = keyToTable[keyIndex] - 19;
                                tempChar.first = SOUND_TABLE.IndexOf(SOUND_TABLE[disuniteChar.last + 40]);
                                disuniteChar.last = 0;
                                EditLastLetter(ref waitString, Unite(disuniteChar));

                                PushTempChar();
                                waitString += Unite(tempChar);
                                waitStatus = KOR_STATUS.MIDDLE;
                            }
                            else
                            {
                                if (mergeDict.ContainsKey(((char)SOUND_TABLE[disuniteChar.last + 40], endTable[SOUND_TABLE[keyToTable[keyIndex]]])))
                                {
                                    disuniteChar.last = mergeDict[((char)SOUND_TABLE[disuniteChar.last + 40], endTable[SOUND_TABLE[keyToTable[keyIndex]]])];
                                    EditLastLetter(ref waitString, Unite(disuniteChar));
                                    waitStatus = KOR_STATUS.END_CC;
                                }
                                else
                                {
                                    PushTempChar();
                                    waitString += key.keyString[keyIndex];
                                    waitStatus = KOR_STATUS.FIRST_C;
                                }
                            }
                        }
                        break;

                    case KOR_STATUS.END_CC: // 자음 + 모음 + 자음 + 자음
                        {
                            disuniteChar = Disunite(waitString[waitString.Length - 1]);
                            if (Array.IndexOf(Vowels, keyIndex) != -1) // vowel check
                            {
                                (int first, int mid, int last) tempChar = (0, 0, 0);
                                tempChar.mid = keyToTable[keyIndex] - 19;
                                (int goLast, int goFirst) = UnMergeDict[disuniteChar.last];
                                tempChar.first = SOUND_TABLE.IndexOf(SOUND_TABLE[goFirst + 40]);
                                disuniteChar.last = goLast;
                                EditLastLetter(ref waitString, Unite(disuniteChar));

                                PushTempChar();
                                waitString += Unite(tempChar);
                                waitStatus = KOR_STATUS.MIDDLE;
                            }
                            else
                            {
                                PushTempChar();
                                waitString += key.keyString[keyIndex];
                                waitStatus = KOR_STATUS.FIRST_C;
                            }
                        }
                        break;
                }
                #endregion
                shift = false;
            }
            else if (keyIndex >= 100 && keyIndex < 110)
            {
                PushTempChar();
                text += (keyIndex - 100).ToString();
            }
            else if (keyIndex == -3)
            {
                (int first, int mid, int last) disuniteChar;
                #region waitStatus Switch
                switch (waitStatus)
                {
                    case KOR_STATUS.FIRST_C:
                        {
                            waitString = "";
                            PushTempChar();
                        }
                        break;

                    case KOR_STATUS.FIRST_V:
                        {
                            waitString = "";
                            PushTempChar();
                        }
                        break;

                    case KOR_STATUS.FIRST_VV:
                        {
                            (int tmp, _) = UnMergeDict[SOUND_TABLE.IndexOf(waitString[waitString.Length - 1])];
                            EditLastLetter(ref waitString, SOUND_TABLE[tmp]);
                            waitStatus = KOR_STATUS.FIRST_V;
                        }
                        break;

                    case KOR_STATUS.MIDDLE:
                        {
                            disuniteChar = Disunite(waitString[waitString.Length - 1]);
                            EditLastLetter(ref waitString, SOUND_TABLE[disuniteChar.first]);
                            waitStatus = KOR_STATUS.FIRST_C;
                        }
                        break;

                    case KOR_STATUS.MIDDLE_VV:
                        {
                            disuniteChar = Disunite(waitString[waitString.Length - 1]);
                            (int tmp, _) = UnMergeDict[disuniteChar.mid + 19];
                            disuniteChar.mid = tmp - 19;
                            EditLastLetter(ref waitString, Unite(disuniteChar));
                            waitStatus = KOR_STATUS.MIDDLE;
                        }
                        break;

                    case KOR_STATUS.END:
                        {
                            disuniteChar = Disunite(waitString[waitString.Length - 1]);
                            disuniteChar.last = 0;
                            EditLastLetter(ref waitString, Unite(disuniteChar));
                            if (Array.IndexOf(Vowels_VV, disuniteChar.mid + 19) != -1)
                            {
                                waitStatus = KOR_STATUS.MIDDLE_VV;
                            }
                            else
                            {
                                waitStatus = KOR_STATUS.MIDDLE;
                            }
                        }
                        break;


                    case KOR_STATUS.END_CC:
                        {
                            disuniteChar = Disunite(waitString[waitString.Length - 1]);
                            (int tmp, _) = UnMergeDict[disuniteChar.last];
                            disuniteChar.last = tmp;
                            EditLastLetter(ref waitString, Unite(disuniteChar));
                            waitStatus = KOR_STATUS.END;
                        }
                        break;
                }
                #endregion
                shift = false;
            }
            else if (keyIndex == -2) //shift
            {
                shift = !shift;
                UIChanged = true;
            }
            else if (keyIndex == -4) //space
            {
                PushTempChar();
                text += " ";
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
            waitStatus = KOR_STATUS.NONE;
        }

        public override void PushTempChar()
        {
            text += waitString;
            waitString = "";
            waitStatus = KOR_STATUS.NONE;
        }

        public override bool CanDeleteText()
        {
            return waitString.Length == 0;
        }

        #region Unite, Disunite, EditLastLetter func
        (int, int, int) Disunite(char KorChar)
        {
            if (KorChar >= 0xAC00 && KorChar <= 0xD7A3)
            {
                return ((KorChar - basic) / (21 * 28), (KorChar - basic) % (21 * 28) / 28, (KorChar - basic) % 28);
            }
            return (-1, -1, -1);
        }
        
        char Unite(int first, int mid, int end)
        {
            if (first >= 0 && mid >= 0)
            {
                return (char)(basic + end + first * (21 * 28) + mid * 28);
            }
            else
            {
                return ' ';
            }
        }

        char Unite(int first, int mid)
        {
            return Unite(first, mid, 0);
        }

        char Unite((int first, int mid, int last) charSet)
        {
            return Unite(charSet.first, charSet.mid, charSet.last);
        }

        void EditLastLetter(ref string str, string addStr)
        {
            if (str.Length > 0)
            {
                str = waitString.Remove(waitString.Length - 1) + addStr;
            }
            else
            {
                str += addStr;
            }
        }
        void EditLastLetter(ref string str, char addStr)
        {
            EditLastLetter(ref str, addStr.ToString());
        }
        #endregion

    }

}
