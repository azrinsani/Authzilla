
dotnet ef migrations add Initial -c AppDbContext -o Migrations\AppIdentityDb --project authzilla.postgresql -- --provider PostgreSQL
dotnet ef migrations add Initial -c PersistedGrantDbContext -o Migrations\PersistedGrantDb --project authzilla.postgresql -- --provider PostgreSQL
dotnet ef migrations add Initial -c AppDbContext -o Migrations\AppIdentityDb --project authzilla.sqlite -- --provider SQLite
dotnet ef migrations add Initial -c PersistedGrantDbContext -o Migrations\PersistedGrantDb --project authzilla.sqlite -- --provider SQLite