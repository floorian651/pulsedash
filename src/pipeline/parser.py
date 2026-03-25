import json


def save_to_json(filename, key, tempo, beats, duree):
    data = {"key": key, "tempo": tempo, "beats": beats, "durée": duree}
    with open(filename, "w") as fic:
        json.dump(data, fic, indent=4)
