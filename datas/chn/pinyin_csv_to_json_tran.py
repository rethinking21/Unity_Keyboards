"""
use python 3.9+ (maybe)
made by rethinking21

database from : https://lingua.mtsu.edu/chinese-computing/statistics/char/list.php?Which=MO
"""
import json, csv
from collections import OrderedDict

RAWDATA_PATH = r'./rawdata/chn_dataset_traditional.tsv'
JSON_PATH = r'./result/pinyin_sorted_traditional.json'

special_u: str = 'ü'

# region func


def split_eng(text: str) -> list[str]:
    return [word.replace('u:', special_u) for word in text.split('/')]

# endregion


info_list: list = []
with open(RAWDATA_PATH, encoding="utf-8") as file:
    tsv_read = csv.reader(file, delimiter='\t')

    for line in tsv_read:
        info = OrderedDict()
        info['literal'] = line[1]
        info['freq'] = int(line[2])
        info['eng'] = split_eng(line[4])
        info_list.append(info)

print("$ make json data")
json_data = OrderedDict({"list": info_list})

with open(JSON_PATH, "w", encoding='utf-8') as json_file:
    json.dump(json_data, json_file, ensure_ascii=False, indent='\t')

print("done")
