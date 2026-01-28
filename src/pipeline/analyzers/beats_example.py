"""Beat detection and strength analysis."""

import numpy as np
import librosa


def analyze_beats(y: np.ndarray, sr: int) -> dict:
    """
    Analyze beats and their strengths from audio signal.

    Args:
        y: Audio time series
        sr: Sampling rate

    Returns:
        Dictionary containing:
            - tempo: Beats per minute
            - beat_frames: Frame indices of detected beats
            - beat_times: Time values of detected beats (in seconds)
            - beat_strengths: Strength/power of each beat
            - beats_data: Combined timing and power information
    """
    # Detect tempo and beat frames
    tempo, beat_frames = librosa.beat.beat_track(y=y, sr=sr)
    tempo = tempo.item() if hasattr(tempo, "item") else float(tempo)

    # Convert beat frames to time
    beat_times = librosa.frames_to_time(beat_frames, sr=sr)

    # Calculate beat strength using onset strength
    onset_env = librosa.onset.onset_strength(y=y, sr=sr)
    beat_strengths = onset_env[beat_frames]

    # Create combined beat data with timing and power
    beats_data = [
        {"timing": float(t), "puissance": float(p)}
        for t, p in zip(beat_times, beat_strengths)
    ]

    return {
        "tempo": tempo,
        "beat_frames": beat_frames,
        "beat_times": beat_times,
        "beat_strengths": beat_strengths,
        "beats_data": beats_data,
    }
