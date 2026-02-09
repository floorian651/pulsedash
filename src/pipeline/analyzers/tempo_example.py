"""Tempo and rhythm analysis."""

import numpy as np
import librosa


def analyze_tempo(y: np.ndarray, sr: int) -> dict:
    """
    Analyze tempo (BPM) from audio signal.

    Args:
        y: Audio time series
        sr: Sampling rate

    Returns:
        Dictionary containing tempo in BPM
    """
    tempo, _ = librosa.beat.beat_track(y=y, sr=sr)
    tempo = tempo.item() if hasattr(tempo, "item") else float(tempo)

    return {"tempo": tempo, "unit": "BPM"}
