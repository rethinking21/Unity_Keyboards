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
            "げこじすぇえぁあさざににづづちちだぢつて" // 0 - 19
            + "けけいいししぉぉぞぞででっったたびび" // 20 - 37
            + "せせぜぜずずそそばばぬぬぱぱ"; // 38 - 51
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
            FIRST_V, // 段失 (乞製)
            FIRST_C, // 段失 (切製)
            FIRST_VV, // 段失 (乞製 + 乞製)
            FIRST_CC, // 段失 (切製 + 切製) (食奄識 搾紫遂)
            MIDDLE, // 切製 + 乞製
            MIDDLE_VV, // 切製 + 乞製 + 乞製
            END, // 切製 + 乞製 + 切製
            END_CC // 切製 + 乞製 + 切製 + 切製
        }

        #region Kor Letter Data
        static readonly int basic = 0xAC00;
        static readonly int[] Vowels =   // vowel keycode
            {10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
            30, 31, 32, 33, 34, 35, 36, 37,
            46, 47, 48, 49, 50, 51};
        static readonly int[] Vowels_VV = 
            {28, 29, 30, 33, 34, 35, 38};

        // 段失, 掻失, 曽失 砺戚鷺.
        static readonly string SOUND_TABLE =
        // 段失 19切 0 ~ 18
        "ぁあいぇえぉけげこさざしじすずせぜそぞ" +
        // 掻失 21切 19 ~ 39
        "ただちぢっつづてでとどなにぬねのはばぱひび" +
        // 曽失 28切 40 ~ 67
        " ぁあぃいぅうぇぉおかがきぎくぐけげごさざしじずせぜそぞ";

        readonly Dictionary<int, int> keyToTable = new Dictionary<int, int>()
        {
            {0, 7}, //げ
            {1, 8}, //こ
            {2, 12}, //じ
            {3, 13}, //す
            {4, 3}, //ぇ
            {5, 4}, //え
            {6, 0}, //ぁ
            {7, 1}, //あ
            {8, 9}, //さ
            {9, 10}, //ざ
            
            {10, 31}, //に
            {11, 31}, //に
            {12, 25}, //づ
            {13, 25}, //づ
            {14, 21}, //ち
            {15, 21}, //ち
            {16, 20}, //だ
            {17, 22}, //ぢ
            {18, 24}, //つ
            {19, 26}, //て
             
            {20, 6}, //け
            {21, 6}, //け
            {22, 2}, //い
            {23, 2}, //い
            {24, 11}, //し
            {25, 11}, //し
            {26, 5}, //ぉ
            {27, 5}, //ぉ
            {28, 18}, //ぞ
            {29, 18}, //ぞ
             
            {30, 27}, //で
            {31, 27}, //で
            {32, 23}, //っ
            {33, 23}, //っ
            {34, 19}, //た
            {35, 19}, //た
            {36, 39}, //び
            {37, 39}, //び
            {38, 15}, //せ
            {39, 15}, //せ
             
            {40, 16}, //ぜ
            {41, 16}, //ぜ
            {42, 14}, //ず
            {43, 14}, //ず
            {44, 17}, //そ
            {45, 17}, //そ
            {46, 36}, //ば
            {47, 36}, //ば
            {48, 32}, //ぬ
            {49, 32}, //ぬ
             
            {50, 37}, //ぱ
            {51, 37}, //ぱ
        };
        readonly Dictionary<char, int> endTable = new Dictionary<char, int>()
        {
            {' ', 0},
            {'ぁ', 1},
            {'あ', 2},
            {'ぃ', 3},
            {'い', 4},
            {'ぅ', 5},
            {'う', 6},
            {'ぇ', 7},
            {'ぉ', 8},
            {'お', 9},

            {'か', 10},
            {'が', 11},
            {'き', 12},
            {'ぎ', 13},
            {'く', 14},
            {'ぐ', 15},
            {'け', 16},
            {'げ', 17},
            {'ご', 18},
            {'さ', 19},

            {'ざ', 20},
            {'し', 21},
            {'じ', 22},
            {'ず', 23},
            {'せ', 24},
            {'ぜ', 25},
            {'そ', 26},
            {'ぞ', 27},

        };
        readonly Dictionary<(char, int), int> mergeDict = new Dictionary<(char, int), int>() // (char, key) Table
        {
            { ('で', 19), 28 }, // と
            { ('で', 20), 29 }, // ど
            { ('で', 39), 30 }, // な
            { ('ぬ', 23), 33 }, // ね
            { ('ぬ', 24), 34 }, // の
            { ('ぬ', 39), 35 }, // は
            { ('ぱ', 39), 38 }, // ひ

            { ('ぁ', 19), 3 }, // ぃ
            { ('い', 22), 5 }, // ぅ
            { ('い', 27), 6 }, // う
            { ('ぉ', 1), 9 }, // お
            { ('ぉ', 16), 10 }, // か
            { ('ぉ', 17), 11 }, // が
            { ('ぉ', 19), 12 }, // き
            { ('ぉ', 25), 13 }, // ぎ
            { ('ぉ', 26), 14 }, // く
            { ('ぉ', 27), 15 }, // ぐ
            { ('げ', 19), 18 }, // ご
        };
        readonly Dictionary<int, (int, int)> UnMergeDict = new Dictionary<int, (int, int)>()
        {
            {28, (27, 19)}, // と
            {29, (27, 20)}, // ど
            {30, (27, 39)}, // な
            {33, (32, 23)}, // ね
            {34, (32, 24)}, // の
            {35, (32, 39)}, // は
            {38, (37, 39)}, // ひ

            {3, (1, 19)}, // ぃ
            {5, (4, 22)}, // ぅ
            {6, (4, 27)}, // う
            {9 , (8, 1)}, // お
            {10, (8, 16)}, // か
            {11, (8, 17)}, // が
            {12, (8, 19)}, // き
            {13, (8, 25)}, // ぎ
            {14, (8, 26)}, // く
            {15, (8, 27)}, // ぐ
            {18, (8, 19)}, // ご
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
                    case KOR_STATUS.NONE: // 段奄雌殿
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

                    case KOR_STATUS.FIRST_V: // 段失 (乞製)
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

                    case KOR_STATUS.FIRST_C: // 段失 (切製)
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

                    case KOR_STATUS.FIRST_VV: // 段失 (乞製 + 乞製)
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

                    case KOR_STATUS.MIDDLE: // 切製 + 乞製
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

                    case KOR_STATUS.MIDDLE_VV: // 切製 + 乞製 + 乞製
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

                    case KOR_STATUS.END: // 切製 + 乞製 + 切製
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

                    case KOR_STATUS.END_CC: // 切製 + 乞製 + 切製 + 切製
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
