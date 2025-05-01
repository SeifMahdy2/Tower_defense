using System;

// Base response class for success/error handling
[Serializable]
public class BaseResponse
{
    public bool success;
    public string error;
    public string message;
}

// Login response
[Serializable]
public class LoginResponse : BaseResponse
{
    public UserData user;
}

// User data structure
[Serializable]
public class UserData
{
    public int id;
    public string username;
    public string email;
    public int levels_completed;
    public LevelData levels;
}

// Level unlock data
[Serializable]
public class LevelData
{
    public bool level1 = true; // Level 1 is always unlocked
    public bool level2 = false;
}

// Progress update request
[Serializable]
public class ProgressUpdateRequest
{
    public string username;
    public bool level1;
    public bool level2;
}

// User registration data
[Serializable]
public class RegistrationRequest
{
    public string username;
    public string email;
    public string password;
}

// Login request
[Serializable]
public class LoginRequest
{
    public string username;
    public string password;
}

// Progress response
[Serializable]
public class ProgressResponse : BaseResponse
{
    public LevelData progress;
} 