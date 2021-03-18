
dotnet ef migrations add Initial -c AppDbContext -o Migrations\AppDb --project authzilla.postgresql -- --provider PostgreSQL
dotnet ef migrations add Initial -c PersistedGrantDbContext -o Migrations\PersistedGrantDb --project authzilla.postgresql -- --provider PostgreSQL
dotnet ef migrations add Initial -c AppDbContext -o Migrations\AppDb --project authzilla.sqlite -- --provider SQLite
dotnet ef migrations add Initial -c PersistedGrantDbContext -o Migrations\PersistedGrantDb --project authzilla.sqlite -- --provider SQLite
dotnet ef migrations add Initial -c AppDbContext -o Migrations\AppDb --project authzilla.mssql -- --provider MSSQL
dotnet ef migrations add Initial -c PersistedGrantDbContext -o Migrations\PersistedGrantDb --project authzilla.mssql -- --provider MSSQL