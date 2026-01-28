"""Spectral and harmonic structure analysis including key detection."""

import numpy as np
import librosa


def analyze_key(y: np.ndarray, sr: int) -> dict:
    """
    Detect the musical key/tonality from audio signal using chroma features.

    Args:
        y: Audio time series
        sr: Sampling rate

    Returns:
        Dictionary containing:
            - key: The detected musical key (note name)
            - confidence: Mean chroma value for detected key (0-1)
            - chroma_profile: Full chroma feature profile
    """
    # Extract chroma features (harmonic content per note)
    chroma = librosa.feature.chroma_cqt(y=y, sr=sr)

    # Average over time to get overall tonality profile
    chroma_mean = np.mean(chroma, axis=1)

    # Musical note names (12 semitones)
    notes = ["C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"]

    # Find dominant note
    key_index = np.argmax(chroma_mean)
    key = notes[key_index]
    confidence = float(chroma_mean[key_index])

    return {
        "key": key,
        "confidence": confidence,
        "chroma_profile": chroma_mean.tolist(),
    }


def analyze_spectrum(y: np.ndarray, sr: int) -> dict:
    """
    Analyze spectral characteristics of audio signal.

    Args:
        y: Audio time series
        sr: Sampling rate

    Returns:
        Dictionary containing spectral analysis results
    """
    # Compute STFT
    D = librosa.stft(y)
    magnitude = np.abs(D)

    # Compute power spectrogram
    S = librosa.feature.melspectrogram(y=y, sr=sr)

    return {
        "magnitude_mean": float(np.mean(magnitude)),
        "magnitude_std": float(np.std(magnitude)),
        "mel_spectrogram_shape": S.shape,
    }
