from typing import Dict, List

from fastapi import WebSocket


class WebSocketManager:
    def __init__(self):
        self.connections: Dict[str, List[WebSocket]] = {}

    async def connect(self, job_id: str, websocket: WebSocket):
        await websocket.accept()
        self.connections.setdefault(job_id, []).append(websocket)

    def disconnect(self, job_id: str, websocket: WebSocket):
        if job_id in self.connections:
            self.connections[job_id].remove(websocket)

    async def broadcast(self, job_id: str, message: str):
        if job_id in self.connections:
            for ws in self.connections[job_id]:
                await ws.send_text(message)
