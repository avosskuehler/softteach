USE [master]
GO
/****** Object:  Database [Liduv]    Script Date: 26.07.2013 14:58:52 ******/
CREATE DATABASE [Liduv]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Liduv', FILENAME = N'C:\SQLDatenbanken\Liduv.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'Liduv_log', FILENAME = N'C:\SQLDatenbanken\Liduv_log.ldf' , SIZE = 3456KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [Liduv] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Liduv].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Liduv] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Liduv] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Liduv] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Liduv] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Liduv] SET ARITHABORT OFF 
GO
ALTER DATABASE [Liduv] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [Liduv] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [Liduv] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Liduv] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Liduv] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Liduv] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Liduv] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Liduv] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Liduv] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Liduv] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Liduv] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Liduv] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Liduv] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Liduv] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Liduv] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Liduv] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Liduv] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Liduv] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Liduv] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Liduv] SET  MULTI_USER 
GO
ALTER DATABASE [Liduv] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Liduv] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Liduv] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Liduv] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [Liduv]
GO
/****** Object:  Table [dbo].[Arbeiten]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Arbeiten](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Bezeichnung] [varchar](max) NOT NULL,
	[JahrtypId] [int] NOT NULL,
	[HalbjahrtypId] [int] NOT NULL,
	[KlasseId] [int] NOT NULL,
	[FachId] [int] NOT NULL,
	[LfdNr] [int] NOT NULL,
	[BewertungsschemaId] [int] NOT NULL,
	[Bepunktungstyp] [nvarchar](50) NOT NULL,
	[Datum] [datetime] NOT NULL,
	[IstKlausur] [bit] NOT NULL,
 CONSTRAINT [PK_Arbeiten] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Aufgaben]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Aufgaben](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ArbeitId] [int] NOT NULL,
	[LfdNr] [int] NOT NULL,
	[MaxPunkte] [int] NOT NULL,
	[Bezeichnung] [nvarchar](max) NULL,
 CONSTRAINT [PK_Aufgaben] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BetroffeneKlassen]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BetroffeneKlassen](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TerminId] [int] NOT NULL,
	[KlasseId] [int] NOT NULL,
 CONSTRAINT [PK_BetroffeneKlassen] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Bewertungsschemata]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bewertungsschemata](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Bezeichnung] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_Bewertungsschemata] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Curricula]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Curricula](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FachId] [int] NOT NULL,
	[KlassenstufeId] [int] NOT NULL,
	[Bezeichnung] [nvarchar](max) NOT NULL,
	[JahrtypId] [int] NOT NULL,
	[HalbjahrtypId] [int] NOT NULL,
 CONSTRAINT [PK_Curricula] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Dateitypen]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Dateitypen](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Bezeichnung] [nvarchar](max) NOT NULL,
	[Kürzel] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Dateitypen] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Dateiverweise]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Dateiverweise](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Dateiname] [nvarchar](max) NOT NULL,
	[DateitypId] [int] NOT NULL,
	[StundenentwurfId] [int] NOT NULL,
 CONSTRAINT [PK_Dateiverweise] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Ergebnisse]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ergebnisse](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AufgabeId] [int] NOT NULL,
	[SchülereintragId] [int] NOT NULL,
	[Punktzahl] [int] NULL,
 CONSTRAINT [PK_Ergebnisse] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Fächer]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Fächer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Bezeichnung] [nvarchar](max) NOT NULL,
	[Farbe] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Fächer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Fachstundenanzahlen]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Fachstundenanzahlen](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FachId] [int] NOT NULL,
	[KlassenstufeId] [int] NOT NULL,
	[Stundenzahl] [int] NOT NULL,
	[Teilungsstundenzahl] [int] NOT NULL,
 CONSTRAINT [PK_Fachstundenanzahlen] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Ferien]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ferien](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JahrtypId] [int] NOT NULL,
	[Bezeichnung] [nvarchar](max) NOT NULL,
	[ErsterFerientag] [datetime] NOT NULL,
	[LetzterFerientag] [datetime] NOT NULL,
 CONSTRAINT [PK_Ferien] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Halbjahrespläne]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Halbjahrespläne](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JahresplanId] [int] NOT NULL,
	[HalbjahrtypId] [int] NOT NULL,
 CONSTRAINT [PK_Halbjahrespläne] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Halbjahrtypen]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Halbjahrtypen](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Bezeichnung] [nvarchar](max) NOT NULL,
	[HalbjahrIndex] [int] NOT NULL,
 CONSTRAINT [PK_Halbjahrtypen] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Hausaufgaben]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Hausaufgaben](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Datum] [datetime] NOT NULL,
	[Bezeichnung] [nvarchar](50) NULL,
	[IstNachgereicht] [bit] NOT NULL,
	[SchülereintragId] [int] NOT NULL,
 CONSTRAINT [PK_Hausaufgaben] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Jahrespläne]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Jahrespläne](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FachId] [int] NOT NULL,
	[KlasseId] [int] NOT NULL,
	[JahrtypId] [int] NOT NULL,
 CONSTRAINT [PK_Jahrespläne] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Jahrgangsstufen]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Jahrgangsstufen](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Bezeichnung] [nvarchar](max) NOT NULL,
	[Bepunktungstyp] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Jahrgangsstufen] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Jahrtypen]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Jahrtypen](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Bezeichnung] [nvarchar](max) NOT NULL,
	[Jahr] [int] NOT NULL,
 CONSTRAINT [PK_Jahrtypen] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Klassen]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Klassen](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[KlassenstufeId] [int] NOT NULL,
	[Bezeichnung] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Klassen] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Klassenstufen]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Klassenstufen](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JahrgangsstufeId] [int] NOT NULL,
	[Bezeichnung] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Klassenstufen] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Medien]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Medien](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Bezeichnung] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Medien] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Module]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Module](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FachId] [int] NOT NULL,
	[Bezeichnung] [nvarchar](max) NOT NULL,
	[JahrgangsstufeId] [int] NOT NULL,
	[Bausteine] [nvarchar](max) NULL,
	[Stundenbedarf] [int] NOT NULL,
 CONSTRAINT [PK_Module] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Monatspläne]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Monatspläne](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HalbjahresplanId] [int] NOT NULL,
	[MonatstypId] [int] NOT NULL,
	[Monatstyp_Id] [int] NOT NULL,
 CONSTRAINT [PK_Monatspläne] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Monatstypen]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Monatstypen](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Bezeichnung] [nvarchar](max) NOT NULL,
	[MonatIndex] [int] NOT NULL,
 CONSTRAINT [PK_Monatstypen] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Noten]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Noten](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SchülereintragId] [int] NOT NULL,
	[ZensurId] [int] NOT NULL,
	[Datum] [datetime] NOT NULL,
	[Wichtung] [int] NOT NULL,
	[IstSchriftlich] [bit] NOT NULL,
	[ArbeitId] [int] NULL,
	[Bezeichnung] [nvarchar](max) NULL,
	[Notentyp] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Noten] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Notentendenzen]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notentendenzen](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TendenztypId] [int] NOT NULL,
	[Datum] [datetime] NOT NULL,
	[TendenzId] [int] NOT NULL,
	[Bezeichnung] [nvarchar](max) NULL,
	[SchülereintragId] [int] NOT NULL,
 CONSTRAINT [PK_Notentendenzen] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[NotenWichtungen]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NotenWichtungen](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Bezeichnung] [nvarchar](max) NOT NULL,
	[MündlichQualität] [real] NOT NULL,
	[MündlichQuantität] [real] NOT NULL,
	[MündlichSonstige] [real] NOT NULL,
	[MündlichGesamt] [real] NOT NULL,
	[SchriftlichKlassenarbeit] [real] NOT NULL,
	[SchriftlichSonstige] [real] NOT NULL,
	[SchriftlichGesamt] [real] NOT NULL,
 CONSTRAINT [PK_Notentypen] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Personen]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Personen](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Vorname] [nvarchar](max) NOT NULL,
	[Nachname] [nvarchar](max) NOT NULL,
	[Geschlecht] [bit] NOT NULL,
	[Geburtstag] [datetime] NULL,
	[Titel] [nvarchar](max) NULL,
	[Telefon] [nvarchar](max) NULL,
	[Fax] [nvarchar](max) NULL,
	[Handy] [nvarchar](max) NULL,
	[EMail] [nvarchar](max) NULL,
	[PLZ] [nvarchar](max) NULL,
	[Straße] [nvarchar](max) NULL,
	[Hausnummer] [nvarchar](max) NULL,
	[Ort] [nvarchar](max) NULL,
	[IstLehrer] [bit] NOT NULL,
	[Foto] [image] NULL,
 CONSTRAINT [PK_Personen] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Phasen]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Phasen](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StundenentwurfId] [int] NOT NULL,
	[Zeit] [int] NOT NULL,
	[MediumId] [int] NOT NULL,
	[SozialformId] [int] NOT NULL,
	[Inhalt] [nvarchar](max) NOT NULL,
	[AbfolgeIndex] [int] NOT NULL,
 CONSTRAINT [PK_Phasen] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[StundenentwurfId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Prozentbereiche]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Prozentbereiche](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ZensurId] [int] NOT NULL,
	[VonProzent] [float] NOT NULL,
	[BisProzent] [float] NOT NULL,
	[BewertungsschemaId] [int] NOT NULL,
 CONSTRAINT [PK_Prozentbereich] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Reihen]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reihen](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ModulId] [int] NOT NULL,
	[Thema] [nvarchar](max) NOT NULL,
	[Stundenbedarf] [int] NOT NULL,
	[CurriculumId] [int] NOT NULL,
	[AbfolgeIndex] [int] NOT NULL,
 CONSTRAINT [PK_Reihen] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Schülereinträge]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Schülereinträge](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SchülerlisteId] [int] NOT NULL,
	[PersonId] [int] NOT NULL,
 CONSTRAINT [PK_Schülereinträge] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Schülerlisten]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Schülerlisten](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JahrtypId] [int] NOT NULL,
	[HalbjahrtypId] [int] NOT NULL,
	[KlasseId] [int] NOT NULL,
	[FachId] [int] NOT NULL,
	[NotenWichtungId] [int] NOT NULL,
 CONSTRAINT [PK_Schülerlisten] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Schultage]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Schultage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SchulwocheId] [int] NOT NULL,
	[TermintypId] [int] NOT NULL,
	[Datum] [datetime] NOT NULL,
 CONSTRAINT [PK_Schultage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Schulwochen]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Schulwochen](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JahrtypId] [int] NOT NULL,
	[Montagsdatum] [datetime] NOT NULL,
 CONSTRAINT [PK_Schulwochen] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Sequenzen]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sequenzen](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReiheId] [int] NOT NULL,
	[AbfolgeIndex] [int] NOT NULL,
	[Stundenbedarf] [int] NOT NULL,
	[Thema] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Sequenzen] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Sozialformen]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sozialformen](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Bezeichnung] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Sozialformen] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Stundenentwürfe]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Stundenentwürfe](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FachId] [int] NOT NULL,
	[JahrgangsstufeId] [int] NOT NULL,
	[ModulId] [int] NOT NULL,
	[Stundenthema] [nvarchar](max) NOT NULL,
	[Kopieren] [bit] NOT NULL,
	[Computer] [bit] NOT NULL,
	[Hausaufgaben] [nvarchar](max) NOT NULL,
	[Ansagen] [nvarchar](max) NOT NULL,
	[Datum] [datetime] NOT NULL,
	[Stundenzahl] [int] NOT NULL,
 CONSTRAINT [PK_Stundenentwürfe] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Stundenpläne]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Stundenpläne](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Bezeichnung] [nvarchar](max) NOT NULL,
	[JahrtypId] [int] NOT NULL,
	[HalbjahrtypId] [int] NOT NULL,
	[GültigAb] [datetime] NOT NULL,
 CONSTRAINT [PK_Stundenpläne] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Stundenplaneinträge]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Stundenplaneinträge](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StundenplanId] [int] NOT NULL,
	[ErsteUnterrichtsstundeIndex] [int] NOT NULL,
	[LetzteUnterrichtsstundeIndex] [int] NOT NULL,
	[WochentagIndex] [int] NOT NULL,
	[FachId] [int] NOT NULL,
	[KlasseId] [int] NOT NULL,
	[Raum] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Stundenplaneinträge] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tagespläne]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tagespläne](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MonatsplanId] [int] NOT NULL,
	[Datum] [datetime] NOT NULL,
	[Ferientag] [bit] NOT NULL,
	[Beschreibung] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Tagespläne] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tendenzen]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tendenzen](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Bezeichnung] [nvarchar](50) NOT NULL,
	[Wichtung] [int] NOT NULL,
 CONSTRAINT [PK_Tendenzen] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tendenztypen]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tendenztypen](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Bezeichnung] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Tendenztypen] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Termine]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Termine](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TermintypId] [int] NOT NULL,
	[Beschreibung] [nvarchar](max) NULL,
	[ErsteUnterrichtsstundeId] [int] NOT NULL,
	[LetzteUnterrichtsstundeId] [int] NOT NULL,
	[Ort] [nvarchar](max) NULL,
	[IstGeprüft] [bit] NOT NULL,
 CONSTRAINT [PK_Termine] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Termine_Lerngruppentermin]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Termine_Lerngruppentermin](
	[TagesplanId] [int] NOT NULL,
	[Id] [int] NOT NULL,
 CONSTRAINT [PK_Termine_Lerngruppentermin] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Termine_Schultermin]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Termine_Schultermin](
	[JahrtypId] [int] NOT NULL,
	[Datum] [datetime] NOT NULL,
	[Id] [int] NOT NULL,
 CONSTRAINT [PK_Termine_Schultermin] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Termine_Stunde]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Termine_Stunde](
	[StundenentwurfId] [int] NULL,
	[Id] [int] NOT NULL,
 CONSTRAINT [PK_Termine_Stunde] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Termintypen]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Termintypen](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Bezeichnung] [nvarchar](max) NOT NULL,
	[Kalenderfarbe] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Termintypen] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Unterrichtsstunden]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Unterrichtsstunden](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Bezeichnung] [nvarchar](max) NOT NULL,
	[Beginn] [time](7) NOT NULL,
	[Ende] [time](7) NOT NULL,
	[Stundenindex] [int] NOT NULL,
 CONSTRAINT [PK_Unterrichtsstunden] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Zensuren]    Script Date: 26.07.2013 14:58:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Zensuren](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Notenpunkte] [int] NOT NULL,
	[NoteMitTendenz] [nvarchar](max) NOT NULL,
	[GanzeNote] [int] NOT NULL,
 CONSTRAINT [PK_Zensuren] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[Bewertungsschemata] ON 

INSERT [dbo].[Bewertungsschemata] ([Id], [Bezeichnung]) VALUES (1, N'Sek I')
INSERT [dbo].[Bewertungsschemata] ([Id], [Bezeichnung]) VALUES (2, N'Sek II')
SET IDENTITY_INSERT [dbo].[Bewertungsschemata] OFF
SET IDENTITY_INSERT [dbo].[Dateitypen] ON 
INSERT [dbo].[Dateitypen] ([Id], [Bezeichnung], [Kürzel]) VALUES (1, N'', N'')
INSERT [dbo].[Dateitypen] ([Id], [Bezeichnung], [Kürzel]) VALUES (2, N'Folie', N'OH')
INSERT [dbo].[Dateitypen] ([Id], [Bezeichnung], [Kürzel]) VALUES (3, N'Arbeitsblatt', N'AB')
INSERT [dbo].[Dateitypen] ([Id], [Bezeichnung], [Kürzel]) VALUES (4, N'Klausur', N'KL')
INSERT [dbo].[Dateitypen] ([Id], [Bezeichnung], [Kürzel]) VALUES (5, N'Klassenarbeit', N'KA')
INSERT [dbo].[Dateitypen] ([Id], [Bezeichnung], [Kürzel]) VALUES (6, N'Test', N'TE')
INSERT [dbo].[Dateitypen] ([Id], [Bezeichnung], [Kürzel]) VALUES (7, N'Tägliche Übungen', N'TÜ')
INSERT [dbo].[Dateitypen] ([Id], [Bezeichnung], [Kürzel]) VALUES (8, N'Tandembogen', N'TA')
INSERT [dbo].[Dateitypen] ([Id], [Bezeichnung], [Kürzel]) VALUES (9, N'Projekt', N'PR')
INSERT [dbo].[Dateitypen] ([Id], [Bezeichnung], [Kürzel]) VALUES (10, N'Präsentation', N'PP')
INSERT [dbo].[Dateitypen] ([Id], [Bezeichnung], [Kürzel]) VALUES (11, N'Wochenplan', N'WP')
INSERT [dbo].[Dateitypen] ([Id], [Bezeichnung], [Kürzel]) VALUES (12, N'Hausaufgabe', N'HA')
INSERT [dbo].[Dateitypen] ([Id], [Bezeichnung], [Kürzel]) VALUES (13, N'Unterrichtsentwurf', N'UE')
INSERT [dbo].[Dateitypen] ([Id], [Bezeichnung], [Kürzel]) VALUES (14, N'Spiel', N'SP')
INSERT [dbo].[Dateitypen] ([Id], [Bezeichnung], [Kürzel]) VALUES (15, N'Film', N'FI')
INSERT [dbo].[Dateitypen] ([Id], [Bezeichnung], [Kürzel]) VALUES (16, N'Bild', N'BI')
INSERT [dbo].[Dateitypen] ([Id], [Bezeichnung], [Kürzel]) VALUES (17, N'Literatur', N'LIT')
SET IDENTITY_INSERT [dbo].[Dateitypen] OFF
SET IDENTITY_INSERT [dbo].[Fächer] ON 

INSERT [dbo].[Fächer] ([Id], [Bezeichnung], [Farbe]) VALUES (1, N'Physik', N'Yellow')
INSERT [dbo].[Fächer] ([Id], [Bezeichnung], [Farbe]) VALUES (2, N'Mathematik', N'Blue')
INSERT [dbo].[Fächer] ([Id], [Bezeichnung], [Farbe]) VALUES (3, N'Biologie', N'Green')
INSERT [dbo].[Fächer] ([Id], [Bezeichnung], [Farbe]) VALUES (4, N'Musik', N'Red')
INSERT [dbo].[Fächer] ([Id], [Bezeichnung], [Farbe]) VALUES (5, N'Vertretungsstunden', N'Gray')
INSERT [dbo].[Fächer] ([Id], [Bezeichnung], [Farbe]) VALUES (6, N'Team', N'#FF555555')
INSERT [dbo].[Fächer] ([Id], [Bezeichnung], [Farbe]) VALUES (7, N'Klassenstunde', N'#FFFF9600')
SET IDENTITY_INSERT [dbo].[Fächer] OFF
SET IDENTITY_INSERT [dbo].[Fachstundenanzahlen] ON 
INSERT [dbo].[Fachstundenanzahlen] ([Id], [FachId], [KlassenstufeId], [Stundenzahl], [Teilungsstundenzahl]) VALUES (1, 1, 1, 1, 1)
INSERT [dbo].[Fachstundenanzahlen] ([Id], [FachId], [KlassenstufeId], [Stundenzahl], [Teilungsstundenzahl]) VALUES (2, 1, 2, 2, 0)
INSERT [dbo].[Fachstundenanzahlen] ([Id], [FachId], [KlassenstufeId], [Stundenzahl], [Teilungsstundenzahl]) VALUES (3, 1, 3, 2, 0)
INSERT [dbo].[Fachstundenanzahlen] ([Id], [FachId], [KlassenstufeId], [Stundenzahl], [Teilungsstundenzahl]) VALUES (4, 1, 4, 2, 0)
INSERT [dbo].[Fachstundenanzahlen] ([Id], [FachId], [KlassenstufeId], [Stundenzahl], [Teilungsstundenzahl]) VALUES (5, 1, 5, 3, 0)
INSERT [dbo].[Fachstundenanzahlen] ([Id], [FachId], [KlassenstufeId], [Stundenzahl], [Teilungsstundenzahl]) VALUES (6, 1, 6, 3, 0)
INSERT [dbo].[Fachstundenanzahlen] ([Id], [FachId], [KlassenstufeId], [Stundenzahl], [Teilungsstundenzahl]) VALUES (7, 2, 1, 4, 0)
INSERT [dbo].[Fachstundenanzahlen] ([Id], [FachId], [KlassenstufeId], [Stundenzahl], [Teilungsstundenzahl]) VALUES (8, 2, 2, 4, 0)
INSERT [dbo].[Fachstundenanzahlen] ([Id], [FachId], [KlassenstufeId], [Stundenzahl], [Teilungsstundenzahl]) VALUES (9, 2, 3, 4, 0)
INSERT [dbo].[Fachstundenanzahlen] ([Id], [FachId], [KlassenstufeId], [Stundenzahl], [Teilungsstundenzahl]) VALUES (10, 2, 4, 4, 0)
INSERT [dbo].[Fachstundenanzahlen] ([Id], [FachId], [KlassenstufeId], [Stundenzahl], [Teilungsstundenzahl]) VALUES (11, 2, 5, 4, 0)
INSERT [dbo].[Fachstundenanzahlen] ([Id], [FachId], [KlassenstufeId], [Stundenzahl], [Teilungsstundenzahl]) VALUES (12, 2, 6, 4, 0)
INSERT [dbo].[Fachstundenanzahlen] ([Id], [FachId], [KlassenstufeId], [Stundenzahl], [Teilungsstundenzahl]) VALUES (13, 1, 10, 5, 0)
INSERT [dbo].[Fachstundenanzahlen] ([Id], [FachId], [KlassenstufeId], [Stundenzahl], [Teilungsstundenzahl]) VALUES (14, 1, 9, 5, 0)
INSERT [dbo].[Fachstundenanzahlen] ([Id], [FachId], [KlassenstufeId], [Stundenzahl], [Teilungsstundenzahl]) VALUES (15, 2, 10, 5, 0)
INSERT [dbo].[Fachstundenanzahlen] ([Id], [FachId], [KlassenstufeId], [Stundenzahl], [Teilungsstundenzahl]) VALUES (16, 2, 9, 5, 0)
SET IDENTITY_INSERT [dbo].[Fachstundenanzahlen] OFF
SET IDENTITY_INSERT [dbo].[Ferien] ON 
INSERT [dbo].[Ferien] ([Id], [JahrtypId], [Bezeichnung], [ErsterFerientag], [LetzterFerientag]) VALUES (1, 2, N'Herbstferien', CAST(0x00009F7200000000 AS DateTime), CAST(0x00009F7C00000000 AS DateTime))
INSERT [dbo].[Ferien] ([Id], [JahrtypId], [Bezeichnung], [ErsterFerientag], [LetzterFerientag]) VALUES (2, 2, N'Weihnachtsferien', CAST(0x00009FC200000000 AS DateTime), CAST(0x00009FCD00000000 AS DateTime))
INSERT [dbo].[Ferien] ([Id], [JahrtypId], [Bezeichnung], [ErsterFerientag], [LetzterFerientag]) VALUES (3, 2, N'Winterferien', CAST(0x00009FE800000000 AS DateTime), CAST(0x00009FED00000000 AS DateTime))
INSERT [dbo].[Ferien] ([Id], [JahrtypId], [Bezeichnung], [ErsterFerientag], [LetzterFerientag]) VALUES (4, 2, N'Osterferien', CAST(0x0000A02700000000 AS DateTime), CAST(0x0000A03300000000 AS DateTime))
INSERT [dbo].[Ferien] ([Id], [JahrtypId], [Bezeichnung], [ErsterFerientag], [LetzterFerientag]) VALUES (5, 2, N'Sommerferien', CAST(0x0000A07600000000 AS DateTime), CAST(0x0000A0A200000000 AS DateTime))
INSERT [dbo].[Ferien] ([Id], [JahrtypId], [Bezeichnung], [ErsterFerientag], [LetzterFerientag]) VALUES (6, 3, N'Herbstferien', CAST(0x0000A0DD00000000 AS DateTime), CAST(0x0000A0E900000000 AS DateTime))
INSERT [dbo].[Ferien] ([Id], [JahrtypId], [Bezeichnung], [ErsterFerientag], [LetzterFerientag]) VALUES (7, 3, N'Weihnachtsferien', CAST(0x0000A13100000000 AS DateTime), CAST(0x0000A13C00000000 AS DateTime))
INSERT [dbo].[Ferien] ([Id], [JahrtypId], [Bezeichnung], [ErsterFerientag], [LetzterFerientag]) VALUES (8, 3, N'Winterferien', CAST(0x0000A15B00000000 AS DateTime), CAST(0x0000A16000000000 AS DateTime))
INSERT [dbo].[Ferien] ([Id], [JahrtypId], [Bezeichnung], [ErsterFerientag], [LetzterFerientag]) VALUES (9, 3, N'Osterferien', CAST(0x0000A18C00000000 AS DateTime), CAST(0x0000A19800000000 AS DateTime))
INSERT [dbo].[Ferien] ([Id], [JahrtypId], [Bezeichnung], [ErsterFerientag], [LetzterFerientag]) VALUES (10, 3, N'Sommerferien', CAST(0x0000A1E200000000 AS DateTime), CAST(0x0000A20D00000000 AS DateTime))
INSERT [dbo].[Ferien] ([Id], [JahrtypId], [Bezeichnung], [ErsterFerientag], [LetzterFerientag]) VALUES (11, 3, N'Himmelfahrt', CAST(0x0000A1B900000000 AS DateTime), CAST(0x0000A1BA00000000 AS DateTime))
INSERT [dbo].[Ferien] ([Id], [JahrtypId], [Bezeichnung], [ErsterFerientag], [LetzterFerientag]) VALUES (12, 3, N'Pfingsten', CAST(0x0000A1C400000000 AS DateTime), CAST(0x0000A1C500000000 AS DateTime))
INSERT [dbo].[Ferien] ([Id], [JahrtypId], [Bezeichnung], [ErsterFerientag], [LetzterFerientag]) VALUES (20, 8, N'Herbstferien', CAST(0x0000A24900000000 AS DateTime), CAST(0x0000A25400000000 AS DateTime))
INSERT [dbo].[Ferien] ([Id], [JahrtypId], [Bezeichnung], [ErsterFerientag], [LetzterFerientag]) VALUES (21, 8, N'Weihnachtsferien', CAST(0x0000A29D00000000 AS DateTime), CAST(0x0000A2A800000000 AS DateTime))
INSERT [dbo].[Ferien] ([Id], [JahrtypId], [Bezeichnung], [ErsterFerientag], [LetzterFerientag]) VALUES (22, 8, N'Winterferien', CAST(0x0000A2C700000000 AS DateTime), CAST(0x0000A2CB00000000 AS DateTime))
INSERT [dbo].[Ferien] ([Id], [JahrtypId], [Bezeichnung], [ErsterFerientag], [LetzterFerientag]) VALUES (23, 8, N'Osterferien', CAST(0x0000A30D00000000 AS DateTime), CAST(0x0000A31800000000 AS DateTime))
INSERT [dbo].[Ferien] ([Id], [JahrtypId], [Bezeichnung], [ErsterFerientag], [LetzterFerientag]) VALUES (24, 8, N'Himmelfahrt', CAST(0x0000A31E00000000 AS DateTime), CAST(0x0000A31F00000000 AS DateTime))
INSERT [dbo].[Ferien] ([Id], [JahrtypId], [Bezeichnung], [ErsterFerientag], [LetzterFerientag]) VALUES (25, 8, N'Pfingsten', CAST(0x0000A34500000000 AS DateTime), CAST(0x0000A34500000000 AS DateTime))
INSERT [dbo].[Ferien] ([Id], [JahrtypId], [Bezeichnung], [ErsterFerientag], [LetzterFerientag]) VALUES (26, 8, N'Sommerferien', CAST(0x0000A36300000000 AS DateTime), CAST(0x0000A38E00000000 AS DateTime))
SET IDENTITY_INSERT [dbo].[Ferien] OFF
SET IDENTITY_INSERT [dbo].[Halbjahrtypen] ON 
INSERT [dbo].[Halbjahrtypen] ([Id], [Bezeichnung], [HalbjahrIndex]) VALUES (1, N'Winter', 1)
INSERT [dbo].[Halbjahrtypen] ([Id], [Bezeichnung], [HalbjahrIndex]) VALUES (2, N'Sommer', 2)
SET IDENTITY_INSERT [dbo].[Halbjahrtypen] OFF
SET IDENTITY_INSERT [dbo].[Jahrgangsstufen] ON 
INSERT [dbo].[Jahrgangsstufen] ([Id], [Bezeichnung], [Bepunktungstyp]) VALUES (1, N'7/8', N'NoteMitTendenz')
INSERT [dbo].[Jahrgangsstufen] ([Id], [Bezeichnung], [Bepunktungstyp]) VALUES (2, N'9/10', N'NoteMitTendenz')
INSERT [dbo].[Jahrgangsstufen] ([Id], [Bezeichnung], [Bepunktungstyp]) VALUES (3, N'Grundkurse', N'Notenpunkte')
INSERT [dbo].[Jahrgangsstufen] ([Id], [Bezeichnung], [Bepunktungstyp]) VALUES (4, N'Personen', N'GanzeNote')
INSERT [dbo].[Jahrgangsstufen] ([Id], [Bezeichnung], [Bepunktungstyp]) VALUES (5, N'Leistungskurse', N'Notenpunkte')
SET IDENTITY_INSERT [dbo].[Jahrgangsstufen] OFF
SET IDENTITY_INSERT [dbo].[Jahrtypen] ON 

INSERT [dbo].[Jahrtypen] ([Id], [Bezeichnung], [Jahr]) VALUES (2, N'2011/2012', 2011)
INSERT [dbo].[Jahrtypen] ([Id], [Bezeichnung], [Jahr]) VALUES (3, N'2012/2013', 2012)
INSERT [dbo].[Jahrtypen] ([Id], [Bezeichnung], [Jahr]) VALUES (8, N'2013/2014', 2013)
SET IDENTITY_INSERT [dbo].[Jahrtypen] OFF
SET IDENTITY_INSERT [dbo].[Klassen] ON 
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (1, 1, N'7a')
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (2, 1, N'7b')
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (3, 1, N'7c')
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (4, 1, N'7d')
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (5, 2, N'8a')
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (6, 2, N'8b')
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (7, 2, N'8c')
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (8, 2, N'8d')
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (9, 3, N'9a')
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (10, 3, N'9b')
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (11, 3, N'9c')
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (12, 3, N'9d')
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (13, 4, N'10a')
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (14, 4, N'10b')
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (15, 4, N'10c')
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (16, 4, N'10d')
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (18, 5, N'GK1 Q1/2')
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (19, 5, N'GK2 Q1/2')
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (20, 5, N'GK3 Q1/2')
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (22, 6, N'GK1 Q3/4')
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (23, 6, N'GK2 Q3/4')
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (24, 6, N'GK3 Q3/4')
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (26, 3, N'9W')
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (27, 2, N'8W')
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (36, 9, N'LK Q1/2')
INSERT [dbo].[Klassen] ([Id], [KlassenstufeId], [Bezeichnung]) VALUES (37, 10, N'LK Q3/4')
SET IDENTITY_INSERT [dbo].[Klassen] OFF
SET IDENTITY_INSERT [dbo].[Klassenstufen] ON 

INSERT [dbo].[Klassenstufen] ([Id], [JahrgangsstufeId], [Bezeichnung]) VALUES (1, 1, N'7')
INSERT [dbo].[Klassenstufen] ([Id], [JahrgangsstufeId], [Bezeichnung]) VALUES (2, 1, N'8')
INSERT [dbo].[Klassenstufen] ([Id], [JahrgangsstufeId], [Bezeichnung]) VALUES (3, 2, N'9')
INSERT [dbo].[Klassenstufen] ([Id], [JahrgangsstufeId], [Bezeichnung]) VALUES (4, 2, N'10')
INSERT [dbo].[Klassenstufen] ([Id], [JahrgangsstufeId], [Bezeichnung]) VALUES (5, 3, N'GK-Q1/2')
INSERT [dbo].[Klassenstufen] ([Id], [JahrgangsstufeId], [Bezeichnung]) VALUES (6, 3, N'GK-Q3/4')
INSERT [dbo].[Klassenstufen] ([Id], [JahrgangsstufeId], [Bezeichnung]) VALUES (7, 4, N'Kollegen')
INSERT [dbo].[Klassenstufen] ([Id], [JahrgangsstufeId], [Bezeichnung]) VALUES (8, 4, N'Schulleitung')
INSERT [dbo].[Klassenstufen] ([Id], [JahrgangsstufeId], [Bezeichnung]) VALUES (9, 5, N'LK-Q1/2')
INSERT [dbo].[Klassenstufen] ([Id], [JahrgangsstufeId], [Bezeichnung]) VALUES (10, 5, N'LK-Q3/4')
SET IDENTITY_INSERT [dbo].[Klassenstufen] OFF
SET IDENTITY_INSERT [dbo].[Medien] ON 

INSERT [dbo].[Medien] ([Id], [Bezeichnung]) VALUES (1, N'')
INSERT [dbo].[Medien] ([Id], [Bezeichnung]) VALUES (2, N'OH')
INSERT [dbo].[Medien] ([Id], [Bezeichnung]) VALUES (3, N'Tafel')
INSERT [dbo].[Medien] ([Id], [Bezeichnung]) VALUES (4, N'Beamer')
INSERT [dbo].[Medien] ([Id], [Bezeichnung]) VALUES (5, N'Experiment')
INSERT [dbo].[Medien] ([Id], [Bezeichnung]) VALUES (6, N'Hefter')
INSERT [dbo].[Medien] ([Id], [Bezeichnung]) VALUES (7, N'Pause')
INSERT [dbo].[Medien] ([Id], [Bezeichnung]) VALUES (8, N'Buch')
INSERT [dbo].[Medien] ([Id], [Bezeichnung]) VALUES (9, N'Spiel')
INSERT [dbo].[Medien] ([Id], [Bezeichnung]) VALUES (10, N'Computer')
INSERT [dbo].[Medien] ([Id], [Bezeichnung]) VALUES (11, N'Film')
INSERT [dbo].[Medien] ([Id], [Bezeichnung]) VALUES (12, N'Arbeitsbogen')
INSERT [dbo].[Medien] ([Id], [Bezeichnung]) VALUES (13, N'Poster')
SET IDENTITY_INSERT [dbo].[Medien] OFF
SET IDENTITY_INSERT [dbo].[Module] ON 

INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (1, 1, N'NN', 1, N'', 2)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (2, 1, N'P1 - Schwimmen Schweben Sinken', 1, N'Masse/Volumen/Dichte, Druck, Auftrieb, Hyaulik, Archimedesdr', 4)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (3, 1, N'P2 - Vom Inneren Aufbau der Materie', 1, N'Teilchenmodell, Aggregatzustände, Modellbegriff, Temperatur, Adhäsion, Kapillarität, Volumen/Längenänderung', 14)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (4, 1, N'P3 - Wärme im Alltag-Energie ist immer dabei', 1, N'Strahlung/Strömung/Leitung, Anwendungen, Wärmetransport im Teilchenmodell', 8)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (5, 1, N'P4 - Sehen und gesehen werden', 1, N'Prinzip Ameise, Schatten, Finsternisse, Spiegel, Lochkamera, Brechung', 14)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (6, 1, N'P5 - Vom Tragen zur goldenen Regel der Mechanik', 1, N'Kraftbegriff, Kraftwandler, Statik, Verschiedene Formen der Arbeit und Energie, Goldene Regel der Mechanik', 23)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (7, 1, N'P6 - Körper bewegen', 1, N'Bewegungen, Geschwindigkeit, Newton, Gleichförmige lineare Bewegungen - Geschwindigkeit, Beschleunigte Bewegungen (schiefe Ebene), Durchschnitts- und Momentan-Geschwindigkeit, Bewegungen mit unterschiedlichen Einheiten, Weg – Zeit - und Geschwindigkeit -Zeit - Diagramme, Geschwindigkeitsvektor', 10)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (8, 1, N'P7 - Ladungen trennen, Magnete ordnen', 1, N'Magnetismus, magnetische Influenz, magnetische Orientierung bei Tieren, Elektronenmodelle, Ladungen, Leitfähigkeit, Strom, Schaltungen, Ladungen, Kräfte auf Ladungen, Atommodell (Kern – Hülle), Elektrische Leiter, Influenz, Gewitter, Blitzableiter, Faradayscher Käfig, elektrisches Feld, magnetisches Feld', 12)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (9, 1, N'P8 - Wirkungen bewegter Ladungen', 1, N'Einfacher Stromkreis, Strom/Strommessung, Gleich- und Wechselstrom / Gefahren des elektrischen Stroms, Magnetische, chemische und Wärmewirkung, Anwendungen: z. B. Lasthebemagnete, elektrische Klingel, Relais, Lautsprecher, Messinstrumente', 5)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (10, 1, N'NN', 2, N'', 2)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (11, 1, N'P1 - Wege des Stromes-Schaltungssysteme', 2, N'', 0)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (12, 1, N'P2 - Bewegung durch Strom-Strom durch Bewegung', 2, N'', 0)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (13, 1, N'P3 - Besser sehen', 2, N'', 0)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (14, 1, N'P4 - Schneller werden und bremsen', 2, N'', 0)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (15, 1, N'P5 - Struktur der Materie-Energie aus dem Atom', 2, N'', 0)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (16, 1, N'P6 - Von der Quelle zum Empfänger', 2, N'', 0)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (17, 1, N'P7 - Mit Energie versorgen', 2, N'', 0)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (18, 1, N'NN', 3, N'', 2)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (19, 1, N'Elektrisches Feld', 3, N'Coulomb, Gewitter, Feldlinien, Feldstärke, Potenzial, Kondensator, Spannungsbegriff', 10)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (20, 1, N'Gravitation', 3, N'Sonnensystem, Kepler, Gravitationsgesetz, Satelliten', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (21, 1, N'Magnetisches Feld', 3, N'Feldlinien, Zusammenhang EMG-Felder, Spulen, Oersted', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (22, 1, N'I1-Massepunkt', 5, N'Energie- und Impulserhaltung, Kinematik, Dynamik, Kreisbewegung', 15)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (23, 1, N'Induktion', 3, N'Induktionsgesetz, Selbstinduktion, Induktivität, Spule, Wechselspannung', 15)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (24, 1, N'Schwingungen', 3, N'Schwingkreis, Gedämpfte, ungedämpfte Schwingune, Resonanz, Mechanische Schwingungen, Thomson', 10)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (25, 1, N'Wellen', 3, N'Dipol, Reflexion, Beugung, Brechung, Interferenz Polarisation Hertzscher Wellen, elektromagnetisches Spektrum', 8)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (26, 1, N'Ladungsträger in Feldern', 3, N'Gewitter, Millikan, Elementarladung, Lorentzkraft, Wienfilter', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (27, 1, N'Quantenobjekte', 3, N'Lichtelektrischer Effekt, Photonenmodell, De Broglie, Elektronenbeugung, Doppelspaltversuch, Heisenberg, Messprozess', 10)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (28, 1, N'III3-Röntgenstrahlung', 5, N'Entstehung, Eigenschaften, Bragg, Spektren', 15)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (29, 1, N'Atomhülle', 3, N'kontinuierliche und Linienspektren, Emissions- Absorbtionsspektren, Franck-Hertz, Emission, Absorption von Photonen, Termschema, Atommodelle', 20)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (30, 1, N'Atomkern', 3, N'Tröpfchenmodell, Nachweisgeräte, Radioaktivität, Zerfallsgesetz, Aktivität, Kernbindung, Massendefekt, Kernspaltung, Kernfusion', 10)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (31, 1, N'W0 - Experimentieren, protokollieren und auswerten', 1, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (32, 1, N'W1 - Luftschiffe und andere Schiffe', 1, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (33, 1, N'W2 - Heizen und Kochen im Haushalt', 1, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (34, 1, N'W3 - Wetterkunde', 1, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (35, 1, N'W4 - Das Auge - optische Spielereien', 1, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (36, 1, N'W5 - Brücken zur Mechanik', 1, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (37, 1, N'W6 - Bewegungen im Sport', 1, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (38, 1, N'W7 - Rückstoß als Antrieb', 1, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (39, 1, N'W8 - Tragbare Spannungsquellen', 1, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (40, 1, N'W01 - Schaltungen im Haushalt', 2, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (41, 1, N'W02 - Energie aus der Steckdose', 2, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (42, 1, N'W03 - Von der Lupe zum Fernrohr', 2, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (43, 1, N'W04 - Farben sehen, Regenbogen', 2, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (44, 1, N'W05 - Physik im Verkehr', 2, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (45, 1, N'W06 - Im Kreis bewegen', 2, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (46, 1, N'W07 - Heilende und tödliche Kernphysik', 2, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (47, 1, N'W08 - Schwingungen, die man hört', 2, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (48, 1, N'W09 - Astronomie und Weltbilder', 2, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (49, 1, N'W10 - Natur des Lichts', 2, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (50, 1, N'Astronomie', 3, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (51, 1, N'Astrophysik', 3, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (52, 1, N'Drehbewegungen', 3, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (53, 1, N'Elektronik', 3, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (54, 1, N'Elementarteilchenphysik', 3, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (55, 1, N'Festkörperphysik', 3, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (56, 1, N'Geschichte der Physik', 3, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (57, 1, N'Interpretation der Quantenphysik', 3, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (58, 1, N'Kosmologie und Weltbilder', 3, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (59, 1, N'Maxwell-Theorie', 3, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (60, 1, N'Nichtlineare Physik,Chaos', 3, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (61, 1, N'Relativistische Dynamik', 3, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (62, 1, N'Relativistische Kinematik', 3, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (63, 1, N'Strahlenbiophysik', 3, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (64, 1, N'Strahlenschutz', 3, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (65, 1, N'Strömungsphysik', 3, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (66, 1, N'Thermodynamik', 3, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (67, 1, N'Vertiefungen zur Atom und Kernphysik', 3, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (68, 1, N'Wechselstrom', 3, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (69, 1, N'Weiteres', 3, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (70, 1, N'Wellenoptik', 3, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (71, 2, N'NN', 1, N'', 2)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (72, 2, N'P01 - Daten erheben und verstehen', 1, N'Urlisten, Häufigkeiten, Kreis-,Linien-,Balkendiagramme, Mittelwerte, Median, Interpretation', 12)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (73, 2, N'P02 - Verhältnisse mit Proportionalität erfassen', 1, N'Proportionale Zuordnungen, Diagramme, Anwendungen, Prozentrechnung, Zinsrechnung, Dreisatz, Quotientengleichheit', 32)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (74, 2, N'P03 - Negative Zahlen verstehen und verwenden', 1, N'Negative Zahlen, rationale Zahlen, Vorzeichen/Rechenzeichen, Rechengesetze, Terme', 24)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (75, 2, N'P04 - Mit Funktionen Beziehung und Veränderung beschreiben', 1, N'Sachsituationen und Graphen, Wechsel zwischen Tabelle, Graph, Skizze, Koordinatensystem', 20)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (76, 2, N'P05 - Mit Variablen, Termen und Gleichungen Probleme lösen', 1, N'Terme aufstellen, umformen, lösen, Probe, Gleichungen lösen, Rechengesetze, Binomische Formel, Distributivgesetz, Formelumstellungen, Lösungsmenge, Gleichungen als Schnittpunkte von Graphen', 52)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (77, 2, N'P06 - Konstruieren und mit ebenen Figuren argumentieren', 1, N'Punkt, Gerade, Strecke, Winkel, Dreiecke, Klassifikation, Winkelsätze, Haus der Vierecke, Dreieckskonstruktion, Kongruenzsätze, Satz des Thales, Winkelsummensätze, Symmetrie', 32)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (78, 2, N'P07 - Proportionale und antiproportionale Modelle', 1, N'Vergleich, Produktgleichheit, Quotientengleichheit, Zuordnungsvorschrift, Verhältnisgleichungen', 8)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (79, 2, N'P08 - Mit dem Zufall rechnen', 1, N'Grundbegriffe, Häufigkeiten, Schätzen, Laplace-Wahrscheinlichkeiten, Abzählbäume', 12)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (80, 2, N'P09 - Reale Situationen mit linearen Modellen beschreiben', 1, N'Geradengleichung, Steigung, y-Abschnitt, Lineare Gleichungssysteme als Graphen, und rechnerisch, Rekonstruktion von Geraden', 20)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (81, 2, N'P10 - Ebene Figuren und Körper schätzen, messen und berechnen', 1, N'Flächeninhaltsformeln, Kreisumfang und Fläche, Netze, Prismenvolumina, Umfänge, Runden, Zusammengesetzte Flächen, Zerlegungund Ergänzung', 24)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (82, 2, N'W1 - Diskrete Strukturen in der Umwelt', 1, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (83, 2, N'W2 - Körper und Figuren darstellen und berechnen', 1, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (84, 2, N'W3 - Geometrische Abbildungen und Symmetrie', 1, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (85, 2, N'W4 - Geometrisches Begründen und Beweisen', 1, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (86, 2, N'NN', 2, N'', 2)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (87, 2, N'P1 - Neue Zahlen entdecken', 2, N'rationale und irrationale Zahlen, reelle Zahlen, Quadratzahlen und Wurzeln, Pi', 20)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (88, 2, N'P2 - Längen und Flächen bestimmen und berechnen', 2, N'Satzgruppe des Pythagoras, Ähnlichkeit, Strahlensätze', 28)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (89, 2, N'P3 - Aus statistischen Daten Schlüsse ziehen', 2, N'Mittelwert, Modalwert, Median, Spannweite, Säulendiagramm, Abweichung, Boxplot, Fälschen', 8)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (90, 2, N'P4 - Situationen mit quadratischen Funktionen und Potenzfunktionen beschreiben', 2, N'Parabeln, Scheitelpunktsform, quadratische Gleichungen, p/q-Formel, Potenzfunktionen', 16)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (91, 2, N'P5 - Mit Winkeln und Längen rechnen', 2, N'Sinus, Kosinus, Tangens, Symmetrie, Sinussatz, Allgemeine Funktionsgleichung, Einheitskreis, Kosinussatz', 32)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (92, 2, N'P6 - Wachstum und Zerfall mit Funktionen beschreiben', 2, N'Exponentielles, lineares Wachstum, Zerfallsprozesse, Exponentialfunktion, Logarithmusfunktion', 20)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (93, 2, N'P7 - Körper herstellen und berechnen', 2, N'Prisma, Zylinder, Pyramide, Kegel, Kugel, Schrägbilder, Netze, Volumen- und Oberflächenberechnung, Cavalieri, Zusammengesetzte Volumina', 16)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (94, 2, N'P8 - Mit Wahrscheinlichkeiten rechnen', 2, N'2-3 stufige Zufallsexperimente, Pfadregel, Baumdiagramm, Urnenmodelle', 12)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (95, 2, N'P9 - Veränderung mit Funktionen beschreiben', 2, N'Vergleich und Interpretation von Funktionstpyen, mittlere Änderungsrate, lokale Änderungsrate, Extrempunkte grafisch, Ableiten grafisch', 34)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (96, 2, N'W1 - Optimale Wege', 2, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (97, 2, N'W2 - Flächensätze am rechtwinkligen Dreieck', 2, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (98, 2, N'W3 - Kugeln und Kreise', 2, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (99, 2, N'W4 - Beschränktes und logistisches Wachstum', 2, N'', 6)
GO
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (100, 2, N'WP1 - Kreisgeometrie', 1, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (101, 2, N'WP2 - Zählen und Rechnen in historischer Entwicklung', 1, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (102, 2, N'WP3 - Der Goldene Schnitt', 1, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (103, 2, N'WP4 - Lineares Optimieren', 1, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (104, 2, N'WP5 - Kryptologie', 1, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (105, 2, N'WP6 - Platonische Körper', 1, N'', 6)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (106, 2, N'NN', 3, N'', 2)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (107, 2, N'Differentialrechnung', 3, N'', 60)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (108, 2, N'Integralrechnung', 3, N'', 60)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (109, 2, N'Stochastik', 3, N'', 40)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (110, 2, N'Analytische Geometrie', 3, N'', 60)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (111, 5, N'NN', 1, N'', 2)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (112, 1, N'WP01 - Vom Fliegen nicht nur träumen', 2, N'', 0)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (113, 1, N'WP02 - Klänge und Geräusche hören', 2, N'', 0)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (114, 1, N'WP03 - Technik im sozialen Wandel', 2, N'', 0)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (115, 1, N'WP04 - Computer im Einsatz', 2, N'', 0)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (116, 1, N'WP05 - Alternative Energiesysteme nutzen', 2, N'', 0)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (117, 1, N'WP06 - Wetter und Klima', 2, N'', 0)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (118, 1, N'WP07 - Farben wahrnehmen', 2, N'', 0)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (119, 1, N'WP08 - Druck in Natur und Technik', 2, N'', 0)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (120, 1, N'WP09 - Nachrichten übertragen', 2, N'', 0)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (121, 1, N'WP10 - Größen messen und Messfehler betrachten', 2, N'', 0)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (122, 1, N'WP11 - Unseren Himmel beobachten', 2, N'', 0)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (123, 1, N'WP12 - Unser Planetensystem kennen', 2, N'', 0)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (124, 2, N'P00 - Wiederholung', 1, N'Bruchrechnung', 4)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (125, 1, N'IIW-Elektronik', 5, N'Projekt', 10)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (126, 1, N'I3-Elektrisches Feld', 5, N'Feldlinien, Feldstärke, inhomogene Felder, Coulombgesetz, Arbeit-Potenzial-Spannung, Materie im elektrischen Feld, Kondensator', 15)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (127, 1, N'I2-Gravitation', 5, N'Keplergesetze, Gravitationsgesetz, Feldlinienmodell, Gravitationsfeldstärke, Gravitationspotenzial, Körpern im Gravitationsfeld', 15)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (128, 1, N'I4-Magnetisches Feld', 5, N'Feldlinien, Flussdichte, Spule, Oersted, Felder im Vergleich', 15)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (130, 1, N'II1-Induktion', 5, N'Induktionsgesetz, Selbstinduktion, Induktivität, Spule als Energiespeicher, Wechselspannung, Effektivwerte', 20)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (131, 1, N'II2-Schwingungen', 5, N'Schwingkreis, Gedämpfte+ungedämpfte Schwingungen, Resonanz, Mechanische Schwingungen, Thomson', 15)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (132, 1, N'II3-Wellen', 5, N'Dipol, Reflexion+Beugung+Brechung+Interferenz+Polarisation Hertzscher Wellen, Radio, elektromagnetisches Spektrum', 25)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (133, 1, N'III1-Ladungsträger in Feldern', 5, N'Gewitter, Millikan, Elementarladung, Lorentzkraft, Wienfilter, Halleffekt', 20)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (134, 1, N'III2-Quantenobjekte', 5, N'Lichtelektrischer Effekt, Photonenmodell, De Broglie, Elektronenbeugung, Taylor, Compton, Doppelspaltversuch, Heisenberg, Messprozess', 25)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (136, 1, N'IV1-Atomhülle', 5, N'kontinuierliche und Linienspektren, Emissions- Absorptionsspektren, Franck-Hertz, Emission+Absorption von Photonen+Termschema, Atommodelle, Quantenmodell', 15)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (137, 1, N'IV2-Atomkern', 5, N'Tröpfchen+Potentialtopfmodell, Entstehung und Eigenschaften, Nachweisgeräte, Radioaktivität, Zerfallsgesetz,Aktivität+Strahlenschutz, Kernbindung, Massendefekt, Kernspaltung, Kernfusion', 40)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (138, 1, N'IW-NichtlinearePhysik', 5, N'Chaos Grundgeriffe, Rekonstruktion, Beispiele', 15)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (139, 1, N'IIIW-Quanteninterpretation', 5, N'Deutungen, Grenzen', 5)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (140, 1, N'IIIW-Wellenoptik', 5, N'', 10)
INSERT [dbo].[Module] ([Id], [FachId], [Bezeichnung], [JahrgangsstufeId], [Bausteine], [Stundenbedarf]) VALUES (141, 1, N'IVW-Elementarteilchen', 5, N'Teilchenzoo, Higgs', 10)
SET IDENTITY_INSERT [dbo].[Module] OFF
SET IDENTITY_INSERT [dbo].[Monatstypen] ON 

INSERT [dbo].[Monatstypen] ([Id], [Bezeichnung], [MonatIndex]) VALUES (1, N'August', 8)
INSERT [dbo].[Monatstypen] ([Id], [Bezeichnung], [MonatIndex]) VALUES (2, N'September', 9)
INSERT [dbo].[Monatstypen] ([Id], [Bezeichnung], [MonatIndex]) VALUES (3, N'Oktober', 10)
INSERT [dbo].[Monatstypen] ([Id], [Bezeichnung], [MonatIndex]) VALUES (4, N'November', 11)
INSERT [dbo].[Monatstypen] ([Id], [Bezeichnung], [MonatIndex]) VALUES (5, N'Dezember', 12)
INSERT [dbo].[Monatstypen] ([Id], [Bezeichnung], [MonatIndex]) VALUES (6, N'Januar', 1)
INSERT [dbo].[Monatstypen] ([Id], [Bezeichnung], [MonatIndex]) VALUES (7, N'Februar', 2)
INSERT [dbo].[Monatstypen] ([Id], [Bezeichnung], [MonatIndex]) VALUES (8, N'März', 3)
INSERT [dbo].[Monatstypen] ([Id], [Bezeichnung], [MonatIndex]) VALUES (9, N'April', 4)
INSERT [dbo].[Monatstypen] ([Id], [Bezeichnung], [MonatIndex]) VALUES (10, N'Mai', 5)
INSERT [dbo].[Monatstypen] ([Id], [Bezeichnung], [MonatIndex]) VALUES (11, N'Juni', 6)
INSERT [dbo].[Monatstypen] ([Id], [Bezeichnung], [MonatIndex]) VALUES (12, N'Juli', 7)
SET IDENTITY_INSERT [dbo].[Monatstypen] OFF
SET IDENTITY_INSERT [dbo].[NotenWichtungen] ON 

INSERT [dbo].[NotenWichtungen] ([Id], [Bezeichnung], [MündlichQualität], [MündlichQuantität], [MündlichSonstige], [MündlichGesamt], [SchriftlichKlassenarbeit], [SchriftlichSonstige], [SchriftlichGesamt]) VALUES (1, N'SEK I - Standard', 0.4, 0.4, 0.2, 0.5, 0.8, 0.2, 0.5)
INSERT [dbo].[NotenWichtungen] ([Id], [Bezeichnung], [MündlichQualität], [MündlichQuantität], [MündlichSonstige], [MündlichGesamt], [SchriftlichKlassenarbeit], [SchriftlichSonstige], [SchriftlichGesamt]) VALUES (2, N'SEK II - Grundkurs', 0.4, 0.4, 0.2, 0.67, 1, 0, 0.33)
SET IDENTITY_INSERT [dbo].[NotenWichtungen] OFF
SET IDENTITY_INSERT [dbo].[Personen] ON 

INSERT [dbo].[Personen] ([Id], [Vorname], [Nachname], [Geschlecht], [Geburtstag], [Titel], [Telefon], [Fax], [Handy], [EMail], [PLZ], [Straße], [Hausnummer], [Ort], [IstLehrer], [Foto]) VALUES (2, N'Adrian', N'Voßkühler', 0, CAST(0x00006B8F00000000 AS DateTime), N'', N'', N'', N'', N'', N'', N'', N'', N'', 1, NULL)
SET IDENTITY_INSERT [dbo].[Personen] OFF
SET IDENTITY_INSERT [dbo].[Prozentbereiche] ON 

INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (1, 1, 1, 1, 1)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (2, 2, 0.949999988079071, 0.99900001287460327, 1)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (3, 3, 0.89999997615814209, 0.94900000095367432, 1)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (4, 4, 0.85000002384185791, 0.89899998903274536, 1)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (5, 5, 0.800000011920929, 0.84900003671646118, 1)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (6, 6, 0.75, 0.79900002479553223, 1)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (7, 7, 0.699999988079071, 0.74900001287460327, 1)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (8, 8, 0.64999997615814209, 0.69900000095367432, 1)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (9, 9, 0.60000002384185791, 0.64899998903274536, 1)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (10, 10, 0.550000011920929, 0.59900003671646118, 1)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (11, 11, 0.50000005960464478, 0.54900002479553223, 1)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (12, 12, 0.44999998807907104, 0.49900001287460327, 1)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (13, 13, 0.34999999403953552, 0.44900000095367432, 1)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (14, 14, 0.20000000298023224, 0.34900000691413879, 1)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (15, 15, 0.10000000149011612, 0.19900000095367432, 1)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (16, 16, 0, 0.0989999994635582, 1)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (17, 1, 0.949999988079071, 1, 2)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (18, 2, 0.89999997615814209, 0.94900000095367432, 2)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (19, 3, 0.85000002384185791, 0.89899998903274536, 2)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (20, 4, 0.800000011920929, 0.84900003671646118, 2)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (21, 5, 0.75, 0.79900002479553223, 2)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (22, 6, 0.699999988079071, 0.74900001287460327, 2)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (23, 7, 0.64999997615814209, 0.69900000095367432, 2)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (24, 8, 0.60000002384185791, 0.64899998903274536, 2)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (25, 9, 0.550000011920929, 0.59900003671646118, 2)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (26, 10, 0.5, 0.54900002479553223, 2)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (27, 11, 0.44999998807907104, 0.49900001287460327, 2)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (28, 12, 0.36000001430511475, 0.44900000095367432, 2)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (29, 13, 0.27000001072883606, 0.359000027179718, 2)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (30, 14, 0.18000000715255737, 0.26899999380111694, 2)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (31, 15, 0.090000003576278687, 0.17899999022483826, 2)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (32, 16, 0, 0.088999994099140167, 2)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (33, 2, 0.89999997615814209, 1, 3)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (34, 14, 0.20000000298023224, 0.49900001287460327, 3)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (35, 11, 0.5, 0.64899998903274536, 3)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (36, 8, 0.64999997615814209, 0.79900002479553223, 3)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (37, 16, 0, 0.19900000095367432, 3)
INSERT [dbo].[Prozentbereiche] ([Id], [ZensurId], [VonProzent], [BisProzent], [BewertungsschemaId]) VALUES (38, 5, 0.800000011920929, 0.89899998903274536, 3)
SET IDENTITY_INSERT [dbo].[Prozentbereiche] OFF
SET IDENTITY_INSERT [dbo].[Sozialformen] ON 

INSERT [dbo].[Sozialformen] ([Id], [Bezeichnung]) VALUES (1, N'')
INSERT [dbo].[Sozialformen] ([Id], [Bezeichnung]) VALUES (2, N'GA')
INSERT [dbo].[Sozialformen] ([Id], [Bezeichnung]) VALUES (3, N'PA')
INSERT [dbo].[Sozialformen] ([Id], [Bezeichnung]) VALUES (4, N'LV')
INSERT [dbo].[Sozialformen] ([Id], [Bezeichnung]) VALUES (5, N'UG')
INSERT [dbo].[Sozialformen] ([Id], [Bezeichnung]) VALUES (6, N'EA')
SET IDENTITY_INSERT [dbo].[Sozialformen] OFF
SET IDENTITY_INSERT [dbo].[Tendenzen] ON 

INSERT [dbo].[Tendenzen] ([Id], [Bezeichnung], [Wichtung]) VALUES (1, N'++', 1)
INSERT [dbo].[Tendenzen] ([Id], [Bezeichnung], [Wichtung]) VALUES (2, N'+', 2)
INSERT [dbo].[Tendenzen] ([Id], [Bezeichnung], [Wichtung]) VALUES (3, N'0', 3)
INSERT [dbo].[Tendenzen] ([Id], [Bezeichnung], [Wichtung]) VALUES (4, N'-', 4)
INSERT [dbo].[Tendenzen] ([Id], [Bezeichnung], [Wichtung]) VALUES (5, N'--', 5)
SET IDENTITY_INSERT [dbo].[Tendenzen] OFF
SET IDENTITY_INSERT [dbo].[Tendenztypen] ON 

INSERT [dbo].[Tendenztypen] ([Id], [Bezeichnung]) VALUES (1, N'Störung')
INSERT [dbo].[Tendenztypen] ([Id], [Bezeichnung]) VALUES (2, N'Besondere Leistung')
INSERT [dbo].[Tendenztypen] ([Id], [Bezeichnung]) VALUES (3, N'Extraaufgabe')
SET IDENTITY_INSERT [dbo].[Tendenztypen] OFF
SET IDENTITY_INSERT [dbo].[Termintypen] ON 

INSERT [dbo].[Termintypen] ([Id], [Bezeichnung], [Kalenderfarbe]) VALUES (1, N'Klausur', N'Yellow')
INSERT [dbo].[Termintypen] ([Id], [Bezeichnung], [Kalenderfarbe]) VALUES (2, N'Tag der offenen Tür', N'Blue')
INSERT [dbo].[Termintypen] ([Id], [Bezeichnung], [Kalenderfarbe]) VALUES (3, N'Wandertag', N'Magenta')
INSERT [dbo].[Termintypen] ([Id], [Bezeichnung], [Kalenderfarbe]) VALUES (4, N'Abitur', N'Red')
INSERT [dbo].[Termintypen] ([Id], [Bezeichnung], [Kalenderfarbe]) VALUES (5, N'MSA', N'Red')
INSERT [dbo].[Termintypen] ([Id], [Bezeichnung], [Kalenderfarbe]) VALUES (6, N'Unterricht', N'LightBlue')
INSERT [dbo].[Termintypen] ([Id], [Bezeichnung], [Kalenderfarbe]) VALUES (7, N'Vertretung', N'LightGray')
INSERT [dbo].[Termintypen] ([Id], [Bezeichnung], [Kalenderfarbe]) VALUES (8, N'Besprechung', N'Orange')
INSERT [dbo].[Termintypen] ([Id], [Bezeichnung], [Kalenderfarbe]) VALUES (9, N'Sondertermin', N'Orange')
INSERT [dbo].[Termintypen] ([Id], [Bezeichnung], [Kalenderfarbe]) VALUES (10, N'Ferien', N'Green')
INSERT [dbo].[Termintypen] ([Id], [Bezeichnung], [Kalenderfarbe]) VALUES (11, N'Kursfahrt', N'#FFFF00FF')
INSERT [dbo].[Termintypen] ([Id], [Bezeichnung], [Kalenderfarbe]) VALUES (12, N'Klassenfahrt', N'#FFFF00FF')
INSERT [dbo].[Termintypen] ([Id], [Bezeichnung], [Kalenderfarbe]) VALUES (13, N'Projekttag', N'#FFFFA500')
INSERT [dbo].[Termintypen] ([Id], [Bezeichnung], [Kalenderfarbe]) VALUES (14, N'Praktikum', N'#FFFFA500')
INSERT [dbo].[Termintypen] ([Id], [Bezeichnung], [Kalenderfarbe]) VALUES (15, N'Geburtstag', N'#FF63BBFF')
INSERT [dbo].[Termintypen] ([Id], [Bezeichnung], [Kalenderfarbe]) VALUES (16, N'Sportveranstaltung', N'#FFB41FB2')
SET IDENTITY_INSERT [dbo].[Termintypen] OFF
SET IDENTITY_INSERT [dbo].[Unterrichtsstunden] ON 

INSERT [dbo].[Unterrichtsstunden] ([Id], [Bezeichnung], [Beginn], [Ende], [Stundenindex]) VALUES (1, N'1', CAST(0x070040230E430000 AS Time), CAST(0x07008E7657490000 AS Time), 1)
INSERT [dbo].[Unterrichtsstunden] ([Id], [Bezeichnung], [Beginn], [Ende], [Stundenindex]) VALUES (2, N'2', CAST(0x0700EC460A4A0000 AS Time), CAST(0x07003A9A53500000 AS Time), 2)
INSERT [dbo].[Unterrichtsstunden] ([Id], [Bezeichnung], [Beginn], [Ende], [Stundenindex]) VALUES (3, N'3', CAST(0x0700540B6C520000 AS Time), CAST(0x0700A25EB5580000 AS Time), 3)
INSERT [dbo].[Unterrichtsstunden] ([Id], [Bezeichnung], [Beginn], [Ende], [Stundenindex]) VALUES (4, N'4', CAST(0x0700002F68590000 AS Time), CAST(0x07004E82B15F0000 AS Time), 4)
INSERT [dbo].[Unterrichtsstunden] ([Id], [Bezeichnung], [Beginn], [Ende], [Stundenindex]) VALUES (5, N'5', CAST(0x0700C6C37C620000 AS Time), CAST(0x07001417C6680000 AS Time), 5)
INSERT [dbo].[Unterrichtsstunden] ([Id], [Bezeichnung], [Beginn], [Ende], [Stundenindex]) VALUES (6, N'6', CAST(0x070072E778690000 AS Time), CAST(0x0700C03AC26F0000 AS Time), 6)
INSERT [dbo].[Unterrichtsstunden] ([Id], [Bezeichnung], [Beginn], [Ende], [Stundenindex]) VALUES (7, N'7', CAST(0x07007CDB27710000 AS Time), CAST(0x0700CA2E71770000 AS Time), 7)
INSERT [dbo].[Unterrichtsstunden] ([Id], [Bezeichnung], [Beginn], [Ende], [Stundenindex]) VALUES (8, N'8', CAST(0x070086CFD6780000 AS Time), CAST(0x070090C385800000 AS Time), 8)
INSERT [dbo].[Unterrichtsstunden] ([Id], [Bezeichnung], [Beginn], [Ende], [Stundenindex]) VALUES (9, N'9', CAST(0x07004C64EB810000 AS Time), CAST(0x07009AB734880000 AS Time), 9)
INSERT [dbo].[Unterrichtsstunden] ([Id], [Bezeichnung], [Beginn], [Ende], [Stundenindex]) VALUES (10, N'10+', CAST(0x070056589A890000 AS Time), CAST(0x07002058A3A70000 AS Time), 10)
SET IDENTITY_INSERT [dbo].[Unterrichtsstunden] OFF
SET IDENTITY_INSERT [dbo].[Zensuren] ON 

INSERT [dbo].[Zensuren] ([Id], [Notenpunkte], [NoteMitTendenz], [GanzeNote]) VALUES (1, 15, N'1+', 1)
INSERT [dbo].[Zensuren] ([Id], [Notenpunkte], [NoteMitTendenz], [GanzeNote]) VALUES (2, 14, N'1', 1)
INSERT [dbo].[Zensuren] ([Id], [Notenpunkte], [NoteMitTendenz], [GanzeNote]) VALUES (3, 13, N'1-', 1)
INSERT [dbo].[Zensuren] ([Id], [Notenpunkte], [NoteMitTendenz], [GanzeNote]) VALUES (4, 12, N'2+', 2)
INSERT [dbo].[Zensuren] ([Id], [Notenpunkte], [NoteMitTendenz], [GanzeNote]) VALUES (5, 11, N'2', 2)
INSERT [dbo].[Zensuren] ([Id], [Notenpunkte], [NoteMitTendenz], [GanzeNote]) VALUES (6, 10, N'2-', 2)
INSERT [dbo].[Zensuren] ([Id], [Notenpunkte], [NoteMitTendenz], [GanzeNote]) VALUES (7, 9, N'3+', 3)
INSERT [dbo].[Zensuren] ([Id], [Notenpunkte], [NoteMitTendenz], [GanzeNote]) VALUES (8, 8, N'3', 3)
INSERT [dbo].[Zensuren] ([Id], [Notenpunkte], [NoteMitTendenz], [GanzeNote]) VALUES (9, 7, N'3-', 3)
INSERT [dbo].[Zensuren] ([Id], [Notenpunkte], [NoteMitTendenz], [GanzeNote]) VALUES (10, 6, N'4+', 4)
INSERT [dbo].[Zensuren] ([Id], [Notenpunkte], [NoteMitTendenz], [GanzeNote]) VALUES (11, 5, N'4', 4)
INSERT [dbo].[Zensuren] ([Id], [Notenpunkte], [NoteMitTendenz], [GanzeNote]) VALUES (12, 4, N'4-', 4)
INSERT [dbo].[Zensuren] ([Id], [Notenpunkte], [NoteMitTendenz], [GanzeNote]) VALUES (13, 3, N'5+', 5)
INSERT [dbo].[Zensuren] ([Id], [Notenpunkte], [NoteMitTendenz], [GanzeNote]) VALUES (14, 2, N'5', 5)
INSERT [dbo].[Zensuren] ([Id], [Notenpunkte], [NoteMitTendenz], [GanzeNote]) VALUES (15, 1, N'5-', 5)
INSERT [dbo].[Zensuren] ([Id], [Notenpunkte], [NoteMitTendenz], [GanzeNote]) VALUES (16, 0, N'6', 6)
SET IDENTITY_INSERT [dbo].[Zensuren] OFF
/****** Object:  Index [IX_FK_HalbjahrtypArbeit]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_HalbjahrtypArbeit] ON [dbo].[Arbeiten]
(
	[HalbjahrtypId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_JahrtypArbeit]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_JahrtypArbeit] ON [dbo].[Arbeiten]
(
	[JahrtypId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_KlasseArbeit]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_KlasseArbeit] ON [dbo].[Arbeiten]
(
	[KlasseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_ArbeitAufgabe]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_ArbeitAufgabe] ON [dbo].[Aufgaben]
(
	[ArbeitId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_KlasseBetroffeneKlasse]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_KlasseBetroffeneKlasse] ON [dbo].[BetroffeneKlassen]
(
	[KlasseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_TerminBetroffeneKlasse]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_TerminBetroffeneKlasse] ON [dbo].[BetroffeneKlassen]
(
	[TerminId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_FachCurriculum]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_FachCurriculum] ON [dbo].[Curricula]
(
	[FachId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_JahrtypCurriculum]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_JahrtypCurriculum] ON [dbo].[Curricula]
(
	[JahrtypId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_KlassenstufeCurriculum]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_KlassenstufeCurriculum] ON [dbo].[Curricula]
(
	[KlassenstufeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_DateitypDateiverweis]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_DateitypDateiverweis] ON [dbo].[Dateiverweise]
(
	[DateitypId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_StundenentwurfDateiverweis]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_StundenentwurfDateiverweis] ON [dbo].[Dateiverweise]
(
	[StundenentwurfId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_AufgabeErgebnis]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_AufgabeErgebnis] ON [dbo].[Ergebnisse]
(
	[AufgabeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_SchülereintragErgebnis]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_SchülereintragErgebnis] ON [dbo].[Ergebnisse]
(
	[SchülereintragId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_FachFachstundenanzahl]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_FachFachstundenanzahl] ON [dbo].[Fachstundenanzahlen]
(
	[FachId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_KlassenstufeFachstundenanzahl]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_KlassenstufeFachstundenanzahl] ON [dbo].[Fachstundenanzahlen]
(
	[KlassenstufeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_JahrtypFerien]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_JahrtypFerien] ON [dbo].[Ferien]
(
	[JahrtypId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_HalbjahrtypSchulhalbjahr]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_HalbjahrtypSchulhalbjahr] ON [dbo].[Halbjahrespläne]
(
	[HalbjahrtypId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_JahresplanHalbjahresplan]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_JahresplanHalbjahresplan] ON [dbo].[Halbjahrespläne]
(
	[JahresplanId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_FachJahresplan]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_FachJahresplan] ON [dbo].[Jahrespläne]
(
	[FachId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_JahrtypJahresplan]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_JahrtypJahresplan] ON [dbo].[Jahrespläne]
(
	[JahrtypId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_KlasseJahresplan]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_KlasseJahresplan] ON [dbo].[Jahrespläne]
(
	[KlasseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_KlassenstufeKlasse]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_KlassenstufeKlasse] ON [dbo].[Klassen]
(
	[KlassenstufeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_JahrgangsstufeKlassenstufe]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_JahrgangsstufeKlassenstufe] ON [dbo].[Klassenstufen]
(
	[JahrgangsstufeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_FachModul]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_FachModul] ON [dbo].[Module]
(
	[FachId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_JahrgangsstufeModul]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_JahrgangsstufeModul] ON [dbo].[Module]
(
	[JahrgangsstufeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_HalbjahresplanMonatsplan]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_HalbjahresplanMonatsplan] ON [dbo].[Monatspläne]
(
	[HalbjahresplanId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_MonatstypKalendermonat]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_MonatstypKalendermonat] ON [dbo].[Monatspläne]
(
	[Monatstyp_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_ZensurNoten]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_ZensurNoten] ON [dbo].[Noten]
(
	[ZensurId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_MediumPhase]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_MediumPhase] ON [dbo].[Phasen]
(
	[MediumId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_SozialformPhase]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_SozialformPhase] ON [dbo].[Phasen]
(
	[SozialformId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_StundenentwurfPhasen]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_StundenentwurfPhasen] ON [dbo].[Phasen]
(
	[StundenentwurfId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_CurriculumReihe]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_CurriculumReihe] ON [dbo].[Reihen]
(
	[CurriculumId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_ModulReihe]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_ModulReihe] ON [dbo].[Reihen]
(
	[ModulId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_PersonSchülereintrag]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_PersonSchülereintrag] ON [dbo].[Schülereinträge]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_SchülerlisteSchülereintrag]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_SchülerlisteSchülereintrag] ON [dbo].[Schülereinträge]
(
	[SchülerlisteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_FachSchülerliste]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_FachSchülerliste] ON [dbo].[Schülerlisten]
(
	[FachId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_HalbjahrtypSchülerliste]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_HalbjahrtypSchülerliste] ON [dbo].[Schülerlisten]
(
	[HalbjahrtypId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_JahrtypSchülerliste]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_JahrtypSchülerliste] ON [dbo].[Schülerlisten]
(
	[JahrtypId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_KlasseSchülerliste]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_KlasseSchülerliste] ON [dbo].[Schülerlisten]
(
	[KlasseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_SchulwocheSchultag]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_SchulwocheSchultag] ON [dbo].[Schultage]
(
	[SchulwocheId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_TermintypSchultag]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_TermintypSchultag] ON [dbo].[Schultage]
(
	[TermintypId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_JahrtypSchulwoche]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_JahrtypSchulwoche] ON [dbo].[Schulwochen]
(
	[JahrtypId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_ReiheSequenz]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_ReiheSequenz] ON [dbo].[Sequenzen]
(
	[ReiheId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_FachStundenentwurf]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_FachStundenentwurf] ON [dbo].[Stundenentwürfe]
(
	[FachId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_JahrgangsstufeStundenentwurf]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_JahrgangsstufeStundenentwurf] ON [dbo].[Stundenentwürfe]
(
	[JahrgangsstufeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_ModulStundenentwurf]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_ModulStundenentwurf] ON [dbo].[Stundenentwürfe]
(
	[ModulId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_HalbjahrtypStundenplan]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_HalbjahrtypStundenplan] ON [dbo].[Stundenpläne]
(
	[HalbjahrtypId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_JahrtypStundenplan]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_JahrtypStundenplan] ON [dbo].[Stundenpläne]
(
	[JahrtypId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_FachStundenplaneintrag]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_FachStundenplaneintrag] ON [dbo].[Stundenplaneinträge]
(
	[FachId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_KlasseStundenplaneintrag]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_KlasseStundenplaneintrag] ON [dbo].[Stundenplaneinträge]
(
	[KlasseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_StundenplanStundenplaneintrag]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_StundenplanStundenplaneintrag] ON [dbo].[Stundenplaneinträge]
(
	[StundenplanId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_MonatsplanTagesplan]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_MonatsplanTagesplan] ON [dbo].[Tagespläne]
(
	[MonatsplanId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_ErsteUnterrichtsstundeTermin]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_ErsteUnterrichtsstundeTermin] ON [dbo].[Termine]
(
	[ErsteUnterrichtsstundeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_TermintypTermin]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_TermintypTermin] ON [dbo].[Termine]
(
	[TermintypId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_UnterrichtsstundeTermin]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_UnterrichtsstundeTermin] ON [dbo].[Termine]
(
	[LetzteUnterrichtsstundeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_TagesplanLerngruppentermin]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_TagesplanLerngruppentermin] ON [dbo].[Termine_Lerngruppentermin]
(
	[TagesplanId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_JahrtypSchultermin]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_JahrtypSchultermin] ON [dbo].[Termine_Schultermin]
(
	[JahrtypId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_StundenentwurfStunde]    Script Date: 26.07.2013 14:58:53 ******/
