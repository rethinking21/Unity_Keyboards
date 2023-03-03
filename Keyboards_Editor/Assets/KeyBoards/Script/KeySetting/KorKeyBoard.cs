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
            "���������������������ˤˤŤŤ������¤Ĥ�" // 0 - 19
            + "���������������������ǤǤää����Ӥ�" // 20 - 37
            + "�����������������ФФ̤̤Ѥ�"; // 38 - 51
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
            FIRST_V, // �ʼ� (����)
            FIRST_C, // �ʼ� (����)
            FIRST_VV, // �ʼ� (���� + ����)
            FIRST_CC, // �ʼ� (���� + ����) (���⼱ ����)
            MIDDLE, // ���� + ����
            MIDDLE_VV, // ���� + ���� + ����
            END, // ���� + ���� + ����
            END_CC // ���� + ���� + ���� + ����
        }

        #region Kor Letter Data
        static readonly int basic = 0xAC00;
        static readonly int[] Vowels =   // vowel keycode
            {10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
            30, 31, 32, 33, 34, 35, 36, 37,
            46, 47, 48, 49, 50, 51};
        static readonly int[] Vowels_VV = 
            {28, 29, 30, 33, 34, 35, 38};

        // �ʼ�, �߼�, ���� ���̺�.
        static readonly string SOUND_TABLE =
        // �ʼ� 19�� 0 ~ 18
        "��������������������������������������" +
        // �߼� 21�� 19 ~ 39
        "�������¤äĤŤƤǤȤɤʤˤ̤ͤΤϤФѤҤ�" +
        // ���� 28�� 40 ~ 67
        " ������������������������������������������������������";

        readonly Dictionary<int, int> keyToTable = new Dictionary<int, int>()
        {
            {0, 7}, //��
            {1, 8}, //��
            {2, 12}, //��
            {3, 13}, //��
            {4, 3}, //��
            {5, 4}, //��
            {6, 0}, //��
            {7, 1}, //��
            {8, 9}, //��
            {9, 10}, //��
            
            {10, 31}, //��
            {11, 31}, //��
            {12, 25}, //��
            {13, 25}, //��
            {14, 21}, //��
            {15, 21}, //��
            {16, 20}, //��
            {17, 22}, //��
            {18, 24}, //��
            {19, 26}, //��
             
            {20, 6}, //��
            {21, 6}, //��
            {22, 2}, //��
            {23, 2}, //��
            {24, 11}, //��
            {25, 11}, //��
            {26, 5}, //��
            {27, 5}, //��
            {28, 18}, //��
            {29, 18}, //��
             
            {30, 27}, //��
            {31, 27}, //��
            {32, 23}, //��
            {33, 23}, //��
            {34, 19}, //��
            {35, 19}, //��
            {36, 39}, //��
            {37, 39}, //��
            {38, 15}, //��
            {39, 15}, //��
             
            {40, 16}, //��
            {41, 16}, //��
            {42, 14}, //��
            {43, 14}, //��
            {44, 17}, //��
            {45, 17}, //��
            {46, 36}, //��
            {47, 36}, //��
            {48, 32}, //��
            {49, 32}, //��
             
            {50, 37}, //��
            {51, 37}, //��
        };
        readonly Dictionary<char, int> endTable = new Dictionary<char, int>()
        {
            {' ', 0},
            {'��', 1},
            {'��', 2},
            {'��', 3},
            {'��', 4},
            {'��', 5},
            {'��', 6},
            {'��', 7},
            {'��', 8},
            {'��', 9},

            {'��', 10},
            {'��', 11},
            {'��', 12},
            {'��', 13},
            {'��', 14},
            {'��', 15},
            {'��', 16},
            {'��', 17},
            {'��', 18},
            {'��', 19},

            {'��', 20},
            {'��', 21},
            {'��', 22},
            {'��', 23},
            {'��', 24},
            {'��', 25},
            {'��', 26},
            {'��', 27},

        };
        readonly Dictionary<(char, int), int> mergeDict = new Dictionary<(char, int), int>() // (char, key) Table
        {
            { ('��', 19), 28 }, // ��
            { ('��', 20), 29 }, // ��
            { ('��', 39), 30 }, // ��
            { ('��', 23), 33 }, // ��
            { ('��', 24), 34 }, // ��
            { ('��', 39), 35 }, // ��
            { ('��', 39), 38 }, // ��

            { ('��', 19), 3 }, // ��
            { ('��', 22), 5 }, // ��
            { ('��', 27), 6 }, // ��
            { ('��', 1), 9 }, // ��
            { ('��', 16), 10 }, // ��
            { ('��', 17), 11 }, // ��
            { ('��', 19), 12 }, // ��
            { ('��', 25), 13 }, // ��
            { ('��', 26), 14 }, // ��
            { ('��', 27), 15 }, // ��
            { ('��', 19), 18 }, // ��
        };
        readonly Dictionary<int, (int, int)> UnMergeDict = new Dictionary<int, (int, int)>()
        {
            {28, (27, 19)}, // ��
            {29, (27, 20)}, // ��
            {30, (27, 39)}, // ��
            {33, (32, 23)}, // ��
            {34, (32, 24)}, // ��
            {35, (32, 39)}, // ��
            {38, (37, 39)}, // ��

            {3, (1, 19)}, // ��
            {5, (4, 22)}, // ��
            {6, (4, 27)}, // ��
            {9 , (8, 1)}, // ��
            {10, (8, 16)}, // ��
            {11, (8, 17)}, // ��
            {12, (8, 19)}, // ��
            {13, (8, 25)}, // ��
            {14, (8, 26)}, // ��
            {15, (8, 27)}, // ��
            {18, (8, 19)}, // ��
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
                    case KOR_STATUS.NONE: // �ʱ����
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

                    case KOR_STATUS.FIRST_V: // �ʼ� (����)
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

                    case KOR_STATUS.FIRST_C: // �ʼ� (����)
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

                    case KOR_STATUS.FIRST_VV: // �ʼ� (���� + ����)
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

                    case KOR_STATUS.MIDDLE: // ���� + ����
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

                    case KOR_STATUS.MIDDLE_VV: // ���� + ���� + ����
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

                    case KOR_STATUS.END: // ���� + ���� + ����
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

                    case KOR_STATUS.END_CC: // ���� + ���� + ���� + ����
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
