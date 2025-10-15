ConfigSettingsDemo (.NET 9) - Full runnable demo (dockerized Web API + Postgres + WinForms)
-------------------------------------------------------------------------------

This archive contains a .NET 9 solution with:
- ConfigSettings.Shared (library)
- ConfigSettings.WebApi (minimal API, dockerized)
- ConfigSettings.WinForms (WinForms desktop client)
- ConfigSettings.Tests (xUnit integration/unit tests)
- docker-compose.yml to run Postgres and the Web API container

Features:
- Strategy pattern for data access (Postgres direct or API)
- AES-256 encryption with per-row IVs (IV stored as base64 in `iv` column)
- HttpClient resilience using Microsoft.Extensions.Http.Resilience (AddStandardResilienceHandler)
- Dockerfile for Web API and docker-compose builds the Web API image and runs it

Quickstart (docker):
1. Ensure Docker and .NET 9 SDK are installed.
2. From the repo root run:
   docker-compose up --build -d
   This will build the Web API image, start Postgres, wait for Postgres healthy, then start the Web API.
3. Apply DB schema (run the SQL script inside the running postgres container):
   docker exec -i configsettings-postgres psql -U postgres -d settingsdb <<'EOS'
   CREATE SCHEMA IF NOT EXISTS settings;
   CREATE TABLE IF NOT EXISTS settings.config_settings (
       config_settings_id SERIAL PRIMARY KEY,
       application_name varchar(75) NOT NULL,
       instance varchar(50) NOT NULL,
       hostname varchar(50) NOT NULL,
       username varchar(50) NOT NULL,
       classification varchar(75) NOT NULL,
       field_name varchar(75) NOT NULL,
       field_value varchar(1500) NOT NULL,
       is_encrypted boolean NOT NULL DEFAULT false,
       salt varchar(100),
       iv varchar(64),
       comment varchar(500),
       updated_by varchar(255) NOT NULL,
       updated_date timestamptz NOT NULL DEFAULT now()
   );
   EOS
4. Web API will be available at http://localhost:5000
5. Open and run the WinForms app (ConfigSettings.WinForms) from Visual Studio or dotnet run (Windows).

Running tests:
- From the ConfigSettings.Tests folder run: dotnet test

Security:
- The demo uses a development master key. Do not use in production.
