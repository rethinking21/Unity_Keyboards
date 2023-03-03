"""
use python 3.9+ (maybe)
made by rethinking21

database from : http://www.edrdg.org/wiki/index.php/KANJIDIC_Project
and http://nihongo.monash.edu/kanjidic2/index.html
"""
import xml.etree.ElementTree as elemTree
import json
from collections import OrderedDict

RAWDATA_PATH = r'./rawdata/kanjidic2.xml'
JSON_PATH = r'./result/kanji_sorted.json'

# region hiragana and katakana
hiragana: dict = {
    'あ': "a", 'い': "i", 'う': "u", 'え': "e", 'お': "o",
    'か': "ka", 'き': "ki", 'く': "ku", 'け': "ke", 'こ': "ko",
    'さ': "sa", 'し': "si", 'す': "su", 'せ': "se", 'そ': "so",
    'た': "ta", 'ち': ["ti", "chi"], 'つ': ["tu", "tsu"], 'て': "te", 'と': "to",
    'な': "na", 'に': "ni", 'ぬ': "nu", 'ね': "ne", 'の': "no",
    'は': "ha", 'ひ': "hi", 'ふ': "hu", 'へ': "he", 'ほ': "ho",
    'ま': "ma", 'み': "mi", 'む': "mu", 'め': "me", 'も': "mo",
    'ら': "ra", 'り': "ri", 'る': "ru", 'れ': "re", 'ろ': "ro",
    'や': "ya", 'ゆ': "yu", 'よ': "yo",
    'わ': "wa", 'を': "wo", 'ん': "n",  # thinking
    'ゐ': "wi", 'ゑ': "we",

    'が': "ga", 'ぎ': "gi", 'ぐ': "gu", 'げ': "ge", 'ご': "go",
    'ざ': "za", 'じ': ["zi", "ji"], 'ず': "zu", 'ぜ': "ze", 'ぞ': "zo",
    'だ': "da", 'ぢ': "di", 'づ': "du", 'で': "de", 'ど': "do",
    'ば': "ba", 'び': "bi", 'ぶ': "bu", 'べ': "be", 'ぼ': "bo",

    'ぱ': "pa", 'ぴ': "pi", 'ぷ': "pu", 'ぺ': "pe", 'ぽ': "po",

    'ょ': "yo", 'ゅ': "yu", 'ゃ': "ya",
}
hiragana_yo: dict = {
    "きゃ": "kya", "きゅ": "kyu", "きょ": "kyo",
    "しゃ": ["sya", "sha"], "しゅ": ["syu", "shu"], "しょ": ["syo", "sho"],
    "ちゃ": ["tya", "cha"], "ちゅ": ["tyu", "chu"], "ちょ": ["tyo", "cho"],
    "にゃ": "nya", "にゅ": "nyu", "にょ": "nyo",
    "ひゃ": "hya", "ひゅ": "hyu", "ひょ": "hyo",
    "みゃ": "mya", "みゅ": "myu", "みょ": "myo",
    "りゃ": "rya", "りゅ": "ryu", "りょ": "ryo",
    "ぎゃ": "gya", "ぎゅ": "gyu", "ぎょ": "gyo",
    "じゃ": ["zya", "za", "ja", "jya"], "じゅ": ["zyu", "zu", "ju", "jyu"], "じょ": ["zyo", "zo", "jyo", "jo"],  # j
    "ぢゃ": "dya", "ぢゅ": "dyu", "ぢょ": "dyo",
    "びゃ": "bya", "びゅ": "byu", "びょ": "byo",
    "ぴゃ": "pya", "ぴゅ": "pyu", "ぴょ": "pyo",
}
hiragana_sokuon: str = 'っ'

katakana: dict = {
    'ア': "a", 'イ': "i", 'ウ': "u", 'エ': "e", 'オ': "o",
    'カ': "ka", 'キ': "ki", 'ク': "ku", 'ケ': "ke", 'コ': "ko",
    'サ': "sa", 'シ': "shi", 'ス': "su", 'セ': "se", 'ソ': "so",
    'タ': "ta", 'チ': ["ti", "chi"], 'ツ': ["tu", "tsu"], 'テ': "te", 'ト': "to",
    'ナ': "na", 'ニ': "ni", 'ヌ': "nu", 'ネ': "ne", 'ノ': "no",
    'ハ': "ha", 'ヒ': "hi", 'フ': "hu", 'ヘ': "he", 'ホ': "ho",
    'マ': "ma", 'ミ': "mi", 'ム': "mu", 'メ': "me", 'モ': "mo",
    'ラ': "ra", 'リ': "ri", 'ル': "ru", 'レ': "re", 'ロ': "ro",
    'ヤ': "ya", 'ユ': "yu", 'ヨ': "yo",
    'ワ': "wa", 'ヰ': "wi", 'ヱ': "we", 'ヲ': "wo",
    'ン': "n",

    'ガ': "ga", 'ギ': "gi", 'グ': "gu", 'ゲ': "ge", 'ゴ': "go",
    'ザ': "za", 'ジ': ["zi", "ji"], 'ズ': "zu", 'ゼ': "ze", 'ゾ': "zo",  # ji zi
    'ダ': "da", 'ヂ': "di", 'ヅ': "du", 'デ': "de", 'ド': "do",
    'バ': "ba", 'ビ': "bi", 'ブ': "bu", 'ベ': "be", 'ボ': "bo",
    'パ': "pa", 'ピ': "pi", 'プ': "pu", 'ペ': "pe", 'ポ': "po",

    'ュ': "yu", 'ョ': "yo", "ャ": "ya"
}
katakana_yo: dict = {
    "キャ": "kya", "キュ": "kyu", "キョ": "kyo",
    "シャ": ["sya", "sha"], "シュ": ["syu", "shu"], "ショ": ["syo", "sho"],
    "チャ": ["tya", "cha"], "チュ": ["tyu", "chu"], "チョ": ["tyo", "cho"],
    "ニャ": "nya", "ニュ": "nyu", "ニョ": "nyo",
    "ヒャ": "hya", "ヒュ": "hyu", "ヒョ": "hyo",
    "ミャ": "mya", "ミュ": "myu", "ミョ": "myo",
    "リャ": "rya", "リュ": "ryu", "リョ": "ryo",
    "ギャ": "gya", "ギュ": "gyu", "ギョ": "gyo",
    "ジャ": ["zya", "za", "ja", "jya"], "ジュ": ["zyu", "zu", "ju", "jyu"], "ジョ": ["zyo", "zo", "jyo", "jo"],
    "ビャ": "bya", "ビュ": "byu", "ビョ": "byo",
    "ピャ": "pya", "ピュ": "pyu", "ピョ": "pyo",
}
katakana_sokuon: str = 'ッ'


