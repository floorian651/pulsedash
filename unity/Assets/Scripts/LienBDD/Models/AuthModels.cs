[System.Serializable]
public class RegisterRequest { public string email; public string password; public string username; }

[System.Serializable]
public class RegisterResponse { public string access_token; public string refresh_token; }

[System.Serializable]
public class LoginRequest { public string email; public string password; }

[System.Serializable]
public class LoginResponse { public string access_token; public string refresh_token; }

[System.Serializable]
public class UserProfile { public int id; public string username; public string email; public bool is_active; }

[System.Serializable]
public class RefreshRequest { public string refresh_token; } 

[System.Serializable]
public class RefreshResponse { public string access_token; }

[System.Serializable]
public class StartSessionRequest { public string music_title; }

[System.Serializable]
public class StartSessionResponse { public string id; public string status; public string started_at; }

[System.Serializable]
public class EndSessionRequest { public float final_score; public bool abandoned; }

[System.Serializable]
public class EndSessionResponse { public string id; public string status; }

[System.Serializable]
public class CreatePlaylistRequest { public string name; public string description; }

[System.Serializable]
public class PlaylistData
{
    public string name;
    public string description;
    public string created_at;
    public TrackData[] tracks;
}

[System.Serializable]
public class TrackData { public int id; public string music_title; public int position; }

[System.Serializable]
public class AddTrackRequest { public string playlist_name; public string music_title; }