CREATE NONCLUSTERED INDEX [IX_FK_StundenentwurfStunde] ON [dbo].[Termine_Stunde]
(
	[StundenentwurfId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Curricula] ADD  CONSTRAINT [DF_Curricula_HalbjahrtypId]  DEFAULT ((1)) FOR [HalbjahrtypId]
GO
ALTER TABLE [dbo].[Phasen] ADD  CONSTRAINT [DF_Phasen_AbfolgeIndex]  DEFAULT ((0)) FOR [AbfolgeIndex]
GO
ALTER TABLE [dbo].[Arbeiten]  WITH CHECK ADD  CONSTRAINT [FK_BewertungsschemaArbeit] FOREIGN KEY([BewertungsschemaId])
REFERENCES [dbo].[Bewertungsschemata] ([Id])
GO
ALTER TABLE [dbo].[Arbeiten] CHECK CONSTRAINT [FK_BewertungsschemaArbeit]
GO
ALTER TABLE [dbo].[Arbeiten]  WITH CHECK ADD  CONSTRAINT [FK_FachArbeit] FOREIGN KEY([FachId])
REFERENCES [dbo].[Fächer] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Arbeiten] CHECK CONSTRAINT [FK_FachArbeit]
GO
ALTER TABLE [dbo].[Arbeiten]  WITH CHECK ADD  CONSTRAINT [FK_HalbjahrtypArbeit] FOREIGN KEY([HalbjahrtypId])
REFERENCES [dbo].[Halbjahrtypen] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Arbeiten] CHECK CONSTRAINT [FK_HalbjahrtypArbeit]
GO
ALTER TABLE [dbo].[Arbeiten]  WITH CHECK ADD  CONSTRAINT [FK_JahrtypArbeit] FOREIGN KEY([JahrtypId])
REFERENCES [dbo].[Jahrtypen] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Arbeiten] CHECK CONSTRAINT [FK_JahrtypArbeit]
GO
ALTER TABLE [dbo].[Arbeiten]  WITH CHECK ADD  CONSTRAINT [FK_KlasseArbeit] FOREIGN KEY([KlasseId])
REFERENCES [dbo].[Klassen] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Arbeiten] CHECK CONSTRAINT [FK_KlasseArbeit]
GO
ALTER TABLE [dbo].[Aufgaben]  WITH CHECK ADD  CONSTRAINT [FK_ArbeitAufgabe] FOREIGN KEY([ArbeitId])
REFERENCES [dbo].[Arbeiten] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Aufgaben] CHECK CONSTRAINT [FK_ArbeitAufgabe]
GO
ALTER TABLE [dbo].[BetroffeneKlassen]  WITH CHECK ADD  CONSTRAINT [FK_KlasseBetroffeneKlasse] FOREIGN KEY([KlasseId])
REFERENCES [dbo].[Klassen] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BetroffeneKlassen] CHECK CONSTRAINT [FK_KlasseBetroffeneKlasse]
GO
ALTER TABLE [dbo].[BetroffeneKlassen]  WITH CHECK ADD  CONSTRAINT [FK_TerminBetroffeneKlasse] FOREIGN KEY([TerminId])
REFERENCES [dbo].[Termine_Schultermin] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BetroffeneKlassen] CHECK CONSTRAINT [FK_TerminBetroffeneKlasse]
GO
ALTER TABLE [dbo].[Curricula]  WITH CHECK ADD  CONSTRAINT [FK_FachCurriculum] FOREIGN KEY([FachId])
REFERENCES [dbo].[Fächer] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Curricula] CHECK CONSTRAINT [FK_FachCurriculum]
GO
ALTER TABLE [dbo].[Curricula]  WITH CHECK ADD  CONSTRAINT [FK_HalbjahrtypCurriculum] FOREIGN KEY([HalbjahrtypId])
REFERENCES [dbo].[Halbjahrtypen] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Curricula] CHECK CONSTRAINT [FK_HalbjahrtypCurriculum]
GO
ALTER TABLE [dbo].[Curricula]  WITH CHECK ADD  CONSTRAINT [FK_JahrtypCurriculum] FOREIGN KEY([JahrtypId])
REFERENCES [dbo].[Jahrtypen] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Curricula] CHECK CONSTRAINT [FK_JahrtypCurriculum]
GO
ALTER TABLE [dbo].[Curricula]  WITH CHECK ADD  CONSTRAINT [FK_KlassenstufeCurriculum] FOREIGN KEY([KlassenstufeId])
REFERENCES [dbo].[Klassenstufen] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Curricula] CHECK CONSTRAINT [FK_KlassenstufeCurriculum]
GO
ALTER TABLE [dbo].[Dateiverweise]  WITH CHECK ADD  CONSTRAINT [FK_DateitypDateiverweis] FOREIGN KEY([DateitypId])
REFERENCES [dbo].[Dateitypen] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Dateiverweise] CHECK CONSTRAINT [FK_DateitypDateiverweis]
GO
ALTER TABLE [dbo].[Dateiverweise]  WITH CHECK ADD  CONSTRAINT [FK_StundenentwurfDateiverweis] FOREIGN KEY([StundenentwurfId])
REFERENCES [dbo].[Stundenentwürfe] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Dateiverweise] CHECK CONSTRAINT [FK_StundenentwurfDateiverweis]
GO
ALTER TABLE [dbo].[Ergebnisse]  WITH CHECK ADD  CONSTRAINT [FK_AufgabeErgebnis] FOREIGN KEY([AufgabeId])
REFERENCES [dbo].[Aufgaben] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Ergebnisse] CHECK CONSTRAINT [FK_AufgabeErgebnis]
GO
ALTER TABLE [dbo].[Ergebnisse]  WITH CHECK ADD  CONSTRAINT [FK_SchülereintragErgebnis] FOREIGN KEY([SchülereintragId])
REFERENCES [dbo].[Schülereinträge] ([Id])
GO
ALTER TABLE [dbo].[Ergebnisse] CHECK CONSTRAINT [FK_SchülereintragErgebnis]
GO
ALTER TABLE [dbo].[Fachstundenanzahlen]  WITH CHECK ADD  CONSTRAINT [FK_FachFachstundenanzahl] FOREIGN KEY([FachId])
REFERENCES [dbo].[Fächer] ([Id])
GO
ALTER TABLE [dbo].[Fachstundenanzahlen] CHECK CONSTRAINT [FK_FachFachstundenanzahl]
GO
ALTER TABLE [dbo].[Fachstundenanzahlen]  WITH CHECK ADD  CONSTRAINT [FK_KlassenstufeFachstundenanzahl] FOREIGN KEY([KlassenstufeId])
REFERENCES [dbo].[Klassenstufen] ([Id])
GO
ALTER TABLE [dbo].[Fachstundenanzahlen] CHECK CONSTRAINT [FK_KlassenstufeFachstundenanzahl]
GO
ALTER TABLE [dbo].[Ferien]  WITH CHECK ADD  CONSTRAINT [FK_JahrtypFerien] FOREIGN KEY([JahrtypId])
REFERENCES [dbo].[Jahrtypen] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Ferien] CHECK CONSTRAINT [FK_JahrtypFerien]
GO
ALTER TABLE [dbo].[Halbjahrespläne]  WITH CHECK ADD  CONSTRAINT [FK_HalbjahrtypSchulhalbjahr] FOREIGN KEY([HalbjahrtypId])
REFERENCES [dbo].[Halbjahrtypen] ([Id])
GO
ALTER TABLE [dbo].[Halbjahrespläne] CHECK CONSTRAINT [FK_HalbjahrtypSchulhalbjahr]
GO
ALTER TABLE [dbo].[Halbjahrespläne]  WITH CHECK ADD  CONSTRAINT [FK_JahresplanHalbjahresplan] FOREIGN KEY([JahresplanId])
REFERENCES [dbo].[Jahrespläne] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Halbjahrespläne] CHECK CONSTRAINT [FK_JahresplanHalbjahresplan]
GO
ALTER TABLE [dbo].[Hausaufgaben]  WITH CHECK ADD  CONSTRAINT [FK_Hausaufgaben_Schülereinträge] FOREIGN KEY([SchülereintragId])
REFERENCES [dbo].[Schülereinträge] ([Id])
GO
ALTER TABLE [dbo].[Hausaufgaben] CHECK CONSTRAINT [FK_Hausaufgaben_Schülereinträge]
GO
ALTER TABLE [dbo].[Jahrespläne]  WITH CHECK ADD  CONSTRAINT [FK_FachJahresplan] FOREIGN KEY([FachId])
REFERENCES [dbo].[Fächer] ([Id])
GO
ALTER TABLE [dbo].[Jahrespläne] CHECK CONSTRAINT [FK_FachJahresplan]
GO
ALTER TABLE [dbo].[Jahrespläne]  WITH CHECK ADD  CONSTRAINT [FK_JahrtypJahresplan] FOREIGN KEY([JahrtypId])
REFERENCES [dbo].[Jahrtypen] ([Id])
GO
ALTER TABLE [dbo].[Jahrespläne] CHECK CONSTRAINT [FK_JahrtypJahresplan]
GO
ALTER TABLE [dbo].[Jahrespläne]  WITH CHECK ADD  CONSTRAINT [FK_KlasseJahresplan] FOREIGN KEY([KlasseId])
REFERENCES [dbo].[Klassen] ([Id])
GO
ALTER TABLE [dbo].[Jahrespläne] CHECK CONSTRAINT [FK_KlasseJahresplan]
GO
ALTER TABLE [dbo].[Klassen]  WITH CHECK ADD  CONSTRAINT [FK_KlassenstufeKlasse] FOREIGN KEY([KlassenstufeId])
REFERENCES [dbo].[Klassenstufen] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Klassen] CHECK CONSTRAINT [FK_KlassenstufeKlasse]
GO
ALTER TABLE [dbo].[Klassenstufen]  WITH CHECK ADD  CONSTRAINT [FK_JahrgangsstufeKlassenstufe] FOREIGN KEY([JahrgangsstufeId])
REFERENCES [dbo].[Jahrgangsstufen] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Klassenstufen] CHECK CONSTRAINT [FK_JahrgangsstufeKlassenstufe]
GO
ALTER TABLE [dbo].[Module]  WITH CHECK ADD  CONSTRAINT [FK_FachModul] FOREIGN KEY([FachId])
REFERENCES [dbo].[Fächer] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Module] CHECK CONSTRAINT [FK_FachModul]
GO
ALTER TABLE [dbo].[Module]  WITH CHECK ADD  CONSTRAINT [FK_JahrgangsstufeModul] FOREIGN KEY([JahrgangsstufeId])
REFERENCES [dbo].[Jahrgangsstufen] ([Id])
GO
ALTER TABLE [dbo].[Module] CHECK CONSTRAINT [FK_JahrgangsstufeModul]
GO
ALTER TABLE [dbo].[Monatspläne]  WITH CHECK ADD  CONSTRAINT [FK_HalbjahresplanMonatsplan] FOREIGN KEY([HalbjahresplanId])
REFERENCES [dbo].[Halbjahrespläne] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Monatspläne] CHECK CONSTRAINT [FK_HalbjahresplanMonatsplan]
GO
ALTER TABLE [dbo].[Monatspläne]  WITH CHECK ADD  CONSTRAINT [FK_MonatstypKalendermonat] FOREIGN KEY([Monatstyp_Id])
REFERENCES [dbo].[Monatstypen] ([Id])
GO
ALTER TABLE [dbo].[Monatspläne] CHECK CONSTRAINT [FK_MonatstypKalendermonat]
GO
ALTER TABLE [dbo].[Noten]  WITH CHECK ADD  CONSTRAINT [FK_Noten_Arbeiten] FOREIGN KEY([ArbeitId])
REFERENCES [dbo].[Arbeiten] ([Id])
GO
ALTER TABLE [dbo].[Noten] CHECK CONSTRAINT [FK_Noten_Arbeiten]
GO
ALTER TABLE [dbo].[Noten]  WITH CHECK ADD  CONSTRAINT [FK_Noten_Schülereinträge] FOREIGN KEY([SchülereintragId])
REFERENCES [dbo].[Schülereinträge] ([Id])
GO
ALTER TABLE [dbo].[Noten] CHECK CONSTRAINT [FK_Noten_Schülereinträge]
GO
ALTER TABLE [dbo].[Noten]  WITH CHECK ADD  CONSTRAINT [FK_ZensurNoten] FOREIGN KEY([ZensurId])
REFERENCES [dbo].[Zensuren] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Noten] CHECK CONSTRAINT [FK_ZensurNoten]
GO
ALTER TABLE [dbo].[Notentendenzen]  WITH CHECK ADD  CONSTRAINT [FK_Notentendenzen_Schülereinträge] FOREIGN KEY([SchülereintragId])
REFERENCES [dbo].[Schülereinträge] ([Id])
GO
ALTER TABLE [dbo].[Notentendenzen] CHECK CONSTRAINT [FK_Notentendenzen_Schülereinträge]
GO
ALTER TABLE [dbo].[Notentendenzen]  WITH CHECK ADD  CONSTRAINT [FK_Notentendenzen_Tendenzen] FOREIGN KEY([TendenzId])
REFERENCES [dbo].[Tendenzen] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Notentendenzen] CHECK CONSTRAINT [FK_Notentendenzen_Tendenzen]
GO
ALTER TABLE [dbo].[Notentendenzen]  WITH CHECK ADD  CONSTRAINT [FK_Notentendenzen_Tendenztypen] FOREIGN KEY([TendenztypId])
REFERENCES [dbo].[Tendenztypen] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Notentendenzen] CHECK CONSTRAINT [FK_Notentendenzen_Tendenztypen]
GO
ALTER TABLE [dbo].[Phasen]  WITH CHECK ADD  CONSTRAINT [FK_MediumPhase] FOREIGN KEY([MediumId])
REFERENCES [dbo].[Medien] ([Id])
GO
ALTER TABLE [dbo].[Phasen] CHECK CONSTRAINT [FK_MediumPhase]
GO
ALTER TABLE [dbo].[Phasen]  WITH CHECK ADD  CONSTRAINT [FK_SozialformPhase] FOREIGN KEY([SozialformId])
REFERENCES [dbo].[Sozialformen] ([Id])
GO
ALTER TABLE [dbo].[Phasen] CHECK CONSTRAINT [FK_SozialformPhase]
GO
ALTER TABLE [dbo].[Phasen]  WITH CHECK ADD  CONSTRAINT [FK_StundenentwurfPhasen] FOREIGN KEY([StundenentwurfId])
REFERENCES [dbo].[Stundenentwürfe] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Phasen] CHECK CONSTRAINT [FK_StundenentwurfPhasen]
GO
ALTER TABLE [dbo].[Prozentbereiche]  WITH CHECK ADD  CONSTRAINT [FK_BewertungsschemaProzentbereich] FOREIGN KEY([BewertungsschemaId])
REFERENCES [dbo].[Bewertungsschemata] ([Id])
GO
ALTER TABLE [dbo].[Prozentbereiche] CHECK CONSTRAINT [FK_BewertungsschemaProzentbereich]
GO
ALTER TABLE [dbo].[Prozentbereiche]  WITH CHECK ADD  CONSTRAINT [FK_ZensurProzentbereich] FOREIGN KEY([ZensurId])
REFERENCES [dbo].[Zensuren] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Prozentbereiche] CHECK CONSTRAINT [FK_ZensurProzentbereich]
GO
ALTER TABLE [dbo].[Reihen]  WITH CHECK ADD  CONSTRAINT [FK_CurriculumReihe] FOREIGN KEY([CurriculumId])
REFERENCES [dbo].[Curricula] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Reihen] CHECK CONSTRAINT [FK_CurriculumReihe]
GO
ALTER TABLE [dbo].[Reihen]  WITH CHECK ADD  CONSTRAINT [FK_ModulReihe] FOREIGN KEY([ModulId])
REFERENCES [dbo].[Module] ([Id])
GO
ALTER TABLE [dbo].[Reihen] CHECK CONSTRAINT [FK_ModulReihe]
GO
ALTER TABLE [dbo].[Schülereinträge]  WITH CHECK ADD  CONSTRAINT [FK_PersonSchülereintrag] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Personen] ([Id])
GO
ALTER TABLE [dbo].[Schülereinträge] CHECK CONSTRAINT [FK_PersonSchülereintrag]
GO
ALTER TABLE [dbo].[Schülereinträge]  WITH CHECK ADD  CONSTRAINT [FK_SchülerlisteSchülereintrag] FOREIGN KEY([SchülerlisteId])
REFERENCES [dbo].[Schülerlisten] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Schülereinträge] CHECK CONSTRAINT [FK_SchülerlisteSchülereintrag]
GO
ALTER TABLE [dbo].[Schülerlisten]  WITH CHECK ADD  CONSTRAINT [FK_FachSchülerlisten] FOREIGN KEY([FachId])
REFERENCES [dbo].[Fächer] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Schülerlisten] CHECK CONSTRAINT [FK_FachSchülerlisten]
GO
ALTER TABLE [dbo].[Schülerlisten]  WITH CHECK ADD  CONSTRAINT [FK_HalbjahrtypSchülerliste] FOREIGN KEY([HalbjahrtypId])
REFERENCES [dbo].[Halbjahrtypen] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Schülerlisten] CHECK CONSTRAINT [FK_HalbjahrtypSchülerliste]
GO
ALTER TABLE [dbo].[Schülerlisten]  WITH CHECK ADD  CONSTRAINT [FK_JahrtypSchülerlisten] FOREIGN KEY([JahrtypId])
REFERENCES [dbo].[Jahrtypen] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Schülerlisten] CHECK CONSTRAINT [FK_JahrtypSchülerlisten]
GO
ALTER TABLE [dbo].[Schülerlisten]  WITH CHECK ADD  CONSTRAINT [FK_KlasseSchülerliste] FOREIGN KEY([KlasseId])
REFERENCES [dbo].[Klassen] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Schülerlisten] CHECK CONSTRAINT [FK_KlasseSchülerliste]
GO
ALTER TABLE [dbo].[Schülerlisten]  WITH CHECK ADD  CONSTRAINT [FK_NotenWichtungenSchülerliste] FOREIGN KEY([NotenWichtungId])
REFERENCES [dbo].[NotenWichtungen] ([Id])
GO
ALTER TABLE [dbo].[Schülerlisten] CHECK CONSTRAINT [FK_NotenWichtungenSchülerliste]
GO
ALTER TABLE [dbo].[Schultage]  WITH CHECK ADD  CONSTRAINT [FK_SchulwocheSchultag] FOREIGN KEY([SchulwocheId])
REFERENCES [dbo].[Schulwochen] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Schultage] CHECK CONSTRAINT [FK_SchulwocheSchultag]
GO
ALTER TABLE [dbo].[Schultage]  WITH CHECK ADD  CONSTRAINT [FK_TermintypSchultag] FOREIGN KEY([TermintypId])
REFERENCES [dbo].[Termintypen] ([Id])
GO
ALTER TABLE [dbo].[Schultage] CHECK CONSTRAINT [FK_TermintypSchultag]
GO
ALTER TABLE [dbo].[Schulwochen]  WITH CHECK ADD  CONSTRAINT [FK_JahrtypSchulwoche] FOREIGN KEY([JahrtypId])
REFERENCES [dbo].[Jahrtypen] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Schulwochen] CHECK CONSTRAINT [FK_JahrtypSchulwoche]
GO
ALTER TABLE [dbo].[Sequenzen]  WITH CHECK ADD  CONSTRAINT [FK_ReiheSequenz] FOREIGN KEY([ReiheId])
REFERENCES [dbo].[Reihen] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Sequenzen] CHECK CONSTRAINT [FK_ReiheSequenz]
GO
ALTER TABLE [dbo].[Stundenentwürfe]  WITH CHECK ADD  CONSTRAINT [FK_FachStundenentwurf] FOREIGN KEY([FachId])
REFERENCES [dbo].[Fächer] ([Id])
GO
ALTER TABLE [dbo].[Stundenentwürfe] CHECK CONSTRAINT [FK_FachStundenentwurf]
GO
ALTER TABLE [dbo].[Stundenentwürfe]  WITH CHECK ADD  CONSTRAINT [FK_JahrgangsstufeStundenentwurf] FOREIGN KEY([JahrgangsstufeId])
REFERENCES [dbo].[Jahrgangsstufen] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Stundenentwürfe] CHECK CONSTRAINT [FK_JahrgangsstufeStundenentwurf]
GO
ALTER TABLE [dbo].[Stundenentwürfe]  WITH CHECK ADD  CONSTRAINT [FK_ModulStundenentwurf] FOREIGN KEY([ModulId])
REFERENCES [dbo].[Module] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Stundenentwürfe] CHECK CONSTRAINT [FK_ModulStundenentwurf]
GO
ALTER TABLE [dbo].[Stundenpläne]  WITH CHECK ADD  CONSTRAINT [FK_HalbjahrtypStundenplan] FOREIGN KEY([HalbjahrtypId])
REFERENCES [dbo].[Halbjahrtypen] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Stundenpläne] CHECK CONSTRAINT [FK_HalbjahrtypStundenplan]
GO
ALTER TABLE [dbo].[Stundenpläne]  WITH CHECK ADD  CONSTRAINT [FK_JahrtypStundenplan] FOREIGN KEY([JahrtypId])
REFERENCES [dbo].[Jahrtypen] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Stundenpläne] CHECK CONSTRAINT [FK_JahrtypStundenplan]
GO
ALTER TABLE [dbo].[Stundenplaneinträge]  WITH CHECK ADD  CONSTRAINT [FK_FachStundenplaneintrag] FOREIGN KEY([FachId])
REFERENCES [dbo].[Fächer] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Stundenplaneinträge] CHECK CONSTRAINT [FK_FachStundenplaneintrag]
GO
ALTER TABLE [dbo].[Stundenplaneinträge]  WITH CHECK ADD  CONSTRAINT [FK_KlasseStundenplaneintrag] FOREIGN KEY([KlasseId])
REFERENCES [dbo].[Klassen] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Stundenplaneinträge] CHECK CONSTRAINT [FK_KlasseStundenplaneintrag]
GO
ALTER TABLE [dbo].[Stundenplaneinträge]  WITH CHECK ADD  CONSTRAINT [FK_StundenplanStundenplaneintrag] FOREIGN KEY([StundenplanId])
REFERENCES [dbo].[Stundenpläne] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Stundenplaneinträge] CHECK CONSTRAINT [FK_StundenplanStundenplaneintrag]
GO
ALTER TABLE [dbo].[Tagespläne]  WITH CHECK ADD  CONSTRAINT [FK_MonatsplanTagesplan] FOREIGN KEY([MonatsplanId])
REFERENCES [dbo].[Monatspläne] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Tagespläne] CHECK CONSTRAINT [FK_MonatsplanTagesplan]
GO
ALTER TABLE [dbo].[Termine]  WITH CHECK ADD  CONSTRAINT [FK_ErsteUnterrichtsstundeTermin] FOREIGN KEY([ErsteUnterrichtsstundeId])
REFERENCES [dbo].[Unterrichtsstunden] ([Id])
GO
ALTER TABLE [dbo].[Termine] CHECK CONSTRAINT [FK_ErsteUnterrichtsstundeTermin]
GO
ALTER TABLE [dbo].[Termine]  WITH CHECK ADD  CONSTRAINT [FK_TermintypTermin] FOREIGN KEY([TermintypId])
REFERENCES [dbo].[Termintypen] ([Id])
GO
ALTER TABLE [dbo].[Termine] CHECK CONSTRAINT [FK_TermintypTermin]
GO
ALTER TABLE [dbo].[Termine]  WITH CHECK ADD  CONSTRAINT [FK_UnterrichtsstundeTermin] FOREIGN KEY([LetzteUnterrichtsstundeId])
REFERENCES [dbo].[Unterrichtsstunden] ([Id])
GO
ALTER TABLE [dbo].[Termine] CHECK CONSTRAINT [FK_UnterrichtsstundeTermin]
GO
ALTER TABLE [dbo].[Termine_Lerngruppentermin]  WITH CHECK ADD  CONSTRAINT [FK_Lerngruppentermin_inherits_Termin] FOREIGN KEY([Id])
REFERENCES [dbo].[Termine] ([Id])
GO
ALTER TABLE [dbo].[Termine_Lerngruppentermin] CHECK CONSTRAINT [FK_Lerngruppentermin_inherits_Termin]
GO
ALTER TABLE [dbo].[Termine_Lerngruppentermin]  WITH CHECK ADD  CONSTRAINT [FK_TagesplanLerngruppentermin] FOREIGN KEY([TagesplanId])
REFERENCES [dbo].[Tagespläne] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Termine_Lerngruppentermin] CHECK CONSTRAINT [FK_TagesplanLerngruppentermin]
GO
ALTER TABLE [dbo].[Termine_Schultermin]  WITH CHECK ADD  CONSTRAINT [FK_JahrtypSchultermin] FOREIGN KEY([JahrtypId])
REFERENCES [dbo].[Jahrtypen] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Termine_Schultermin] CHECK CONSTRAINT [FK_JahrtypSchultermin]
GO
ALTER TABLE [dbo].[Termine_Schultermin]  WITH CHECK ADD  CONSTRAINT [FK_Schultermin_inherits_Termin] FOREIGN KEY([Id])
REFERENCES [dbo].[Termine] ([Id])
GO
ALTER TABLE [dbo].[Termine_Schultermin] CHECK CONSTRAINT [FK_Schultermin_inherits_Termin]
GO
ALTER TABLE [dbo].[Termine_Stunde]  WITH CHECK ADD  CONSTRAINT [FK_Stunde_inherits_Lerngruppentermin] FOREIGN KEY([Id])
REFERENCES [dbo].[Termine_Lerngruppentermin] ([Id])
GO
ALTER TABLE [dbo].[Termine_Stunde] CHECK CONSTRAINT [FK_Stunde_inherits_Lerngruppentermin]
GO
ALTER TABLE [dbo].[Termine_Stunde]  WITH CHECK ADD  CONSTRAINT [FK_StundenentwurfStunde] FOREIGN KEY([StundenentwurfId])
REFERENCES [dbo].[Stundenentwürfe] ([Id])
GO
ALTER TABLE [dbo].[Termine_Stunde] CHECK CONSTRAINT [FK_StundenentwurfStunde]
GO
USE [master]
GO
ALTER DATABASE [Liduv] SET  READ_WRITE 
GO
