# WAVR - Audio Rhythm Analysis Engine

A full-stack audio analysis and rhythm detection system combining Python signal processing with Unity game integration.

## Project Overview

WAVR is an audio analysis engine that extracts musical features from audio files and exports them for real-time integration in Unity games. The pipeline analyzes rhythm, tempo, energy, and musical structure to drive interactive gameplay.

### Key Features

- **Audio Feature Extraction**: Tempo/BPM detection, beat tracking, energy analysis, spectral analysis
- **Rhythm Processing**: Beat quantization, smoothing, threshold detection
- **Multi-format Support**: Works with MP3, WAV, and other audio formats
- **FastAPI Backend**: RESTful API for file uploads and analysis requests
- **Asynchronous Processing**: Celery workers for distributed audio processing
- **Unity Integration**: Exports analysis results as JSON for Unity `StreamingAssets`
- **Docker Setup**: Complete dev container environment with Python and .NET SDK

## Documentation

Full documentation available at: [https://floorian651.github.io/wavr/](https://floorian651.github.io/wavr/)

## Quick Start

### Prerequisites

- Docker & Docker Compose
- VS Code with Dev Containers extension (or local Python 3.9+)

### Development Setup

```bash
# Clone the repository
git clone https://github.com/floorian651/wavr.git

# Open in dev container (VS Code)
# Command Palette > Dev Containers: Reopen in Container
```

### Running the Pipeline

```bash
# Run audio analysis on a file
python -m src.pipeline.main

# Or use FastAPI server
uvicorn src.api.main:app --reload

# Check results
cat resultat/analyse_rythme.json
```

## Technology Stack

- **Backend**: Python 3, FastAPI, Celery
- **Audio Processing**: librosa, numpy, scipy
- **Client**: Unity 2022+, C#
- **Infrastructure**: Docker, Docker Compose
- **Database**: PostgreSQL, Redis
- **Storage**: MinIO
- **CI/CD**: GitHub Actions

## Testing

```bash
# Run tests with coverage
pytest tests/

# View coverage report
coverage report
```

## Docker & Dev Container

The project includes a pre-configured dev container.