# endregion

# region func

def remove_char(word: str) -> str:
    return word.replace('-', '').replace('.', '')


def check_hiragana(word: str) -> bool:
    if len(word) == 0:
        return False
    elif word[0] == hiragana_sokuon or (word[0] in hiragana):
        return True
    else:
        return False


def check_katakana(word: str) -> bool:
    if len(word) == 0:
        return False
    elif word[0] == katakana_sokuon or (word[0] in katakana):
        return True
    else:
        return False


def add_word(words: list, adds) -> list[str]:
    if type(adds) is str:
        return [word + adds for word in words]
    elif type(adds) is list:
        return [word + add for word in words for add in adds]


def hiragana_to_eng(word: str) -> list[str]:
    output: list[str] = ['']
    while len(word) != 0:  # thinking
        if len(word) >= 2 and word[:1] in hiragana_yo:
            output = add_word(words=output, adds=hiragana_yo[word[:1]])
            word = word[2:]
        elif word[0] == hiragana_sokuon:
            if len(word) == 1:
                output = add_word(words=output, adds=hiragana['つ'])
            else:
                add = hiragana[word[1]]
                if type(add) is str:
                    output = add_word(words=output, adds=add[:1])
                elif type(add) is list:
                    output = add_word(words=output, adds=add[0][:1])
            word = word[1:]
        elif word[0] == 'ー':
            word = word[1:]
        elif word[0] in hiragana:
            output = add_word(words=output, adds=hiragana[word[0]])
            word = word[1:]
    return output


def katakana_to_eng(word: str) -> list[str]:
    output: list[str] = ['']
    while len(word) != 0:  # thinking
        if len(word) >= 2 and word[:1] in katakana_yo:
            output = add_word(words=output, adds=katakana_yo[word[:1]])
            word = word[2:]
        elif word[0] == katakana_sokuon:
            if len(word) == 1:
                output = add_word(words=output, adds=katakana['ツ'])
            else:
                add = katakana[word[1]]
                if type(add) is str:
                    output = add_word(words=output, adds=add[:1])
                elif type(add) is list:
                    output = add_word(words=output, adds=add[0][:1])
            word = word[1:]
        elif word[0] == 'ー':
            word = word[1:]
        elif word[0] in katakana:
            output = add_word(words=output, adds=katakana[word[0]])
            word = word[1:]
    return output

# endregion


print("$ parse raw data.")
xml_data = elemTree.parse(RAWDATA_PATH)
print("$ get root")
root = xml_data.getroot()

count = 0

info_list: list = []

print("$ parse character")
for character_xml in root.iter('character'):
    literal: str = character_xml.find('literal').text
    reading_meaning = character_xml.find('reading_meaning')
    rmgroup = reading_meaning.find('rmgroup') if reading_meaning else None

    freq: int = 0
    if character_xml.find('misc').find('freq') is not None:
        freq = int(character_xml.find('misc').find('freq').text)
    else:
        freq = 10000

    if rmgroup:
        eng_set = set()
        jpn_set = set()
        for reading in rmgroup.iter('reading'):
            if "ja_on" in reading.attrib.values() or "ja_kun" in reading.attrib.values():
                text = remove_char(reading.text)
                if check_katakana(text):
                    eng_set.update(katakana_to_eng(text))
                    jpn_set.add(text)
                elif check_hiragana(text):
                    eng_set.update(hiragana_to_eng(text))
                    jpn_set.add(text)
        if len(eng_set) != 0:
            info = OrderedDict()
            info['literal'] = literal
            info['freq'] = freq
            info['eng'] = list(eng_set)
            info['jpn'] = list(jpn_set)
            info_list.append(info)
    count += 1

print("$ sort by freq")
info_list.sort(key=lambda x: x['freq'])

print("$ make json data")
json_data = OrderedDict({"list": info_list})


with open(JSON_PATH, "w", encoding='utf-8') as json_file:
    json.dump(json_data, json_file, ensure_ascii=False, indent='\t')

print("done")
