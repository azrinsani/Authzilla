namespace Authzilla
{
    public class Settings
    {
        public LoginSettings Login { get; set; } = new LoginSettings();
        public DatabaseSettings Database { get; set; } = new DatabaseSettings();
    }
    public class InternalClient
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string[] Scopes { get; set; }
    }
    public enum DatabaseType
    {
        SQLite,
        PostgreSQL,
        MSSQL
    }
    public class DatabaseSettings
    {
        public DatabaseType Type { get; set; } = DatabaseType.SQLite;
        public string ConnectionString { get; set; }
    }

    public class LoginSettings
    { 
        public bool MustLoginWithEmail { get; set; } = false;
        public bool ShowRememberMe { get; set; } = false;
        public bool DefaultRememberMeValue { get; set; } = false;
        public string RememberMeText { get; set; } = "Remember Me? (JOLA)";
        public string UsernameEmpty { get; set; } = "Username can't be empty! (JOLA)";
        public string EmailEmpty { get; set; } = "Email can't be empty! (JOLA)";
        public string EmailInvalid { get; set; } = "Invalid Email! (JOLA)";
        public string PasswordInvalid { get; set; } = "Invalid Password! (JOLA)";
        public string LoginFailed { get; set; } = "Login Failed! (JOLA)";
    }
}
