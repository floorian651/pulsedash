"""JSON export functionality for audio analysis results."""

import json
from pathlib import Path
from typing import Any, Dict, Union


def save_analysis_to_json(
    filepath: Union[str, Path], data: Dict[str, Any], indent: int = 4
) -> None:
    """
    Save audio analysis data to JSON file.

    Args:
        filepath: Path where to save the JSON file
        data: Dictionary containing analysis results
        indent: JSON indentation level (default: 4)
    """
    filepath = Path(filepath)
    filepath.parent.mkdir(parents=True, exist_ok=True)

    with open(filepath, "w") as f:
        json.dump(data, f, indent=indent)


def save_rhythm_analysis(
    filepath: Union[str, Path], key: str, tempo: float, beats: list, duration: float
) -> None:
    """
    Save rhythm analysis results (compatible with analyse-audio format).

    Args:
        filepath: Path where to save the JSON file
        key: Musical key/tonality
        tempo: Beats per minute
        beats: List of beat dictionaries with timing and puissance
        duration: Total audio duration in seconds
    """
    data = {"key": key, "tempo": tempo, "beats": beats, "durée": duration}
    save_analysis_to_json(filepath, data)


def load_json_analysis(filepath: Union[str, Path]) -> Dict[str, Any]:
    """
    Load audio analysis data from JSON file.

    Args:
        filepath: Path to the JSON file

    Returns:
        Dictionary containing analysis results
    """
    with open(filepath, "r") as f:
        return json.load(f)
