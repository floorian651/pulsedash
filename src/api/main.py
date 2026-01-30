from fastapi import FastAPI, UploadFile, File
from src.pipeline.main import main as run_pipeline

app = FastAPI()


@app.post("/analyze")
async def analyze(file: UploadFile = File(...)):
    content = await file.read()
    temp_path = "/tmp/input_audio.mp3"
    with open(temp_path, "wb") as f:
        f.write(content)

    # Appel de ton pipeline Python
    result = run_pipeline(temp_path)

    return {"result": result}


def analyze_audio(path: str):
    """
    Analyse un fichier audio et retourne un résultat exploitable par l'API.
    """
    # Ici tu appelles ton vrai pipeline
    # Exemple : result = run_full_pipeline(path)
    # Pour l'instant on met un placeholder
    result = {"message": f"Analyse terminée pour {path}"}

    return result
