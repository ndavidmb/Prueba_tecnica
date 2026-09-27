-- Habilitar extensión para generación de GUIDs si no existe
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- 1. Crear tabla Teams
CREATE TABLE IF NOT EXISTS "Teams" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "Name" VARCHAR(200) NOT NULL
);

-- 2. Crear tabla Matches
CREATE TABLE IF NOT EXISTS "Matches" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "LocalTeamId" UUID NOT NULL,
    "VisitorTeamId" UUID NOT NULL,
    "LocalGoals" INT NOT NULL DEFAULT 0,
    "VisitorGoals" INT NOT NULL DEFAULT 0,
    "Status" VARCHAR(50) NOT NULL,
    CONSTRAINT "FK_Matches_Teams_Local" FOREIGN KEY ("LocalTeamId") REFERENCES "Teams"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Matches_Teams_Visitor" FOREIGN KEY ("VisitorTeamId") REFERENCES "Teams"("Id") ON DELETE CASCADE
);

-- 3. Equipos Grupo A
INSERT INTO "Teams" ("Id", "Name") VALUES
(gen_random_uuid(), 'Real Madrid'),
(gen_random_uuid(), 'Bayern München'),
(gen_random_uuid(), 'Paris Saint-Germain'),
(gen_random_uuid(), 'Inter de Milán');

-- 4. Equipos Grupo B
INSERT INTO "Teams" ("Id", "Name") VALUES
(gen_random_uuid(), 'Manchester City'),
(gen_random_uuid(), 'FC Barcelona'),
(gen_random_uuid(), 'Borussia Dortmund'),
(gen_random_uuid(), 'Arsenal');

-- 5. Bloque anónimo para insertar partidos
DO $$
DECLARE
    v_a1 UUID; v_a2 UUID; v_a3 UUID; v_a4 UUID;
    v_b1 UUID; v_b2 UUID; v_b3 UUID; v_b4 UUID;
BEGIN
    SELECT "Id" INTO v_a1 FROM "Teams" WHERE "Name" = 'Real Madrid';
    SELECT "Id" INTO v_a2 FROM "Teams" WHERE "Name" = 'Bayern München';
    SELECT "Id" INTO v_a3 FROM "Teams" WHERE "Name" = 'Paris Saint-Germain';
    SELECT "Id" INTO v_a4 FROM "Teams" WHERE "Name" = 'Inter de Milán';

    SELECT "Id" INTO v_b1 FROM "Teams" WHERE "Name" = 'Manchester City';
    SELECT "Id" INTO v_b2 FROM "Teams" WHERE "Name" = 'FC Barcelona';
    SELECT "Id" INTO v_b3 FROM "Teams" WHERE "Name" = 'Borussia Dortmund';
    SELECT "Id" INTO v_b4 FROM "Teams" WHERE "Name" = 'Arsenal';

    INSERT INTO "Matches" ("Id", "LocalTeamId", "VisitorTeamId", "LocalGoals", "VisitorGoals", "Status") VALUES
    (gen_random_uuid(), v_a1, v_a2, 2, 1, 'FullTime'),
    (gen_random_uuid(), v_a3, v_a4, 0, 0, 'FullTime'),
    (gen_random_uuid(), v_a1, v_a3, 1, 3, 'FullTime'),
    (gen_random_uuid(), v_a2, v_a4, 0, 0, 'UpcomingMatch'),
    (gen_random_uuid(), v_a4, v_a1, 0, 0, 'UpcomingMatch'),
    (gen_random_uuid(), v_a2, v_a3, 0, 0, 'UpcomingMatch');

    INSERT INTO "Matches" ("Id", "LocalTeamId", "VisitorTeamId", "LocalGoals", "VisitorGoals", "Status") VALUES
    (gen_random_uuid(), v_b1, v_b2, 3, 1, 'FullTime'),
    (gen_random_uuid(), v_b3, v_b4, 2, 2, 'FullTime'),
    (gen_random_uuid(), v_b1, v_b3, 1, 0, 'FullTime'),
    (gen_random_uuid(), v_b2, v_b4, 0, 0, 'UpcomingMatch'),
    (gen_random_uuid(), v_b4, v_b1, 0, 0, 'UpcomingMatch'),
    (gen_random_uuid(), v_b2, v_b3, 0, 0, 'UpcomingMatch');
END $$;