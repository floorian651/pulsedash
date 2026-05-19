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
public class RefreshResponse{ public string access_token; }