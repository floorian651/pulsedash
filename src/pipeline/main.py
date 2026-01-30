import os
import librosa
import numpy as np

from .processors.parser import save_to_json


def main(fichier_audio: str = "assets/Darude.mp3"):
    y, sr = librosa.load(fichier_audio)

    # Tempo (pulsation) et rythme
    tempo, beat_frames = librosa.beat.beat_track(y=y, sr=sr)
    tempo = tempo.item() if hasattr(tempo, "item") else float(tempo)
    beats = librosa.frames_to_time(beat_frames, sr=sr)

    # Calculer la puissance (intensité) de chaque beat
    onset_env = librosa.onset.onset_strength(y=y, sr=sr)
    beat_strengths = onset_env[beat_frames]

    # Extraire le chroma (Graphe des harmoniques par note)
    chroma = librosa.feature.chroma_cqt(y=y, sr=sr)
    # Moyenner sur le temps pour obtenir le profil de tonalité
    chroma_mean = np.mean(chroma, axis=1)
    # Trouver la note dominante
    notes = ["C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"]
    key_index = np.argmax(chroma_mean)
    key = notes[key_index]

    os.makedirs("resultat", exist_ok=True)

    # Créer une liste combinant temps et puissance
    beats_avec_puissance = [
        {"timing": float(t), "puissance": float(p)}
        for t, p in zip(beats, beat_strengths)
    ]

    resultat_path = "resultat/analyse_rythme.json"
    duree = librosa.get_duration(y=y, sr=sr)

    save_to_json(
        resultat_path,
        key,
        tempo,
        beats_avec_puissance,
        duree,
    )

    return {
        "resultat_path": resultat_path,
        "key": key,
        "tempo": tempo,
        "beats": beats_avec_puissance,
        "duree": duree,
    }


if __name__ == "__main__":
    result = main()
    print("\nDonnées sauvegardées dans:", result["resultat_path"])
