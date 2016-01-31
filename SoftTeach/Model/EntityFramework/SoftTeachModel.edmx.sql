
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 12/30/2012 22:52:01
-- Generated from EDMX file: C:\Users\Adrian\VisualStudioProjects\SoftTeach@Codeplex\SoftTeach\Model\SoftTeachModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [SoftTeach];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_ArbeitAufgabe]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Aufgaben] DROP CONSTRAINT [FK_ArbeitAufgabe];
GO
IF OBJECT_ID(N'[dbo].[FK_AufgabeErgebnis]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Ergebnisse] DROP CONSTRAINT [FK_AufgabeErgebnis];
GO
IF OBJECT_ID(N'[dbo].[FK_CurriculumReihe]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Reihen] DROP CONSTRAINT [FK_CurriculumReihe];
GO
IF OBJECT_ID(N'[dbo].[FK_DateitypDateiverweis]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Dateiverweise] DROP CONSTRAINT [FK_DateitypDateiverweis];
GO
IF OBJECT_ID(N'[dbo].[FK_ErsteUnterrichtsstundeTermin]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Termine] DROP CONSTRAINT [FK_ErsteUnterrichtsstundeTermin];
GO
IF OBJECT_ID(N'[dbo].[FK_FachCurriculum]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Curricula] DROP CONSTRAINT [FK_FachCurriculum];
GO
IF OBJECT_ID(N'[dbo].[FK_FachFachstundenanzahl]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fachstundenanzahlen] DROP CONSTRAINT [FK_FachFachstundenanzahl];
GO
IF OBJECT_ID(N'[dbo].[FK_FachJahresplan]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Jahrespläne] DROP CONSTRAINT [FK_FachJahresplan];
GO
IF OBJECT_ID(N'[dbo].[FK_FachModul]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Module] DROP CONSTRAINT [FK_FachModul];
GO
IF OBJECT_ID(N'[dbo].[FK_FachStundenentwurf]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Stundenentwürfe] DROP CONSTRAINT [FK_FachStundenentwurf];
GO
IF OBJECT_ID(N'[dbo].[FK_FachStundenplaneintrag]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Stundenplaneinträge] DROP CONSTRAINT [FK_FachStundenplaneintrag];
GO
IF OBJECT_ID(N'[dbo].[FK_HalbjahresplanMonatsplan]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Monatspläne] DROP CONSTRAINT [FK_HalbjahresplanMonatsplan];
GO
IF OBJECT_ID(N'[dbo].[FK_HalbjahrtypArbeit]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Arbeiten] DROP CONSTRAINT [FK_HalbjahrtypArbeit];
GO
IF OBJECT_ID(N'[dbo].[FK_HalbjahrtypSchülerliste]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Schülerlisten] DROP CONSTRAINT [FK_HalbjahrtypSchülerliste];
GO
IF OBJECT_ID(N'[dbo].[FK_HalbjahrtypSchulhalbjahr]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Halbjahrespläne] DROP CONSTRAINT [FK_HalbjahrtypSchulhalbjahr];
GO
IF OBJECT_ID(N'[dbo].[FK_HalbjahrtypStundenplan]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Stundenpläne] DROP CONSTRAINT [FK_HalbjahrtypStundenplan];
GO
IF OBJECT_ID(N'[dbo].[FK_JahresplanHalbjahresplan]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Halbjahrespläne] DROP CONSTRAINT [FK_JahresplanHalbjahresplan];
GO
IF OBJECT_ID(N'[dbo].[FK_JahrgangsstufeKlassenstufe]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Klassenstufen] DROP CONSTRAINT [FK_JahrgangsstufeKlassenstufe];
GO
IF OBJECT_ID(N'[dbo].[FK_JahrgangsstufeModul]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Module] DROP CONSTRAINT [FK_JahrgangsstufeModul];
GO
IF OBJECT_ID(N'[dbo].[FK_JahrgangsstufeStundenentwurf]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Stundenentwürfe] DROP CONSTRAINT [FK_JahrgangsstufeStundenentwurf];
GO
IF OBJECT_ID(N'[dbo].[FK_JahrtypArbeit]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Arbeiten] DROP CONSTRAINT [FK_JahrtypArbeit];
GO
IF OBJECT_ID(N'[dbo].[FK_JahrtypCurriculum]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Curricula] DROP CONSTRAINT [FK_JahrtypCurriculum];
GO
IF OBJECT_ID(N'[dbo].[FK_JahrtypFerien]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Ferien] DROP CONSTRAINT [FK_JahrtypFerien];
GO
IF OBJECT_ID(N'[dbo].[FK_JahrtypJahresplan]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Jahrespläne] DROP CONSTRAINT [FK_JahrtypJahresplan];
GO
IF OBJECT_ID(N'[dbo].[FK_JahrtypSchülerliste]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Schülerlisten] DROP CONSTRAINT [FK_JahrtypSchülerliste];
GO
IF OBJECT_ID(N'[dbo].[FK_JahrtypSchultermin]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Termine_Schultermin] DROP CONSTRAINT [FK_JahrtypSchultermin];
GO
IF OBJECT_ID(N'[dbo].[FK_JahrtypSchulwoche]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Schulwochen] DROP CONSTRAINT [FK_JahrtypSchulwoche];
GO
IF OBJECT_ID(N'[dbo].[FK_JahrtypStundenplan]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Stundenpläne] DROP CONSTRAINT [FK_JahrtypStundenplan];
GO
IF OBJECT_ID(N'[dbo].[FK_KlasseArbeit]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Arbeiten] DROP CONSTRAINT [FK_KlasseArbeit];
GO
IF OBJECT_ID(N'[dbo].[FK_KlasseBetroffeneKlasse]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BetroffeneKlassen] DROP CONSTRAINT [FK_KlasseBetroffeneKlasse];
GO
IF OBJECT_ID(N'[dbo].[FK_KlasseJahresplan]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Jahrespläne] DROP CONSTRAINT [FK_KlasseJahresplan];
GO
IF OBJECT_ID(N'[dbo].[FK_KlassenstufeCurriculum]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Curricula] DROP CONSTRAINT [FK_KlassenstufeCurriculum];
GO
IF OBJECT_ID(N'[dbo].[FK_KlassenstufeFachstundenanzahl]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fachstundenanzahlen] DROP CONSTRAINT [FK_KlassenstufeFachstundenanzahl];
GO
IF OBJECT_ID(N'[dbo].[FK_KlassenstufeKlasse]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Klassen] DROP CONSTRAINT [FK_KlassenstufeKlasse];
GO
IF OBJECT_ID(N'[dbo].[FK_KlasseSchülerliste]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Schülerlisten] DROP CONSTRAINT [FK_KlasseSchülerliste];
GO
IF OBJECT_ID(N'[dbo].[FK_KlasseStundenplaneintrag]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Stundenplaneinträge] DROP CONSTRAINT [FK_KlasseStundenplaneintrag];
GO
IF OBJECT_ID(N'[dbo].[FK_Lerngruppentermin_inherits_Termin]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Termine_Lerngruppentermin] DROP CONSTRAINT [FK_Lerngruppentermin_inherits_Termin];
GO
IF OBJECT_ID(N'[dbo].[FK_MediumPhase]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Phasen] DROP CONSTRAINT [FK_MediumPhase];
GO
IF OBJECT_ID(N'[dbo].[FK_ModulReihe]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Reihen] DROP CONSTRAINT [FK_ModulReihe];
GO
IF OBJECT_ID(N'[dbo].[FK_ModulStundenentwurf]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Stundenentwürfe] DROP CONSTRAINT [FK_ModulStundenentwurf];
GO
IF OBJECT_ID(N'[dbo].[FK_MonatsplanTagesplan]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tagespläne] DROP CONSTRAINT [FK_MonatsplanTagesplan];
GO
IF OBJECT_ID(N'[dbo].[FK_MonatstypKalendermonat]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Monatspläne] DROP CONSTRAINT [FK_MonatstypKalendermonat];
GO
IF OBJECT_ID(N'[dbo].[FK_NotentypArbeit]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Arbeiten] DROP CONSTRAINT [FK_NotentypArbeit];
GO
IF OBJECT_ID(N'[dbo].[FK_NotentypNoten]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Noten] DROP CONSTRAINT [FK_NotentypNoten];
GO
IF OBJECT_ID(N'[dbo].[FK_PersonSchülereintrag]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Schülereinträge] DROP CONSTRAINT [FK_PersonSchülereintrag];
GO
IF OBJECT_ID(N'[dbo].[FK_ReiheSequenz]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sequenzen] DROP CONSTRAINT [FK_ReiheSequenz];
GO
IF OBJECT_ID(N'[dbo].[FK_SchülereintragErgebnis]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Ergebnisse] DROP CONSTRAINT [FK_SchülereintragErgebnis];
GO
IF OBJECT_ID(N'[dbo].[FK_SchülerlisteSchülereintrag]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Schülereinträge] DROP CONSTRAINT [FK_SchülerlisteSchülereintrag];
GO
IF OBJECT_ID(N'[dbo].[FK_Schultermin_inherits_Termin]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Termine_Schultermin] DROP CONSTRAINT [FK_Schultermin_inherits_Termin];
GO
IF OBJECT_ID(N'[dbo].[FK_SchulwocheSchultag]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Schultage] DROP CONSTRAINT [FK_SchulwocheSchultag];
GO
IF OBJECT_ID(N'[dbo].[FK_SozialformPhase]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Phasen] DROP CONSTRAINT [FK_SozialformPhase];
GO
IF OBJECT_ID(N'[dbo].[FK_Stunde_inherits_Lerngruppentermin]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Termine_Stunde] DROP CONSTRAINT [FK_Stunde_inherits_Lerngruppentermin];
GO
IF OBJECT_ID(N'[dbo].[FK_StundenentwurfDateiverweis]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Dateiverweise] DROP CONSTRAINT [FK_StundenentwurfDateiverweis];
GO
IF OBJECT_ID(N'[dbo].[FK_StundenentwurfPhasen]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Phasen] DROP CONSTRAINT [FK_StundenentwurfPhasen];
GO
IF OBJECT_ID(N'[dbo].[FK_StundenentwurfStunde]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Termine_Stunde] DROP CONSTRAINT [FK_StundenentwurfStunde];
GO
IF OBJECT_ID(N'[dbo].[FK_StundenplanStundenplaneintrag]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Stundenplaneinträge] DROP CONSTRAINT [FK_StundenplanStundenplaneintrag];
GO
IF OBJECT_ID(N'[dbo].[FK_TagesplanLerngruppentermin]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Termine_Lerngruppentermin] DROP CONSTRAINT [FK_TagesplanLerngruppentermin];
GO
IF OBJECT_ID(N'[dbo].[FK_TerminBetroffeneKlasse]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BetroffeneKlassen] DROP CONSTRAINT [FK_TerminBetroffeneKlasse];
GO
IF OBJECT_ID(N'[dbo].[FK_TermintypSchultag]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Schultage] DROP CONSTRAINT [FK_TermintypSchultag];
GO
IF OBJECT_ID(N'[dbo].[FK_TermintypTermin]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Termine] DROP CONSTRAINT [FK_TermintypTermin];
GO
IF OBJECT_ID(N'[dbo].[FK_UnterrichtsstundeTermin]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Termine] DROP CONSTRAINT [FK_UnterrichtsstundeTermin];
GO
IF OBJECT_ID(N'[dbo].[FK_ZensurNoten]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Noten] DROP CONSTRAINT [FK_ZensurNoten];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Arbeiten]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Arbeiten];
GO
IF OBJECT_ID(N'[dbo].[Aufgaben]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Aufgaben];
GO
IF OBJECT_ID(N'[dbo].[BetroffeneKlassen]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BetroffeneKlassen];
GO
IF OBJECT_ID(N'[dbo].[Curricula]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Curricula];
GO
IF OBJECT_ID(N'[dbo].[Dateitypen]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Dateitypen];
GO
IF OBJECT_ID(N'[dbo].[Dateiverweise]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Dateiverweise];
GO
IF OBJECT_ID(N'[dbo].[Ergebnisse]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Ergebnisse];
GO
IF OBJECT_ID(N'[dbo].[Fächer]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Fächer];
GO
IF OBJECT_ID(N'[dbo].[Fachstundenanzahlen]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Fachstundenanzahlen];
GO
IF OBJECT_ID(N'[dbo].[Ferien]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Ferien];
GO
IF OBJECT_ID(N'[dbo].[Halbjahrespläne]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Halbjahrespläne];
GO
IF OBJECT_ID(N'[dbo].[Halbjahrtypen]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Halbjahrtypen];
GO
IF OBJECT_ID(N'[dbo].[Jahrespläne]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Jahrespläne];
GO
IF OBJECT_ID(N'[dbo].[Jahrgangsstufen]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Jahrgangsstufen];
GO
IF OBJECT_ID(N'[dbo].[Jahrtypen]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Jahrtypen];
GO
IF OBJECT_ID(N'[dbo].[Klassen]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Klassen];
GO
IF OBJECT_ID(N'[dbo].[Klassenstufen]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Klassenstufen];
GO
IF OBJECT_ID(N'[dbo].[Medien]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Medien];
GO
IF OBJECT_ID(N'[dbo].[Module]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Module];
GO
IF OBJECT_ID(N'[dbo].[Monatspläne]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Monatspläne];
GO
IF OBJECT_ID(N'[dbo].[Monatstypen]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Monatstypen];
GO
IF OBJECT_ID(N'[dbo].[Noten]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Noten];
GO
IF OBJECT_ID(N'[dbo].[Notentypen]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Notentypen];
GO
IF OBJECT_ID(N'[dbo].[Personen]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Personen];
GO
IF OBJECT_ID(N'[dbo].[Phasen]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Phasen];
GO
IF OBJECT_ID(N'[dbo].[Reihen]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Reihen];
GO
IF OBJECT_ID(N'[dbo].[Schülereinträge]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Schülereinträge];
GO
IF OBJECT_ID(N'[dbo].[Schülerlisten]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Schülerlisten];
GO
IF OBJECT_ID(N'[dbo].[Schultage]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Schultage];
GO
IF OBJECT_ID(N'[dbo].[Schulwochen]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Schulwochen];
GO
IF OBJECT_ID(N'[dbo].[Sequenzen]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sequenzen];
GO
IF OBJECT_ID(N'[dbo].[Sozialformen]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sozialformen];
GO
IF OBJECT_ID(N'[dbo].[Stundenentwürfe]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Stundenentwürfe];
GO
IF OBJECT_ID(N'[dbo].[Stundenpläne]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Stundenpläne];
GO
IF OBJECT_ID(N'[dbo].[Stundenplaneinträge]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Stundenplaneinträge];
GO
IF OBJECT_ID(N'[dbo].[StundenplanWochentage]', 'U') IS NOT NULL
    DROP TABLE [dbo].[StundenplanWochentage];
GO
IF OBJECT_ID(N'[dbo].[Tagespläne]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tagespläne];
GO
IF OBJECT_ID(N'[dbo].[Tagtypen]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tagtypen];
GO
IF OBJECT_ID(N'[dbo].[Termine]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Termine];
GO
IF OBJECT_ID(N'[dbo].[Termine_Lerngruppentermin]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Termine_Lerngruppentermin];
GO
IF OBJECT_ID(N'[dbo].[Termine_Schultermin]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Termine_Schultermin];
GO
IF OBJECT_ID(N'[dbo].[Termine_Stunde]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Termine_Stunde];
GO
IF OBJECT_ID(N'[dbo].[Termintypen]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Termintypen];
GO
IF OBJECT_ID(N'[dbo].[Unterrichtsstunden]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Unterrichtsstunden];
GO
IF OBJECT_ID(N'[dbo].[Zensuren]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Zensuren];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Stundenentwürfe'
CREATE TABLE [dbo].[Stundenentwürfe] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FachId] int  NOT NULL,
    [JahrgangsstufeId] int  NOT NULL,
    [ModulId] int  NOT NULL,
    [Stundenthema] nvarchar(max)  NOT NULL,
    [Kopieren] bit  NOT NULL,
    [Computer] bit  NOT NULL,
    [Hausaufgaben] nvarchar(max)  NOT NULL,
    [Ansagen] nvarchar(max)  NOT NULL,
    [Datum] datetime  NOT NULL,
    [Stundenzahl] int  NOT NULL
);
GO

-- Creating table 'Phasen'
CREATE TABLE [dbo].[Phasen] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [StundenentwurfId] int  NOT NULL,
    [Zeit] int  NOT NULL,
    [MediumId] int  NOT NULL,
    [SozialformId] int  NOT NULL,
    [Inhalt] nvarchar(max)  NOT NULL,
    [AbfolgeIndex] int  NOT NULL
);
GO

-- Creating table 'Medien'
CREATE TABLE [dbo].[Medien] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Bezeichnung] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Sozialformen'
CREATE TABLE [dbo].[Sozialformen] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Bezeichnung] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Module'
CREATE TABLE [dbo].[Module] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FachId] int  NOT NULL,
    [Bezeichnung] nvarchar(max)  NOT NULL,
    [JahrgangsstufeId] int  NOT NULL,
    [Bausteine] nvarchar(max)  NULL,
    [Stundenbedarf] int  NOT NULL,
);
GO

-- Creating table 'Fächer'
CREATE TABLE [dbo].[Fächer] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Bezeichnung] nvarchar(max)  NOT NULL,
    [Farbe] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Halbjahrespläne'
CREATE TABLE [dbo].[Halbjahrespläne] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [JahresplanId] int  NOT NULL,
    [HalbjahrtypId] int  NOT NULL
);
GO

-- Creating table 'Klassen'
CREATE TABLE [dbo].[Klassen] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [KlassenstufeId] int  NOT NULL,
    [Bezeichnung] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Unterrichtsstunden'
CREATE TABLE [dbo].[Unterrichtsstunden] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Bezeichnung] nvarchar(max)  NOT NULL,
    [Beginn] time  NOT NULL,
    [Ende] time  NOT NULL,
    [Stundenindex] int  NOT NULL
);
GO

-- Creating table 'Jahrgangsstufen'
CREATE TABLE [dbo].[Jahrgangsstufen] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Bezeichnung] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Tagespläne'
CREATE TABLE [dbo].[Tagespläne] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [MonatsplanId] int  NOT NULL,
    [Datum] datetime  NOT NULL,
    [Ferientag] bit  NOT NULL,
    [Beschreibung] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Monatspläne'
CREATE TABLE [dbo].[Monatspläne] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [HalbjahresplanId] int  NOT NULL,
    [MonatstypId] int  NOT NULL,
    [Monatstyp_Id] int  NOT NULL
);
GO

-- Creating table 'Jahrespläne'
CREATE TABLE [dbo].[Jahrespläne] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Bezeichnung] nvarchar(max)  NOT NULL,
    [FachId] int  NOT NULL,
    [KlasseId] int  NOT NULL,
    [JahrtypId] int  NOT NULL
);
GO

-- Creating table 'Halbjahrtypen'
CREATE TABLE [dbo].[Halbjahrtypen] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Bezeichnung] nvarchar(max)  NOT NULL,
    [HalbjahrIndex] int  NOT NULL
);
GO

-- Creating table 'Monatstypen'
CREATE TABLE [dbo].[Monatstypen] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Bezeichnung] nvarchar(max)  NOT NULL,
    [MonatIndex] int  NOT NULL
);
GO

-- Creating table 'Stundenplaneinträge'
CREATE TABLE [dbo].[Stundenplaneinträge] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [StundenplanId] int  NOT NULL,
    [ErsteUnterrichtsstundeIndex] int  NOT NULL,
    [LetzteUnterrichtsstundeIndex] int  NOT NULL,
    [WochentagIndex] int  NOT NULL,
    [FachId] int  NOT NULL,
    [KlasseId] int  NOT NULL,
    [Raum] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Jahrtypen'
CREATE TABLE [dbo].[Jahrtypen] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Bezeichnung] nvarchar(max)  NOT NULL,
    [Jahr] int  NOT NULL
);
GO

-- Creating table 'Stundenpläne'
CREATE TABLE [dbo].[Stundenpläne] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Bezeichnung] nvarchar(max)  NOT NULL,
    [JahrtypId] int  NOT NULL,
    [HalbjahrtypId] int  NOT NULL,
    [GültigAb] datetime  NOT NULL
);
GO

-- Creating table 'Ferien'
CREATE TABLE [dbo].[Ferien] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [JahrtypId] int  NOT NULL,
    [Bezeichnung] nvarchar(max)  NOT NULL,
    [ErsterFerientag] datetime  NOT NULL,
    [LetzterFerientag] datetime  NOT NULL
);
GO

-- Creating table 'BetroffeneKlassen'
CREATE TABLE [dbo].[BetroffeneKlassen] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TerminId] int  NOT NULL,
    [KlasseId] int  NOT NULL
);
GO

-- Creating table 'Termintypen'
CREATE TABLE [dbo].[Termintypen] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Bezeichnung] nvarchar(max)  NOT NULL,
    [Kalenderfarbe] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Dateiverweise'
CREATE TABLE [dbo].[Dateiverweise] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Dateiname] nvarchar(max)  NOT NULL,
    [DateitypId] int  NOT NULL,
    [StundenentwurfId] int  NOT NULL
);
GO

-- Creating table 'Dateitypen'
CREATE TABLE [dbo].[Dateitypen] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Bezeichnung] nvarchar(max)  NOT NULL,
    [Kürzel] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Termine'
CREATE TABLE [dbo].[Termine] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TermintypId] int  NOT NULL,
    [Beschreibung] nvarchar(max)  NULL,
    [ErsteUnterrichtsstundeId] int  NOT NULL,
    [LetzteUnterrichtsstundeId] int  NOT NULL,
    [Ort] nvarchar(max)  NULL,
    [IstGeprüft] bit  NOT NULL
);
GO

-- Creating table 'Reihen'
CREATE TABLE [dbo].[Reihen] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ModulId] int  NOT NULL,
    [Thema] nvarchar(max)  NOT NULL,
    [Stundenbedarf] int  NOT NULL,
    [CurriculumId] int  NOT NULL,
    [AbfolgeIndex] int  NOT NULL
);
GO

-- Creating table 'Curricula'
CREATE TABLE [dbo].[Curricula] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FachId] int  NOT NULL,
    [KlassenstufeId] int  NOT NULL,
    [Bezeichnung] nvarchar(max)  NOT NULL,
    [JahrtypId] int  NOT NULL
);
GO

-- Creating table 'Sequenzen'
CREATE TABLE [dbo].[Sequenzen] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ReiheId] int  NOT NULL,
    [AbfolgeIndex] int  NOT NULL,
    [Stundenbedarf] int  NOT NULL,
    [Thema] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Fachstundenanzahlen'
CREATE TABLE [dbo].[Fachstundenanzahlen] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FachId] int  NOT NULL,
    [KlassenstufeId] int  NOT NULL,
    [Stundenzahl] int  NOT NULL,
    [Teilungsstundenzahl] int  NOT NULL
);
GO

-- Creating table 'Klassenstufen'
CREATE TABLE [dbo].[Klassenstufen] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [JahrgangsstufeId] int  NOT NULL,
    [Bezeichnung] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Schulwochen'
CREATE TABLE [dbo].[Schulwochen] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [JahrtypId] int  NOT NULL,
    [Montagsdatum] datetime  NOT NULL
);
GO

-- Creating table 'Schultage'
CREATE TABLE [dbo].[Schultage] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [SchulwocheId] int  NOT NULL,
    [TermintypId] int  NOT NULL,
    [Datum] datetime  NOT NULL
);
GO

-- Creating table 'Personen'
CREATE TABLE [dbo].[Personen] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Vorname] nvarchar(max)  NOT NULL,
    [Nachname] nvarchar(max)  NOT NULL,
    [Geschlecht] bit  NOT NULL,
    [Geburtstag] datetime  NULL,
    [Titel] nvarchar(max)  NULL,
    [Telefon] nvarchar(max)  NULL,
    [Fax] nvarchar(max)  NULL,
    [Handy] nvarchar(max)  NULL,
    [EMail] nvarchar(max)  NULL,
    [PLZ] nvarchar(max)  NULL,
    [Straße] nvarchar(max)  NULL,
    [Hausnummer] nvarchar(max)  NULL,
    [Ort] nvarchar(max)  NULL,
    [IstLehrer] bit  NOT NULL,
    [Foto] varbinary(max)  NOT NULL
);
GO

-- Creating table 'Noten'
CREATE TABLE [dbo].[Noten] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [SchülereintragId] int  NOT NULL,
    [NotentypId] int  NOT NULL,
    [ZensurId] int  NOT NULL
);
GO

-- Creating table 'Notentypen'
CREATE TABLE [dbo].[Notentypen] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Bezeichnung] nvarchar(max)  NOT NULL,
    [Wichtung] real  NOT NULL,
    [Art] bit  NOT NULL
);
GO

-- Creating table 'Zensuren'
CREATE TABLE [dbo].[Zensuren] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Notenpunkte] int  NOT NULL,
    [NoteMitTendenz] nvarchar(max)  NOT NULL,
    [GanzeNote] int  NOT NULL
);
GO

-- Creating table 'Arbeiten'
CREATE TABLE [dbo].[Arbeiten] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [JahrtypId] int  NOT NULL,
    [HalbjahrtypId] int  NOT NULL,
    [KlasseId] int  NOT NULL,
    [LfdNr] int  NOT NULL,
    [NotentypId] int  NOT NULL
);
GO

-- Creating table 'Aufgaben'
CREATE TABLE [dbo].[Aufgaben] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ArbeitId] int  NOT NULL,
    [LfdNr] int  NOT NULL,
    [MaxPunkte] int  NOT NULL
);
GO

-- Creating table 'Ergebnisse'
CREATE TABLE [dbo].[Ergebnisse] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [AufgabeId] int  NOT NULL,
    [SchülereintragId] int  NOT NULL,
    [Punktzahl] int  NOT NULL
);
GO

-- Creating table 'Schülerlisten'
CREATE TABLE [dbo].[Schülerlisten] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [JahrtypId] int  NOT NULL,
    [HalbjahrtypId] int  NOT NULL,
    [KlasseId] int  NOT NULL,
    [FachId] int  NULL
);
GO

-- Creating table 'Schülereinträge'
CREATE TABLE [dbo].[Schülereinträge] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [SchülerlisteId] int  NOT NULL,
    [PersonId] int  NOT NULL
);
GO

-- Creating table 'StundenplanWochentage'
CREATE TABLE [dbo].[StundenplanWochentage] (
    [StundenplanWochentagId] int IDENTITY(1,1) NOT NULL,
    [Wochentag] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Tagtypen'
CREATE TABLE [dbo].[Tagtypen] (
    [TagtypId] int IDENTITY(1,1) NOT NULL,
    [Bezeichnung] nvarchar(100)  NOT NULL,
    [Kalenderfarbe] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Termine_Lerngruppentermin'
CREATE TABLE [dbo].[Termine_Lerngruppentermin] (
    [TagesplanId] int  NOT NULL,
    [Id] int  NOT NULL
);
GO

-- Creating table 'Termine_Stunde'
CREATE TABLE [dbo].[Termine_Stunde] (
    [StundenentwurfId] int  NULL,
    [Id] int  NOT NULL
);
GO

-- Creating table 'Termine_Schultermin'
CREATE TABLE [dbo].[Termine_Schultermin] (
    [JahrtypId] int  NOT NULL,
    [Datum] datetime  NOT NULL,
    [Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Stundenentwürfe'
ALTER TABLE [dbo].[Stundenentwürfe]
ADD CONSTRAINT [PK_Stundenentwürfe]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id], [StundenentwurfId] in table 'Phasen'
ALTER TABLE [dbo].[Phasen]
ADD CONSTRAINT [PK_Phasen]
    PRIMARY KEY CLUSTERED ([Id], [StundenentwurfId] ASC);
GO

-- Creating primary key on [Id] in table 'Medien'
ALTER TABLE [dbo].[Medien]
ADD CONSTRAINT [PK_Medien]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Sozialformen'
ALTER TABLE [dbo].[Sozialformen]
ADD CONSTRAINT [PK_Sozialformen]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Module'
ALTER TABLE [dbo].[Module]
ADD CONSTRAINT [PK_Module]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Fächer'
ALTER TABLE [dbo].[Fächer]
ADD CONSTRAINT [PK_Fächer]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Halbjahrespläne'
ALTER TABLE [dbo].[Halbjahrespläne]
ADD CONSTRAINT [PK_Halbjahrespläne]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Klassen'
ALTER TABLE [dbo].[Klassen]
ADD CONSTRAINT [PK_Klassen]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Unterrichtsstunden'
ALTER TABLE [dbo].[Unterrichtsstunden]
ADD CONSTRAINT [PK_Unterrichtsstunden]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Jahrgangsstufen'
ALTER TABLE [dbo].[Jahrgangsstufen]
ADD CONSTRAINT [PK_Jahrgangsstufen]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tagespläne'
ALTER TABLE [dbo].[Tagespläne]
ADD CONSTRAINT [PK_Tagespläne]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Monatspläne'
ALTER TABLE [dbo].[Monatspläne]
ADD CONSTRAINT [PK_Monatspläne]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Jahrespläne'
ALTER TABLE [dbo].[Jahrespläne]
ADD CONSTRAINT [PK_Jahrespläne]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Halbjahrtypen'
ALTER TABLE [dbo].[Halbjahrtypen]
ADD CONSTRAINT [PK_Halbjahrtypen]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Monatstypen'
ALTER TABLE [dbo].[Monatstypen]
ADD CONSTRAINT [PK_Monatstypen]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Stundenplaneinträge'
ALTER TABLE [dbo].[Stundenplaneinträge]
ADD CONSTRAINT [PK_Stundenplaneinträge]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Jahrtypen'
ALTER TABLE [dbo].[Jahrtypen]
ADD CONSTRAINT [PK_Jahrtypen]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Stundenpläne'
ALTER TABLE [dbo].[Stundenpläne]
ADD CONSTRAINT [PK_Stundenpläne]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Ferien'
ALTER TABLE [dbo].[Ferien]
ADD CONSTRAINT [PK_Ferien]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BetroffeneKlassen'
ALTER TABLE [dbo].[BetroffeneKlassen]
ADD CONSTRAINT [PK_BetroffeneKlassen]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Termintypen'
ALTER TABLE [dbo].[Termintypen]
ADD CONSTRAINT [PK_Termintypen]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Dateiverweise'
ALTER TABLE [dbo].[Dateiverweise]
ADD CONSTRAINT [PK_Dateiverweise]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Dateitypen'
ALTER TABLE [dbo].[Dateitypen]
ADD CONSTRAINT [PK_Dateitypen]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Termine'
ALTER TABLE [dbo].[Termine]
ADD CONSTRAINT [PK_Termine]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Reihen'
ALTER TABLE [dbo].[Reihen]
ADD CONSTRAINT [PK_Reihen]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Curricula'
ALTER TABLE [dbo].[Curricula]
ADD CONSTRAINT [PK_Curricula]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Sequenzen'
ALTER TABLE [dbo].[Sequenzen]
ADD CONSTRAINT [PK_Sequenzen]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Fachstundenanzahlen'
ALTER TABLE [dbo].[Fachstundenanzahlen]
ADD CONSTRAINT [PK_Fachstundenanzahlen]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Klassenstufen'
ALTER TABLE [dbo].[Klassenstufen]
ADD CONSTRAINT [PK_Klassenstufen]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Schulwochen'
ALTER TABLE [dbo].[Schulwochen]
ADD CONSTRAINT [PK_Schulwochen]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Schultage'
ALTER TABLE [dbo].[Schultage]
ADD CONSTRAINT [PK_Schultage]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Personen'
ALTER TABLE [dbo].[Personen]
ADD CONSTRAINT [PK_Personen]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Noten'
ALTER TABLE [dbo].[Noten]
ADD CONSTRAINT [PK_Noten]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Notentypen'
ALTER TABLE [dbo].[Notentypen]
ADD CONSTRAINT [PK_Notentypen]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Zensuren'
ALTER TABLE [dbo].[Zensuren]
ADD CONSTRAINT [PK_Zensuren]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Arbeiten'
ALTER TABLE [dbo].[Arbeiten]
ADD CONSTRAINT [PK_Arbeiten]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Aufgaben'
ALTER TABLE [dbo].[Aufgaben]
ADD CONSTRAINT [PK_Aufgaben]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Ergebnisse'
ALTER TABLE [dbo].[Ergebnisse]
ADD CONSTRAINT [PK_Ergebnisse]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Schülerlisten'
ALTER TABLE [dbo].[Schülerlisten]
ADD CONSTRAINT [PK_Schülerlisten]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Schülereinträge'
ALTER TABLE [dbo].[Schülereinträge]
ADD CONSTRAINT [PK_Schülereinträge]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [StundenplanWochentagId] in table 'StundenplanWochentage'
ALTER TABLE [dbo].[StundenplanWochentage]
ADD CONSTRAINT [PK_StundenplanWochentage]
    PRIMARY KEY CLUSTERED ([StundenplanWochentagId] ASC);
GO

-- Creating primary key on [TagtypId] in table 'Tagtypen'
ALTER TABLE [dbo].[Tagtypen]
ADD CONSTRAINT [PK_Tagtypen]
    PRIMARY KEY CLUSTERED ([TagtypId] ASC);
GO

-- Creating primary key on [Id] in table 'Termine_Lerngruppentermin'
ALTER TABLE [dbo].[Termine_Lerngruppentermin]
ADD CONSTRAINT [PK_Termine_Lerngruppentermin]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Termine_Stunde'
ALTER TABLE [dbo].[Termine_Stunde]
ADD CONSTRAINT [PK_Termine_Stunde]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Termine_Schultermin'
ALTER TABLE [dbo].[Termine_Schultermin]
ADD CONSTRAINT [PK_Termine_Schultermin]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [StundenentwurfId] in table 'Phasen'
ALTER TABLE [dbo].[Phasen]
ADD CONSTRAINT [FK_StundenentwurfPhasen]
    FOREIGN KEY ([StundenentwurfId])
    REFERENCES [dbo].[Stundenentwürfe]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StundenentwurfPhasen'
CREATE INDEX [IX_FK_StundenentwurfPhasen]
ON [dbo].[Phasen]
    ([StundenentwurfId]);
GO

-- Creating foreign key on [ModulId] in table 'Stundenentwürfe'
ALTER TABLE [dbo].[Stundenentwürfe]
ADD CONSTRAINT [FK_ModulStundenentwurf]
    FOREIGN KEY ([ModulId])
    REFERENCES [dbo].[Module]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ModulStundenentwurf'
CREATE INDEX [IX_FK_ModulStundenentwurf]
ON [dbo].[Stundenentwürfe]
    ([ModulId]);
GO

-- Creating foreign key on [FachId] in table 'Stundenentwürfe'
ALTER TABLE [dbo].[Stundenentwürfe]
ADD CONSTRAINT [FK_FachStundenentwurf]
    FOREIGN KEY ([FachId])
    REFERENCES [dbo].[Fächer]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_FachStundenentwurf'
CREATE INDEX [IX_FK_FachStundenentwurf]
ON [dbo].[Stundenentwürfe]
    ([FachId]);
GO

-- Creating foreign key on [SozialformId] in table 'Phasen'
ALTER TABLE [dbo].[Phasen]
ADD CONSTRAINT [FK_SozialformPhase]
    FOREIGN KEY ([SozialformId])
    REFERENCES [dbo].[Sozialformen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SozialformPhase'
CREATE INDEX [IX_FK_SozialformPhase]
ON [dbo].[Phasen]
    ([SozialformId]);
GO

-- Creating foreign key on [MediumId] in table 'Phasen'
ALTER TABLE [dbo].[Phasen]
ADD CONSTRAINT [FK_MediumPhase]
    FOREIGN KEY ([MediumId])
    REFERENCES [dbo].[Medien]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MediumPhase'
CREATE INDEX [IX_FK_MediumPhase]
ON [dbo].[Phasen]
    ([MediumId]);
GO

-- Creating foreign key on [JahrgangsstufeId] in table 'Module'
ALTER TABLE [dbo].[Module]
ADD CONSTRAINT [FK_JahrgangsstufeModul]
    FOREIGN KEY ([JahrgangsstufeId])
    REFERENCES [dbo].[Jahrgangsstufen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_JahrgangsstufeModul'
CREATE INDEX [IX_FK_JahrgangsstufeModul]
ON [dbo].[Module]
    ([JahrgangsstufeId]);
GO

-- Creating foreign key on [FachId] in table 'Module'
ALTER TABLE [dbo].[Module]
ADD CONSTRAINT [FK_FachModul]
    FOREIGN KEY ([FachId])
    REFERENCES [dbo].[Fächer]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_FachModul'
CREATE INDEX [IX_FK_FachModul]
ON [dbo].[Module]
    ([FachId]);
GO

-- Creating foreign key on [KlasseId] in table 'Stundenplaneinträge'
ALTER TABLE [dbo].[Stundenplaneinträge]
ADD CONSTRAINT [FK_KlasseStundenplaneintrag]
    FOREIGN KEY ([KlasseId])
    REFERENCES [dbo].[Klassen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_KlasseStundenplaneintrag'
CREATE INDEX [IX_FK_KlasseStundenplaneintrag]
ON [dbo].[Stundenplaneinträge]
    ([KlasseId]);
GO

-- Creating foreign key on [FachId] in table 'Stundenplaneinträge'
ALTER TABLE [dbo].[Stundenplaneinträge]
ADD CONSTRAINT [FK_FachStundenplaneintrag]
    FOREIGN KEY ([FachId])
    REFERENCES [dbo].[Fächer]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_FachStundenplaneintrag'
CREATE INDEX [IX_FK_FachStundenplaneintrag]
ON [dbo].[Stundenplaneinträge]
    ([FachId]);
GO

-- Creating foreign key on [HalbjahrtypId] in table 'Halbjahrespläne'
ALTER TABLE [dbo].[Halbjahrespläne]
ADD CONSTRAINT [FK_HalbjahrtypSchulhalbjahr]
    FOREIGN KEY ([HalbjahrtypId])
    REFERENCES [dbo].[Halbjahrtypen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_HalbjahrtypSchulhalbjahr'
CREATE INDEX [IX_FK_HalbjahrtypSchulhalbjahr]
ON [dbo].[Halbjahrespläne]
    ([HalbjahrtypId]);
GO

-- Creating foreign key on [Monatstyp_Id] in table 'Monatspläne'
ALTER TABLE [dbo].[Monatspläne]
ADD CONSTRAINT [FK_MonatstypKalendermonat]
    FOREIGN KEY ([Monatstyp_Id])
    REFERENCES [dbo].[Monatstypen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MonatstypKalendermonat'
CREATE INDEX [IX_FK_MonatstypKalendermonat]
ON [dbo].[Monatspläne]
    ([Monatstyp_Id]);
GO

-- Creating foreign key on [StundenentwurfId] in table 'Termine_Stunde'
ALTER TABLE [dbo].[Termine_Stunde]
ADD CONSTRAINT [FK_StundenentwurfStunde]
    FOREIGN KEY ([StundenentwurfId])
    REFERENCES [dbo].[Stundenentwürfe]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StundenentwurfStunde'
CREATE INDEX [IX_FK_StundenentwurfStunde]
ON [dbo].[Termine_Stunde]
    ([StundenentwurfId]);
GO

-- Creating foreign key on [HalbjahresplanId] in table 'Monatspläne'
ALTER TABLE [dbo].[Monatspläne]
ADD CONSTRAINT [FK_HalbjahresplanMonatsplan]
    FOREIGN KEY ([HalbjahresplanId])
    REFERENCES [dbo].[Halbjahrespläne]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_HalbjahresplanMonatsplan'
CREATE INDEX [IX_FK_HalbjahresplanMonatsplan]
ON [dbo].[Monatspläne]
    ([HalbjahresplanId]);
GO

-- Creating foreign key on [MonatsplanId] in table 'Tagespläne'
ALTER TABLE [dbo].[Tagespläne]
ADD CONSTRAINT [FK_MonatsplanTagesplan]
    FOREIGN KEY ([MonatsplanId])
    REFERENCES [dbo].[Monatspläne]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MonatsplanTagesplan'
CREATE INDEX [IX_FK_MonatsplanTagesplan]
ON [dbo].[Tagespläne]
    ([MonatsplanId]);
GO

-- Creating foreign key on [FachId] in table 'Jahrespläne'
ALTER TABLE [dbo].[Jahrespläne]
ADD CONSTRAINT [FK_FachJahresplan]
    FOREIGN KEY ([FachId])
    REFERENCES [dbo].[Fächer]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_FachJahresplan'
CREATE INDEX [IX_FK_FachJahresplan]
ON [dbo].[Jahrespläne]
    ([FachId]);
GO

-- Creating foreign key on [KlasseId] in table 'Jahrespläne'
ALTER TABLE [dbo].[Jahrespläne]
ADD CONSTRAINT [FK_KlasseJahresplan]
    FOREIGN KEY ([KlasseId])
    REFERENCES [dbo].[Klassen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_KlasseJahresplan'
CREATE INDEX [IX_FK_KlasseJahresplan]
ON [dbo].[Jahrespläne]
    ([KlasseId]);
GO

-- Creating foreign key on [TagesplanId] in table 'Termine_Lerngruppentermin'
ALTER TABLE [dbo].[Termine_Lerngruppentermin]
ADD CONSTRAINT [FK_TagesplanLerngruppentermin]
    FOREIGN KEY ([TagesplanId])
    REFERENCES [dbo].[Tagespläne]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TagesplanLerngruppentermin'
CREATE INDEX [IX_FK_TagesplanLerngruppentermin]
ON [dbo].[Termine_Lerngruppentermin]
    ([TagesplanId]);
GO

-- Creating foreign key on [JahresplanId] in table 'Halbjahrespläne'
ALTER TABLE [dbo].[Halbjahrespläne]
ADD CONSTRAINT [FK_JahresplanHalbjahresplan]
    FOREIGN KEY ([JahresplanId])
    REFERENCES [dbo].[Jahrespläne]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_JahresplanHalbjahresplan'
CREATE INDEX [IX_FK_JahresplanHalbjahresplan]
ON [dbo].[Halbjahrespläne]
    ([JahresplanId]);
GO

-- Creating foreign key on [JahrtypId] in table 'Jahrespläne'
ALTER TABLE [dbo].[Jahrespläne]
ADD CONSTRAINT [FK_JahrtypJahresplan]
    FOREIGN KEY ([JahrtypId])
    REFERENCES [dbo].[Jahrtypen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_JahrtypJahresplan'
CREATE INDEX [IX_FK_JahrtypJahresplan]
ON [dbo].[Jahrespläne]
    ([JahrtypId]);
GO

-- Creating foreign key on [HalbjahrtypId] in table 'Stundenpläne'
ALTER TABLE [dbo].[Stundenpläne]
ADD CONSTRAINT [FK_HalbjahrtypStundenplan]
    FOREIGN KEY ([HalbjahrtypId])
    REFERENCES [dbo].[Halbjahrtypen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_HalbjahrtypStundenplan'
CREATE INDEX [IX_FK_HalbjahrtypStundenplan]
ON [dbo].[Stundenpläne]
    ([HalbjahrtypId]);
GO

-- Creating foreign key on [JahrtypId] in table 'Stundenpläne'
ALTER TABLE [dbo].[Stundenpläne]
ADD CONSTRAINT [FK_JahrtypStundenplan]
    FOREIGN KEY ([JahrtypId])
    REFERENCES [dbo].[Jahrtypen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_JahrtypStundenplan'
CREATE INDEX [IX_FK_JahrtypStundenplan]
ON [dbo].[Stundenpläne]
    ([JahrtypId]);
GO

-- Creating foreign key on [StundenplanId] in table 'Stundenplaneinträge'
ALTER TABLE [dbo].[Stundenplaneinträge]
ADD CONSTRAINT [FK_StundenplanStundenplaneintrag]
    FOREIGN KEY ([StundenplanId])
    REFERENCES [dbo].[Stundenpläne]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StundenplanStundenplaneintrag'
CREATE INDEX [IX_FK_StundenplanStundenplaneintrag]
ON [dbo].[Stundenplaneinträge]
    ([StundenplanId]);
GO

-- Creating foreign key on [JahrtypId] in table 'Ferien'
ALTER TABLE [dbo].[Ferien]
ADD CONSTRAINT [FK_JahrtypFerien]
    FOREIGN KEY ([JahrtypId])
    REFERENCES [dbo].[Jahrtypen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_JahrtypFerien'
CREATE INDEX [IX_FK_JahrtypFerien]
ON [dbo].[Ferien]
    ([JahrtypId]);
GO

-- Creating foreign key on [TerminId] in table 'BetroffeneKlassen'
ALTER TABLE [dbo].[BetroffeneKlassen]
ADD CONSTRAINT [FK_TerminBetroffeneKlasse]
    FOREIGN KEY ([TerminId])
    REFERENCES [dbo].[Termine_Schultermin]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TerminBetroffeneKlasse'
CREATE INDEX [IX_FK_TerminBetroffeneKlasse]
ON [dbo].[BetroffeneKlassen]
    ([TerminId]);
GO

-- Creating foreign key on [JahrgangsstufeId] in table 'Stundenentwürfe'
ALTER TABLE [dbo].[Stundenentwürfe]
ADD CONSTRAINT [FK_JahrgangsstufeStundenentwurf]
    FOREIGN KEY ([JahrgangsstufeId])
    REFERENCES [dbo].[Jahrgangsstufen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_JahrgangsstufeStundenentwurf'
CREATE INDEX [IX_FK_JahrgangsstufeStundenentwurf]
ON [dbo].[Stundenentwürfe]
    ([JahrgangsstufeId]);
GO

-- Creating foreign key on [KlasseId] in table 'BetroffeneKlassen'
ALTER TABLE [dbo].[BetroffeneKlassen]
ADD CONSTRAINT [FK_KlasseBetroffeneKlasse]
    FOREIGN KEY ([KlasseId])
    REFERENCES [dbo].[Klassen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_KlasseBetroffeneKlasse'
CREATE INDEX [IX_FK_KlasseBetroffeneKlasse]
ON [dbo].[BetroffeneKlassen]
    ([KlasseId]);
GO

-- Creating foreign key on [DateitypId] in table 'Dateiverweise'
ALTER TABLE [dbo].[Dateiverweise]
ADD CONSTRAINT [FK_DateitypDateiverweis]
    FOREIGN KEY ([DateitypId])
    REFERENCES [dbo].[Dateitypen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DateitypDateiverweis'
CREATE INDEX [IX_FK_DateitypDateiverweis]
ON [dbo].[Dateiverweise]
    ([DateitypId]);
GO

-- Creating foreign key on [StundenentwurfId] in table 'Dateiverweise'
ALTER TABLE [dbo].[Dateiverweise]
ADD CONSTRAINT [FK_StundenentwurfDateiverweis]
    FOREIGN KEY ([StundenentwurfId])
    REFERENCES [dbo].[Stundenentwürfe]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StundenentwurfDateiverweis'
CREATE INDEX [IX_FK_StundenentwurfDateiverweis]
ON [dbo].[Dateiverweise]
    ([StundenentwurfId]);
GO

-- Creating foreign key on [ErsteUnterrichtsstundeId] in table 'Termine'
ALTER TABLE [dbo].[Termine]
ADD CONSTRAINT [FK_ErsteUnterrichtsstundeTermin]
    FOREIGN KEY ([ErsteUnterrichtsstundeId])
    REFERENCES [dbo].[Unterrichtsstunden]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ErsteUnterrichtsstundeTermin'
CREATE INDEX [IX_FK_ErsteUnterrichtsstundeTermin]
ON [dbo].[Termine]
    ([ErsteUnterrichtsstundeId]);
GO

-- Creating foreign key on [LetzteUnterrichtsstundeId] in table 'Termine'
ALTER TABLE [dbo].[Termine]
ADD CONSTRAINT [FK_UnterrichtsstundeTermin]
    FOREIGN KEY ([LetzteUnterrichtsstundeId])
    REFERENCES [dbo].[Unterrichtsstunden]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UnterrichtsstundeTermin'
CREATE INDEX [IX_FK_UnterrichtsstundeTermin]
ON [dbo].[Termine]
    ([LetzteUnterrichtsstundeId]);
GO

-- Creating foreign key on [TermintypId] in table 'Termine'
ALTER TABLE [dbo].[Termine]
ADD CONSTRAINT [FK_TermintypTermin]
    FOREIGN KEY ([TermintypId])
    REFERENCES [dbo].[Termintypen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TermintypTermin'
CREATE INDEX [IX_FK_TermintypTermin]
ON [dbo].[Termine]
    ([TermintypId]);
GO

-- Creating foreign key on [ModulId] in table 'Reihen'
ALTER TABLE [dbo].[Reihen]
ADD CONSTRAINT [FK_ModulReihe]
    FOREIGN KEY ([ModulId])
    REFERENCES [dbo].[Module]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ModulReihe'
CREATE INDEX [IX_FK_ModulReihe]
ON [dbo].[Reihen]
    ([ModulId]);
GO

-- Creating foreign key on [FachId] in table 'Curricula'
ALTER TABLE [dbo].[Curricula]
ADD CONSTRAINT [FK_FachCurriculum]
    FOREIGN KEY ([FachId])
    REFERENCES [dbo].[Fächer]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_FachCurriculum'
CREATE INDEX [IX_FK_FachCurriculum]
ON [dbo].[Curricula]
    ([FachId]);
GO

-- Creating foreign key on [ReiheId] in table 'Sequenzen'
ALTER TABLE [dbo].[Sequenzen]
ADD CONSTRAINT [FK_ReiheSequenz]
    FOREIGN KEY ([ReiheId])
    REFERENCES [dbo].[Reihen]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ReiheSequenz'
CREATE INDEX [IX_FK_ReiheSequenz]
ON [dbo].[Sequenzen]
    ([ReiheId]);
GO

-- Creating foreign key on [JahrtypId] in table 'Termine_Schultermin'
ALTER TABLE [dbo].[Termine_Schultermin]
ADD CONSTRAINT [FK_JahrtypSchultermin]
    FOREIGN KEY ([JahrtypId])
    REFERENCES [dbo].[Jahrtypen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_JahrtypSchultermin'
CREATE INDEX [IX_FK_JahrtypSchultermin]
ON [dbo].[Termine_Schultermin]
    ([JahrtypId]);
GO

-- Creating foreign key on [JahrtypId] in table 'Curricula'
ALTER TABLE [dbo].[Curricula]
ADD CONSTRAINT [FK_JahrtypCurriculum]
    FOREIGN KEY ([JahrtypId])
    REFERENCES [dbo].[Jahrtypen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_JahrtypCurriculum'
CREATE INDEX [IX_FK_JahrtypCurriculum]
ON [dbo].[Curricula]
    ([JahrtypId]);
GO

-- Creating foreign key on [FachId] in table 'Fachstundenanzahlen'
ALTER TABLE [dbo].[Fachstundenanzahlen]
ADD CONSTRAINT [FK_FachFachstundenanzahl]
    FOREIGN KEY ([FachId])
    REFERENCES [dbo].[Fächer]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_FachFachstundenanzahl'
CREATE INDEX [IX_FK_FachFachstundenanzahl]
ON [dbo].[Fachstundenanzahlen]
    ([FachId]);
GO

-- Creating foreign key on [JahrgangsstufeId] in table 'Klassenstufen'
ALTER TABLE [dbo].[Klassenstufen]
ADD CONSTRAINT [FK_JahrgangsstufeKlassenstufe]
    FOREIGN KEY ([JahrgangsstufeId])
    REFERENCES [dbo].[Jahrgangsstufen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_JahrgangsstufeKlassenstufe'
CREATE INDEX [IX_FK_JahrgangsstufeKlassenstufe]
ON [dbo].[Klassenstufen]
    ([JahrgangsstufeId]);
GO

-- Creating foreign key on [KlassenstufeId] in table 'Fachstundenanzahlen'
ALTER TABLE [dbo].[Fachstundenanzahlen]
ADD CONSTRAINT [FK_KlassenstufeFachstundenanzahl]
    FOREIGN KEY ([KlassenstufeId])
    REFERENCES [dbo].[Klassenstufen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_KlassenstufeFachstundenanzahl'
CREATE INDEX [IX_FK_KlassenstufeFachstundenanzahl]
ON [dbo].[Fachstundenanzahlen]
    ([KlassenstufeId]);
GO

-- Creating foreign key on [KlassenstufeId] in table 'Curricula'
ALTER TABLE [dbo].[Curricula]
ADD CONSTRAINT [FK_KlassenstufeCurriculum]
    FOREIGN KEY ([KlassenstufeId])
    REFERENCES [dbo].[Klassenstufen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_KlassenstufeCurriculum'
CREATE INDEX [IX_FK_KlassenstufeCurriculum]
ON [dbo].[Curricula]
    ([KlassenstufeId]);
GO

-- Creating foreign key on [KlassenstufeId] in table 'Klassen'
ALTER TABLE [dbo].[Klassen]
ADD CONSTRAINT [FK_KlassenstufeKlasse]
    FOREIGN KEY ([KlassenstufeId])
    REFERENCES [dbo].[Klassenstufen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_KlassenstufeKlasse'
CREATE INDEX [IX_FK_KlassenstufeKlasse]
ON [dbo].[Klassen]
    ([KlassenstufeId]);
GO

-- Creating foreign key on [CurriculumId] in table 'Reihen'
ALTER TABLE [dbo].[Reihen]
ADD CONSTRAINT [FK_CurriculumReihe]
    FOREIGN KEY ([CurriculumId])
    REFERENCES [dbo].[Curricula]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CurriculumReihe'
CREATE INDEX [IX_FK_CurriculumReihe]
ON [dbo].[Reihen]
    ([CurriculumId]);
GO

-- Creating foreign key on [JahrtypId] in table 'Schulwochen'
ALTER TABLE [dbo].[Schulwochen]
ADD CONSTRAINT [FK_JahrtypSchulwoche]
    FOREIGN KEY ([JahrtypId])
    REFERENCES [dbo].[Jahrtypen]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_JahrtypSchulwoche'
CREATE INDEX [IX_FK_JahrtypSchulwoche]
ON [dbo].[Schulwochen]
    ([JahrtypId]);
GO

-- Creating foreign key on [SchulwocheId] in table 'Schultage'
ALTER TABLE [dbo].[Schultage]
ADD CONSTRAINT [FK_SchulwocheSchultag]
    FOREIGN KEY ([SchulwocheId])
    REFERENCES [dbo].[Schulwochen]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SchulwocheSchultag'
CREATE INDEX [IX_FK_SchulwocheSchultag]
ON [dbo].[Schultage]
    ([SchulwocheId]);
GO

-- Creating foreign key on [TermintypId] in table 'Schultage'
ALTER TABLE [dbo].[Schultage]
ADD CONSTRAINT [FK_TermintypSchultag]
    FOREIGN KEY ([TermintypId])
    REFERENCES [dbo].[Termintypen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TermintypSchultag'
CREATE INDEX [IX_FK_TermintypSchultag]
ON [dbo].[Schultage]
    ([TermintypId]);
GO

-- Creating foreign key on [AufgabeId] in table 'Ergebnisse'
ALTER TABLE [dbo].[Ergebnisse]
ADD CONSTRAINT [FK_AufgabeErgebnis]
    FOREIGN KEY ([AufgabeId])
    REFERENCES [dbo].[Aufgaben]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AufgabeErgebnis'
CREATE INDEX [IX_FK_AufgabeErgebnis]
ON [dbo].[Ergebnisse]
    ([AufgabeId]);
GO

-- Creating foreign key on [ArbeitId] in table 'Aufgaben'
ALTER TABLE [dbo].[Aufgaben]
ADD CONSTRAINT [FK_ArbeitAufgabe]
    FOREIGN KEY ([ArbeitId])
    REFERENCES [dbo].[Arbeiten]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ArbeitAufgabe'
CREATE INDEX [IX_FK_ArbeitAufgabe]
ON [dbo].[Aufgaben]
    ([ArbeitId]);
GO

-- Creating foreign key on [NotentypId] in table 'Arbeiten'
ALTER TABLE [dbo].[Arbeiten]
ADD CONSTRAINT [FK_NotentypArbeit]
    FOREIGN KEY ([NotentypId])
    REFERENCES [dbo].[Notentypen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_NotentypArbeit'
CREATE INDEX [IX_FK_NotentypArbeit]
ON [dbo].[Arbeiten]
    ([NotentypId]);
GO

-- Creating foreign key on [KlasseId] in table 'Arbeiten'
ALTER TABLE [dbo].[Arbeiten]
ADD CONSTRAINT [FK_KlasseArbeit]
    FOREIGN KEY ([KlasseId])
    REFERENCES [dbo].[Klassen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_KlasseArbeit'
CREATE INDEX [IX_FK_KlasseArbeit]
ON [dbo].[Arbeiten]
    ([KlasseId]);
GO

-- Creating foreign key on [NotentypId] in table 'Noten'
ALTER TABLE [dbo].[Noten]
ADD CONSTRAINT [FK_NotentypNoten]
    FOREIGN KEY ([NotentypId])
    REFERENCES [dbo].[Notentypen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_NotentypNoten'
CREATE INDEX [IX_FK_NotentypNoten]
ON [dbo].[Noten]
    ([NotentypId]);
GO

-- Creating foreign key on [ZensurId] in table 'Noten'
ALTER TABLE [dbo].[Noten]
ADD CONSTRAINT [FK_ZensurNoten]
    FOREIGN KEY ([ZensurId])
    REFERENCES [dbo].[Zensuren]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ZensurNoten'
CREATE INDEX [IX_FK_ZensurNoten]
ON [dbo].[Noten]
    ([ZensurId]);
GO

-- Creating foreign key on [JahrtypId] in table 'Arbeiten'
ALTER TABLE [dbo].[Arbeiten]
ADD CONSTRAINT [FK_JahrtypArbeit]
    FOREIGN KEY ([JahrtypId])
    REFERENCES [dbo].[Jahrtypen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_JahrtypArbeit'
CREATE INDEX [IX_FK_JahrtypArbeit]
ON [dbo].[Arbeiten]
    ([JahrtypId]);
GO

-- Creating foreign key on [HalbjahrtypId] in table 'Arbeiten'
ALTER TABLE [dbo].[Arbeiten]
ADD CONSTRAINT [FK_HalbjahrtypArbeit]
    FOREIGN KEY ([HalbjahrtypId])
    REFERENCES [dbo].[Halbjahrtypen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_HalbjahrtypArbeit'
CREATE INDEX [IX_FK_HalbjahrtypArbeit]
ON [dbo].[Arbeiten]
    ([HalbjahrtypId]);
GO

-- Creating foreign key on [HalbjahrtypId] in table 'Schülerlisten'
ALTER TABLE [dbo].[Schülerlisten]
ADD CONSTRAINT [FK_HalbjahrtypSchülerliste]
    FOREIGN KEY ([HalbjahrtypId])
    REFERENCES [dbo].[Halbjahrtypen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_HalbjahrtypSchülerliste'
CREATE INDEX [IX_FK_HalbjahrtypSchülerliste]
ON [dbo].[Schülerlisten]
    ([HalbjahrtypId]);
GO

-- Creating foreign key on [KlasseId] in table 'Schülerlisten'
ALTER TABLE [dbo].[Schülerlisten]
ADD CONSTRAINT [FK_KlasseSchülerliste]
    FOREIGN KEY ([KlasseId])
    REFERENCES [dbo].[Klassen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_KlasseSchülerliste'
CREATE INDEX [IX_FK_KlasseSchülerliste]
ON [dbo].[Schülerlisten]
    ([KlasseId]);
GO

-- Creating foreign key on [SchülerlisteId] in table 'Schülereinträge'
ALTER TABLE [dbo].[Schülereinträge]
ADD CONSTRAINT [FK_SchülerlisteSchülereintrag]
    FOREIGN KEY ([SchülerlisteId])
    REFERENCES [dbo].[Schülerlisten]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SchülerlisteSchülereintrag'
CREATE INDEX [IX_FK_SchülerlisteSchülereintrag]
ON [dbo].[Schülereinträge]
    ([SchülerlisteId]);
GO

-- Creating foreign key on [SchülereintragId] in table 'Ergebnisse'
ALTER TABLE [dbo].[Ergebnisse]
ADD CONSTRAINT [FK_SchülereintragErgebnis]
    FOREIGN KEY ([SchülereintragId])
    REFERENCES [dbo].[Schülereinträge]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SchülereintragErgebnis'
CREATE INDEX [IX_FK_SchülereintragErgebnis]
ON [dbo].[Ergebnisse]
    ([SchülereintragId]);
GO

-- Creating foreign key on [PersonId] in table 'Schülereinträge'
ALTER TABLE [dbo].[Schülereinträge]
ADD CONSTRAINT [FK_PersonSchülereintrag]
    FOREIGN KEY ([PersonId])
    REFERENCES [dbo].[Personen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PersonSchülereintrag'
CREATE INDEX [IX_FK_PersonSchülereintrag]
ON [dbo].[Schülereinträge]
    ([PersonId]);
GO

-- Creating foreign key on [FachId] in table 'Schülerlisten'
ALTER TABLE [dbo].[Schülerlisten]
ADD CONSTRAINT [FK_FachSchülerliste]
    FOREIGN KEY ([FachId])
    REFERENCES [dbo].[Fächer]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_FachSchülerliste'
CREATE INDEX [IX_FK_FachSchülerliste]
ON [dbo].[Schülerlisten]
    ([FachId]);
GO

-- Creating foreign key on [JahrtypId] in table 'Schülerlisten'
ALTER TABLE [dbo].[Schülerlisten]
ADD CONSTRAINT [FK_JahrtypSchülerliste]
    FOREIGN KEY ([JahrtypId])
    REFERENCES [dbo].[Jahrtypen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_JahrtypSchülerliste'
CREATE INDEX [IX_FK_JahrtypSchülerliste]
ON [dbo].[Schülerlisten]
    ([JahrtypId]);
GO

-- Creating foreign key on [JahrtypId] in table 'Schülerlisten'
ALTER TABLE [dbo].[Schülerlisten]
ADD CONSTRAINT [FK_JahrtypSchülerliste]
    FOREIGN KEY ([JahrtypId])
    REFERENCES [dbo].[Jahrtypen]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_JahrtypSchülerliste'
CREATE INDEX [IX_FK_JahrtypSchülerliste]
ON [dbo].[Schülerlisten]
    ([JahrtypId]);
GO

-- Creating foreign key on [Id] in table 'Termine_Lerngruppentermin'
ALTER TABLE [dbo].[Termine_Lerngruppentermin]
ADD CONSTRAINT [FK_Lerngruppentermin_inherits_Termin]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Termine]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Termine_Stunde'
ALTER TABLE [dbo].[Termine_Stunde]
ADD CONSTRAINT [FK_Stunde_inherits_Lerngruppentermin]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Termine_Lerngruppentermin]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Termine_Schultermin'
ALTER TABLE [dbo].[Termine_Schultermin]
ADD CONSTRAINT [FK_Schultermin_inherits_Termin]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Termine]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------