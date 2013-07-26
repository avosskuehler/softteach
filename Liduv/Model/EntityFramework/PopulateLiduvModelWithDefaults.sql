-- Populating default values for nearly all tables
-- Some for testing purposes.
-- --------------------------------------------------
  SET IDENTITY_INSERT [Jahrgangsstufen] ON
  GO
  INSERT INTO [Jahrgangsstufen] (Id, Bezeichnung)
  VALUES (1, '7/8');
  INSERT INTO [Jahrgangsstufen] (Id, Bezeichnung)
  VALUES (2, '9/10');
  INSERT INTO [Jahrgangsstufen] (Id, Bezeichnung)
  VALUES (3, 'Oberstufe');
  GO
  SET IDENTITY_INSERT [Jahrgangsstufen] OFF
  GO
  
  SET IDENTITY_INSERT [Klassenstufen] ON
  GO
  INSERT INTO [Klassenstufen] (Id, JahrgangsstufeId, Bezeichnung)
  VALUES (1, 1 ,'7');
  INSERT INTO [Klassenstufen] (Id, JahrgangsstufeId, Bezeichnung)
  VALUES (2, 1 ,'8');
  INSERT INTO [Klassenstufen] (Id, JahrgangsstufeId, Bezeichnung)
  VALUES (3, 2 ,'9');
  INSERT INTO [Klassenstufen] (Id, JahrgangsstufeId, Bezeichnung)
  VALUES (4, 2 ,'10');
  INSERT INTO [Klassenstufen] (Id, JahrgangsstufeId, Bezeichnung)
  VALUES (5, 3 ,'Q1/2');
  INSERT INTO [Klassenstufen] (Id, JahrgangsstufeId, Bezeichnung)
  VALUES (6, 3 ,'Q3/4');
  GO
  SET IDENTITY_INSERT [Klassenstufen] OFF
  GO
  
  SET IDENTITY_INSERT [Fächer] ON
  GO
  INSERT INTO [Fächer] (Id, Bezeichnung, Farbe)
  VALUES (1, 'Physik','Yellow');
  INSERT INTO [Fächer] (Id, Bezeichnung, Farbe)
  VALUES (2, 'Mathematik','Blue');
  INSERT INTO [Fächer] (Id, Bezeichnung, Farbe)
  VALUES (3, 'Biologie','Green');
  INSERT INTO [Fächer] (Id, Bezeichnung, Farbe)
  VALUES (4, 'Musik','Red');
  INSERT INTO [Fächer] (Id, Bezeichnung, Farbe)
  VALUES (5, 'Vertretungsstunden','Gray');  
  GO
  SET IDENTITY_INSERT [Fächer] OFF
  GO

  SET IDENTITY_INSERT [Fachstundenanzahlen] ON
  GO
  INSERT INTO [Fachstundenanzahlen] (Id, FachId, KlassenstufeId, Stundenzahl, Teilungsstundenzahl)
  VALUES (1, 1, 1, 1, 1);
  INSERT INTO [Fachstundenanzahlen] (Id, FachId, KlassenstufeId, Stundenzahl, Teilungsstundenzahl)
  VALUES (2, 1, 2, 2);
  INSERT INTO [Fachstundenanzahlen] (Id, FachId, KlassenstufeId, Stundenzahl, Teilungsstundenzahl)
  VALUES (3, 1, 3, 2);
  INSERT INTO [Fachstundenanzahlen] (Id, FachId, KlassenstufeId, Stundenzahl, Teilungsstundenzahl)
  VALUES (4, 1, 4, 2);
  INSERT INTO [Fachstundenanzahlen] (Id, FachId, KlassenstufeId, Stundenzahl, Teilungsstundenzahl)
  VALUES (5, 1, 5, 3);
  INSERT INTO [Fachstundenanzahlen] (Id, FachId, KlassenstufeId, Stundenzahl, Teilungsstundenzahl)
  VALUES (6, 1, 6, 3);
  INSERT INTO [Fachstundenanzahlen] (Id, FachId, KlassenstufeId, Stundenzahl, Teilungsstundenzahl)
  VALUES (7, 2, 1, 4);
  INSERT INTO [Fachstundenanzahlen] (Id, FachId, KlassenstufeId, Stundenzahl, Teilungsstundenzahl)
  VALUES (8, 2, 2, 4);
  INSERT INTO [Fachstundenanzahlen] (Id, FachId, KlassenstufeId, Stundenzahl, Teilungsstundenzahl)
  VALUES (9, 2, 3, 4);
  INSERT INTO [Fachstundenanzahlen] (Id, FachId, KlassenstufeId, Stundenzahl, Teilungsstundenzahl)
  VALUES (10, 2, 4, 4);
  INSERT INTO [Fachstundenanzahlen] (Id, FachId, KlassenstufeId, Stundenzahl, Teilungsstundenzahl)
  VALUES (11, 2, 5, 4);
  INSERT INTO [Fachstundenanzahlen] (Id, FachId, KlassenstufeId, Stundenzahl, Teilungsstundenzahl)
  VALUES (12, 2, 6, 4);
  GO
  SET IDENTITY_INSERT [Fachstundenanzahlen] OFF
  GO

  SET IDENTITY_INSERT [Dateitypen] ON
  GO
  INSERT INTO [Dateitypen] (Id, Bezeichnung, Kürzel)
  VALUES (1, '','');
  INSERT INTO [Dateitypen] (Id, Bezeichnung, Kürzel)
  VALUES (2, 'Folie','OH');
  INSERT INTO [Dateitypen] (Id, Bezeichnung, Kürzel)
  VALUES (3, 'Arbeitsblatt','AB');
  INSERT INTO [Dateitypen] (Id, Bezeichnung, Kürzel)
  VALUES (4, 'Klausur','KL');
  INSERT INTO [Dateitypen] (Id, Bezeichnung, Kürzel)
  VALUES (5, 'Klassenarbeit','KA');
  INSERT INTO [Dateitypen] (Id, Bezeichnung, Kürzel)
  VALUES (6, 'Test','TE');
  INSERT INTO [Dateitypen] (Id, Bezeichnung, Kürzel)
  VALUES (7, 'Tägliche Übungen','TÜ');
  INSERT INTO [Dateitypen] (Id, Bezeichnung, Kürzel)
  VALUES (8, 'Tandembogen','TA');  
  INSERT INTO [Dateitypen] (Id, Bezeichnung, Kürzel)
  VALUES (9, 'Projekt','PR');  
  INSERT INTO [Dateitypen] (Id, Bezeichnung, Kürzel)
  VALUES (10, 'Präsentation','PP');  
  INSERT INTO [Dateitypen] (Id, Bezeichnung, Kürzel)
  VALUES (11, 'Wochenplan','WP');    
  INSERT INTO [Dateitypen] (Id, Bezeichnung, Kürzel)
  VALUES (12, 'Hausaufgabe','HA');    
  INSERT INTO [Dateitypen] (Id, Bezeichnung, Kürzel)
  VALUES (13, 'Unterrichtsentwurf','UE');    
  INSERT INTO [Dateitypen] (Id, Bezeichnung, Kürzel)
  VALUES (14, 'Spiel','SP');   
  GO
  SET IDENTITY_INSERT [Dateitypen] OFF
  GO

  SET IDENTITY_INSERT [Module] ON
  GO
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (1, 1, 1, 'NN','',2);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (2, 1, 1, 'P1 - Schwimmen Schweben Sinken','Druck, Auftrieb, Hydraulik, Archimedes', 6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (3, 1, 1, 'P2 - Vom Inneren Aufbau der Materie','Teilchemodell, Aggregatzustände, Modellbegriff, Masse/Volumen/Dichte, Temperatur, Adhäsion, Kapillarität, Volumen/Längenänderung', 16);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (4, 1, 1, 'P3 - Wärme im Alltag-Energie ist immer dabei','Strahlung/Strömung/Leitung, Anwendungen', 12);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (5, 1, 1, 'P4 - Sehen und gesehen werden','Prinzip Ameise, Schatten, Finsternisse, Spiegel, Lochkamera, Brechung', 14);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (6, 1, 1, 'P5 - Vom Tragen zur goldenen Regel der Mechanik','Kraftbegriff, Kraftwandler, Statik',14);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (7, 1, 1, 'P6 - Körper bewegen','Bewegungen, Geschwindigkeit, Newton', 12);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (8, 1, 1, 'P7 - Ladungen trennen, Magnete ordnen', 'Magnetismus, Elektronenodelle, Ladungen, Leitfähigkeit, Strom, Schaltungen', 18);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (9, 1, 1, 'P8 - Wirkungen bewegter Ladungen','Licht/Wärmewirkung, Elektromagnetismus, Widerstand, Gefahren',8);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (10, 1, 2, 'NN','',2);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (11, 1, 2, 'P1 - Wege des Stromes-Schaltungssysteme','',0);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (12, 1, 2, 'P2 - Bewegung durch Strom-Strom durch Bewegung','',0);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (13, 1, 2, 'P3 - Besser sehen','',0);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (14, 1, 2, 'P4 - Schneller werden und bremsen','',0);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (15, 1, 2, 'P5 - Struktur der Materie-Energie aus dem Atom','',0);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (16, 1, 2, 'P6 - Von der Quelle zum Empfänger','',0);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (17, 1, 2, 'P7 - Mit Energie versorgen','',0);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (18, 1, 3, 'NN','',2);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (19, 1, 3, 'Elektrisches Feld','Coulomb, Gewitter, Feldlinien, Feldstärke, Potenzial, Kondensator, Spannungsbegriff', 10);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (20, 1, 3, 'Gravitation', 'Sonnensystem, Kepler, Gravitationsgesetz, Satelliten', 6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (21, 1, 3, 'Magnetisches Feld','Feldlinien, Zusammenhang EMG-Felder, Spulen, Oersted',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (22, 1, 3, 'Massepunkt LK','Energie- und Impulserhaltung, Kinematik, Dynamik, Kreisbewegung', 10);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (23, 1, 3, 'Induktion','Induktionsgesetz, Selbstinduktion, Induktivität, Spule, Wechselspannung',15);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (24, 1, 3, 'Schwingungen','Schwingkreis, Gedämpfte, ungedämpfte Schwingune, Resonanz, Mechanische Schwingungen, Thomson', 10);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (25, 1, 3, 'Wellen','Dipol, Reflexion, Beugung, Brechung, Interferenz Polarisation Hertzscher Wellen, elektromagnetisches Spektrum', 8);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (26, 1, 3, 'Ladungsträger in Feldern','Gewitter, Millikan, Elementarladung, Lorentzkraft, Wienfilter',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (27, 1, 3, 'Quantenobjekte','Lichtelektrischer Effekt, Photonenmodell, De Broglie, Elektronenbeugung, Doppelspaltversuch, Heisenberg, Messprozess', 10);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (28, 1, 3, 'Röntgenstrahlung LK','Entstehung, Eigenschaften, Bragg, Spektren', 10);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (29, 1, 3, 'Atomhülle', 'kontinuierliche und Linienspektren, Emissions- Absorbtionsspektren, Franck-Hertz, Emission, Absorption von Photonen, Termschema, Atommodelle', 20);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (30, 1, 3, 'Atomkern','Tröpfchenmodell, Nachweisgeräte, Radioaktivität, Zerfallsgesetz, Aktivität, Kernbindung, Massendefekt, Kernspaltung, Kernfusion', 10);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (31, 1, 1, 'W0 - Experimentieren, protokollieren und auswerten','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (32, 1, 1, 'W1 - Luftschiffe und andere Schiffe','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (33, 1, 1, 'W2 - Heizen und Kochen im Haushalt','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (34, 1, 1, 'W3 - Wetterkunde','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (35, 1, 1, 'W4 - Das Auge - optische Spielereien','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (36, 1, 1, 'W5 - Brücken zur Mechanik','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (37, 1, 1, 'W6 - Bewegungen im Sport','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (38, 1, 1, 'W7 - Rückstoß als Antrieb','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (39, 1, 1, 'W8 - Tragbare Spannungsquellen','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (40, 1, 2, 'W01 - Schaltungen im Haushalt','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (41, 1, 2, 'W02 - Energie aus der Steckdose','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (42, 1, 2, 'W03 - Von der Lupe zum Fernrohr','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (43, 1, 2, 'W04 - Farben sehen, Regenbogen','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (44, 1, 2, 'W05 - Physik im Verkehr','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (45, 1, 2, 'W06 - Im Kreis bewegen','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (46, 1, 2, 'W07 - Heilende und tödliche Kernphysik','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (47, 1, 2, 'W08 - Schwingungen, die man hört','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (48, 1, 2, 'W09 - Astronomie und Weltbilder','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (49, 1, 2, 'W10 - Natur des Lichts','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (50, 1, 3, 'Astronomie','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (51, 1, 3, 'Astrophysik','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (52, 1, 3, 'Drehbewegungen','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (53, 1, 3, 'Elektronik','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (54, 1, 3, 'Elementarteilchenphysik','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (55, 1, 3, 'Festkörperphysik','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (56, 1, 3, 'Geschichte der Physik','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (57, 1, 3, 'Interpretation der Quantenphysik','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (58, 1, 3, 'Kosmologie und Weltbilder','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (59, 1, 3, 'Maxwell-Theorie','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (60, 1, 3, 'Nichtlineare Physik,Chaos','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (61, 1, 3, 'Relativistische Dynamik','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (62, 1, 3, 'Relativistische Kinematik','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (63, 1, 3, 'Strahlenbiophysik','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (64, 1, 3, 'Strahlenschutz','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (65, 1, 3, 'Strömungsphysik','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (66, 1, 3, 'Thermodynamik','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (67, 1, 3, 'Vertiefungen zur Atom und Kernphysik','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (68, 1, 3, 'Wechselstrom','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (69, 1, 3, 'Weiteres','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (70, 1, 3, 'Wellenoptik','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (71, 2, 1, 'NN','',2);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (72, 2, 1, 'P01 - Daten erheben und verstehen','Urlisten, Häufigkeiten, Kreis-,Linien-,Balkendiagramme, Mittelwerte, Median, Interpretation', 12);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (73, 2, 1, 'P02 - Verhältnisse mit Proportionalität erfassen','Proportionale Zuordnungen, Diagramme, Anwendungen, Prozentrechnung, Zinsrechnung, Dreisatz, Quotientengleichheit',16);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (74, 2, 1, 'P03 - Negative Zahlen verstehen und verwenden','Negative Zahlen, rationale Zahlen, Vorzeichen/Rechenzeichen, Rechengesetze, Terme', 12);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (75, 2, 1, 'P04 - Mit Funktionen Beziehung und Veränderung beschreiben','Sachsituationen und Graphen, Wechsel zwischen Tabelle, Graph, Skizze, Koordinatensystem',8);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (76, 2, 1, 'P05 - Mit Variablen, Termen und Gleichungen Probleme lösen', 'Terme aufstellen, umformen, lösen, Probe, Gleichungen lösen, Rechengesetze, Binomische Formel, Distributivgesetz, Formelumstellungen, Lösungsmenge, Gleichungen als Schnittpunkte von Graphen',20);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (77, 2, 1, 'P06 - Konstruieren und mit ebenen Figuren argumentieren','Punkt, Gerade, Strecke, Winkel, Dreiecke, Klassifikation, Winkelsätze, Haus der Vierecke, Dreieckskonstruktion, Kongruenzsätze, Satz des Thales, Winkelsummensätze, Symmetrie',20);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (78, 2, 1, 'P07 - Proportionale und antiproportionale Modelle','Vergleich, Produktgleichheit, Quotientengleichheit, Zuordnungsvorschrift, Verhältnisgleichungen',12);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (79, 2, 1, 'P08 - Mit dem Zufall rechnen','Grundbegriffe, Häufigkeiten, Schätzen, Laplace-Wahrscheinlichkeiten, Abzählbäume',12,0);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (80, 2, 1, 'P09 - Reale Situationen mit linearen Modellen beschreiben','Geradengleichung, Steigung, y-Abschnitt, Lineare Gleichungssysteme als Graphen, und rechnerisch, Rekonstruktion von Geraden',16);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (81, 2, 1, 'P10 - Ebene Figuren und Körper schätzen, messen und berechnen','Flächeninhaltsformeln, Kreisumfang und Fläche, Netze, Prismenvolumina, Umfänge, Runden, Zusammengesetzte Flächen, Zerlegungund Ergänzung',16);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (82, 2, 1, 'W1 - Diskrete Strukturen in der Umwelt','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (83, 2, 1, 'W2 - Körper und Figuren darstellen und berechnen','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (84, 2, 1, 'W3 - Geometrische Abbildungen und Symmetrie','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (85, 2, 1, 'W4 - Geometrisches Begründen und Beweisen','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (86, 2, 2, 'NN','',2);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (87, 2, 2, 'P1 - Neue Zahlen entdecken','rationale und irrationale Zahlen, reelle Zahlen, Quadratzahlen und Wurzeln, Pi', 20);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (88, 2, 2, 'P2 - Längen und Flächen bestimmen und berechnen','Satzgruppe des Pythagoras, Ähnlichkeit, Strahlensätze',28);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (89, 2, 2, 'P3 - Aus statistischen Daten Schlüsse ziehen','Mittelwert, Modalwert, Median, Spannweite, Säulendiagramm, Abweichung, Boxplot, Fälschen',8);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (90, 2, 2, 'P4 - Situationen mit quadratischen Funktionen und Potenzfunktionen beschreiben','Parabeln, Scheitelpunktsform, quadratische Gleichungen, p/q-Formel, Potenzfunktionen',16);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (91, 2, 2, 'P5 - Mit Winkeln und Längen rechnen','Sinus, Kosinus, Tangens, Symmetrie, Sinussatz, Allgemeine Funktionsgleichung, Einheitskreis, Kosinussatz',32);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (92, 2, 2, 'P6 - Wachstum und Zerfall mit Funktionen beschreiben','Exponentielles, lineares Wachstum, Zerfallsprozesse, Exponentialfunktion, Logarithmusfunktion',20);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (93, 2, 2, 'P7 - Körper herstellen und berechnen','Prisma, Zylinder, Pyramide, Kegel, Kugel, Schrägbilder, Netze, Volumen- und Oberflächenberechnung, Cavalieri, Zusammengesetzte Volumina', 16);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (94, 2, 2, 'P8 - Mit Wahrscheinlichkeiten rechnen','2-3 stufige Zufallsexperimente, Pfadregel, Baumdiagramm, Urnenmodelle', 12);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (95, 2, 2, 'P9 - Veränderung mit Funktionen beschreiben', 'Vergleich und Interpretation von Funktionstpyen, mittlere Änderungsrate, lokale Änderungsrate, Extrempunkte grafisch, Ableiten grafisch',34);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (96, 2, 2, 'W1 - Optimale Wege','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (97, 2, 2, 'W2 - Flächensätze am rechtwinkligen Dreieck','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (98, 2, 2, 'W3 - Kugeln und Kreise','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (99, 2, 2, 'W4 - Beschränktes und logistisches Wachstum','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (100, 2, 1, 'WP1 - Kreisgeometrie','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (101, 2, 1, 'WP2 - Zählen und Rechnen in historischer Entwicklung','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (102, 2, 1, 'WP3 - Der Goldene Schnitt','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (103, 2, 1, 'WP4 - Lineares Optimieren','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (104, 2, 1, 'WP5 - Kryptologie','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (105, 2, 1, 'WP6 - Platonische Körper','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (106, 2, 3, 'Keine Modulzuordnung','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (107, 2, 3, 'Differentialrechnung','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (108, 2, 3, 'Integralrechnung','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (109, 2, 3, 'Stochastik','',6);
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (110, 2, 3, 'Analytische Geometrie','',6 );
  INSERT INTO [Module] (Id, FachId, JahrgangsstufeId, Bezeichnung, Bausteine, Stundenbedarf)
  VALUES (111, 5, 1, 'NN','',2);
  GO
  SET IDENTITY_INSERT [Module] OFF
  GO



  SET IDENTITY_INSERT [Medien] ON
  GO
  INSERT INTO [Medien] (Id, Bezeichnung)
  VALUES (1, '');
  INSERT INTO [Medien] (Id, Bezeichnung)
  VALUES (2, 'OH');
  INSERT INTO [Medien] (Id, Bezeichnung)
  VALUES (3, 'Tafel');
  INSERT INTO [Medien] (Id, Bezeichnung)
  VALUES (4, 'Beamer');
  INSERT INTO [Medien] (Id, Bezeichnung)
  VALUES (5, 'Experiment');
  GO
  SET IDENTITY_INSERT [Medien] OFF
  GO

  SET IDENTITY_INSERT [Sozialformen] ON
  GO
  INSERT INTO [Sozialformen] (Id, Bezeichnung)
  VALUES (1, '');
  INSERT INTO [Sozialformen] (Id, Bezeichnung)
  VALUES (2, 'GA');
  INSERT INTO [Sozialformen] (Id, Bezeichnung)
  VALUES (3, 'PA');
  INSERT INTO [Sozialformen] (Id, Bezeichnung)
  VALUES (4, 'LV');
  INSERT INTO [Sozialformen] (Id, Bezeichnung)
  VALUES (5, 'UG');
  GO
  SET IDENTITY_INSERT [Sozialformen] OFF
  GO
 
  SET IDENTITY_INSERT [Jahrtypen] ON
  GO
  INSERT INTO [Jahrtypen] (Id, Bezeichnung, Jahr)
  VALUES (1, '2010/2011', 2010);
  INSERT INTO [Jahrtypen] (Id, Bezeichnung, Jahr)
  VALUES (2, '2011/2012', 2011);
  INSERT INTO [Jahrtypen] (Id, Bezeichnung, Jahr)
  VALUES (3, '2012/2013', 2012);
  GO
  SET IDENTITY_INSERT [Jahrtypen] OFF
  GO

  SET IDENTITY_INSERT Ferien ON
  GO
  INSERT INTO Ferien (Id, JahrtypId, Bezeichnung, ErsterFerientag, LetzterFerientag)
  VALUES (1,2,'Herbstferien','10.04.2011','10.14.2011');
  INSERT INTO Ferien (Id, JahrtypId, Bezeichnung, ErsterFerientag, LetzterFerientag)
  VALUES (2,2,'Weihnachtsferien','12.23.2011','01.03.2012');
  INSERT INTO Ferien (Id, JahrtypId, Bezeichnung, ErsterFerientag, LetzterFerientag)
  VALUES (3,2,'Winterferien','01.30.2012','02.04.2012');
  INSERT INTO Ferien (Id, JahrtypId, Bezeichnung, ErsterFerientag, LetzterFerientag)
  VALUES (4,2,'Osterferien','04.02.2012','04.14.2012');
  INSERT INTO Ferien (Id, JahrtypId, Bezeichnung, ErsterFerientag, LetzterFerientag)
  VALUES (5,2,'Sommerferien','06.20.2012','08.03.2012');
  INSERT INTO Ferien (Id, JahrtypId, Bezeichnung, ErsterFerientag, LetzterFerientag)
  VALUES (6,3,'Herbstferien','10.01.2012','10.13.2012');
  INSERT INTO Ferien (Id, JahrtypId, Bezeichnung, ErsterFerientag, LetzterFerientag)
  VALUES (7,3,'Weihnachtsferien','12.24.2012','01.04.2013');
  INSERT INTO Ferien (Id, JahrtypId, Bezeichnung, ErsterFerientag, LetzterFerientag)
  VALUES (8,3,'Winterferien','02.04.2013','02.09.2013');
  INSERT INTO Ferien (Id, JahrtypId, Bezeichnung, ErsterFerientag, LetzterFerientag)
  VALUES (9,3,'Osterferien','03.25.2013','04.06.2013');
  INSERT INTO Ferien (Id, JahrtypId, Bezeichnung, ErsterFerientag, LetzterFerientag)
  VALUES (10,3,'Sommerferien','06.19.2013','08.01.2013');
  GO
  SET IDENTITY_INSERT Ferien OFF
  GO

  SET IDENTITY_INSERT [Halbjahrtypen] ON
  GO
  INSERT INTO [Halbjahrtypen] (Id, Bezeichnung, HalbjahrIndex)
  VALUES (1, 'Winter', 1);
  INSERT INTO [Halbjahrtypen] (Id, Bezeichnung, HalbjahrIndex)
  VALUES (2, 'Sommer', 2);
  GO
  SET IDENTITY_INSERT [Halbjahrtypen] OFF
  GO

  SET IDENTITY_INSERT [Monatstypen] ON
  GO
  INSERT INTO [Monatstypen] (Id, Bezeichnung, MonatIndex)
  VALUES (1, 'August', 8);
  INSERT INTO [Monatstypen] (Id, Bezeichnung, MonatIndex)
  VALUES (2, 'September', 9);
  INSERT INTO [Monatstypen] (Id, Bezeichnung, MonatIndex)
  VALUES (3, 'Oktober', 10);
  INSERT INTO [Monatstypen] (Id, Bezeichnung, MonatIndex)
  VALUES (4, 'November', 11);
  INSERT INTO [Monatstypen] (Id, Bezeichnung, MonatIndex)
  VALUES (5, 'Dezember', 12);
  INSERT INTO [Monatstypen] (Id, Bezeichnung, MonatIndex)
  VALUES (6, 'Januar', 1);
  INSERT INTO [Monatstypen] (Id, Bezeichnung, MonatIndex)
  VALUES (7, 'Februar', 2);
  INSERT INTO [Monatstypen] (Id, Bezeichnung, MonatIndex)
  VALUES (8, 'März', 3);
  INSERT INTO [Monatstypen] (Id, Bezeichnung, MonatIndex)
  VALUES (9, 'April', 4);
  INSERT INTO [Monatstypen] (Id, Bezeichnung, MonatIndex)
  VALUES (10, 'Mai', 5);
  INSERT INTO [Monatstypen] (Id, Bezeichnung, MonatIndex)
  VALUES (11, 'Juni', 6);
  INSERT INTO [Monatstypen] (Id, Bezeichnung, MonatIndex)
  VALUES (12, 'Juli', 7);
  GO
  SET IDENTITY_INSERT [Monatstypen] OFF
  GO

  SET IDENTITY_INSERT [Klassen] ON
  GO
  INSERT INTO [Klassen] (Id, KlassenstufeId, Bezeichnung)
  VALUES (1, 1, '7a');
  INSERT INTO [Klassen] (Id, KlassenstufeId, Bezeichnung)
  VALUES (2, 1, '7b');
  INSERT INTO [Klassen] (Id, KlassenstufeId, Bezeichnung)
  VALUES (3, 1, '7c');
  INSERT INTO [Klassen] (Id, KlassenstufeId, Bezeichnung)
  VALUES (4, 1, '7d');
  INSERT INTO [Klassen] (Id, KlassenstufeId, Bezeichnung)
  VALUES (5, 2, '8a');
  INSERT INTO [Klassen] (Id, KlassenstufeId, Bezeichnung)
  VALUES (6, 2, '8b');
  INSERT INTO [Klassen] (Id, KlassenstufeId, Bezeichnung)
  VALUES (7, 2, '8c');
  INSERT INTO [Klassen] (Id, KlassenstufeId, Bezeichnung)
  VALUES (8, 2, '8d');
  INSERT INTO [Klassen] (Id, KlassenstufeId, Bezeichnung)
  VALUES (9, 3, '9a');
  INSERT INTO [Klassen] (Id, KlassenstufeId, Bezeichnung)
  VALUES (10, 3, '9b');
  INSERT INTO [Klassen] (Id, KlassenstufeId, Bezeichnung)
  VALUES (11, 3, '9c');
  INSERT INTO [Klassen] (Id, KlassenstufeId, Bezeichnung)
  VALUES (12, 3, '9d');
  INSERT INTO [Klassen] (Id, KlassenstufeId, Bezeichnung)
  VALUES (13, 4, '10a');
  INSERT INTO [Klassen] (Id, KlassenstufeId, Bezeichnung)
  VALUES (14, 4, '10b');
  INSERT INTO [Klassen] (Id, KlassenstufeId, Bezeichnung)
  VALUES (15, 4, '10c');
  INSERT INTO [Klassen] (Id, KlassenstufeId, Bezeichnung)
  VALUES (16, 4, '10d');
  INSERT INTO [Klassen] (Id, KlassenstufeId, Bezeichnung)
  VALUES (17, 5, 'Q1/2-LK');
  INSERT INTO [Klassen] (Id, KlassenstufeId, Bezeichnung)
  VALUES (18, 5, 'Q1/2-GK1');
  INSERT INTO [Klassen] (Id, KlassenstufeId, Bezeichnung)
  VALUES (19, 5, 'Q1/2-GK2');
  INSERT INTO [Klassen] (Id, KlassenstufeId, Bezeichnung)
  VALUES (20, 5, 'Q1/2-GK3');
  INSERT INTO [Klassen] (Id, KlassenstufeId, Bezeichnung)
  VALUES (21, 6, 'Q3/4-LK');
  INSERT INTO [Klassen] (Id, KlassenstufeId, Bezeichnung)
  VALUES (22, 6, 'Q3/4-GK1');
  INSERT INTO [Klassen] (Id, KlassenstufeId, Bezeichnung)
  VALUES (23, 6, 'Q3/4-GK2');
  INSERT INTO [Klassen] (Id, KlassenstufeId, Bezeichnung)
  VALUES (24, 6, 'Q3/4-GK3');
  INSERT INTO [Klassen] (Id, KlassenstufeId, Bezeichnung)
  VALUES (25, 1, 'Alle');
  GO
  SET IDENTITY_INSERT [Klassen] OFF
  GO

  SET IDENTITY_INSERT [Termintypen] ON
  GO
  INSERT INTO [Termintypen] (Id, Bezeichnung, Kalenderfarbe)
  VALUES (1, 'Klausur','Yellow');
  INSERT INTO [Termintypen] (Id, Bezeichnung, Kalenderfarbe)
  VALUES (2, 'Tag der offenen Tür','Blue');
  INSERT INTO [Termintypen] (Id, Bezeichnung, Kalenderfarbe)
  VALUES (3, 'Wandertag','Magenta');
  INSERT INTO [Termintypen] (Id, Bezeichnung, Kalenderfarbe)
  VALUES (4, 'Abitur','Red');
  INSERT INTO [Termintypen] (Id, Bezeichnung, Kalenderfarbe)
  VALUES (5, 'MSA','Red');
  INSERT INTO [Termintypen] (Id, Bezeichnung, Kalenderfarbe)
  VALUES (6, 'Unterricht','LightBlue');
  INSERT INTO [Termintypen] (Id, Bezeichnung, Kalenderfarbe)
  VALUES (7, 'Vertretung','LightGray'); 
  INSERT INTO [Termintypen] (Id, Bezeichnung, Kalenderfarbe)
  VALUES (8, 'Besprechung','Orange');   
  INSERT INTO [Termintypen] (Id, Bezeichnung, Kalenderfarbe)
  VALUES (9, 'Sondertermin','Orange');
  INSERT INTO [Termintypen] (Id, Bezeichnung, Kalenderfarbe)
  VALUES (10, 'Ferien','Green');  
  GO
  SET IDENTITY_INSERT [Termintypen] OFF
  GO

  SET IDENTITY_INSERT [Unterrichtsstunden] ON
  GO
  INSERT INTO [Unterrichtsstunden] (Id, Bezeichnung, Beginn, Ende, Stundenindex)
  VALUES (1, '1', '08:00','08:45',1);
  INSERT INTO [Unterrichtsstunden] (Id, Bezeichnung, Beginn, Ende, Stundenindex)
  VALUES (2, '2', '08:50','09:35',2);
  INSERT INTO [Unterrichtsstunden] (Id, Bezeichnung, Beginn, Ende, Stundenindex)
  VALUES (3, '3', '09:50','10:35',3);
  INSERT INTO [Unterrichtsstunden] (Id, Bezeichnung, Beginn, Ende, Stundenindex)
  VALUES (4, '4', '10:40','11:25',4);
  INSERT INTO [Unterrichtsstunden] (Id, Bezeichnung, Beginn, Ende, Stundenindex)
  VALUES (5, '5', '11:45','12:30',5);
  INSERT INTO [Unterrichtsstunden] (Id, Bezeichnung, Beginn, Ende, Stundenindex)
  VALUES (6, '6', '12:35','13:20',6);
  INSERT INTO [Unterrichtsstunden] (Id, Bezeichnung, Beginn, Ende, Stundenindex)
  VALUES (7, '7', '13:30','14:15',7);
  INSERT INTO [Unterrichtsstunden] (Id, Bezeichnung, Beginn, Ende, Stundenindex)
  VALUES (8, '8', '14:25','15:20',8);
  INSERT INTO [Unterrichtsstunden] (Id, Bezeichnung, Beginn, Ende, Stundenindex)
  VALUES (9, '9', '15:30','16:15',9);
  INSERT INTO [Unterrichtsstunden] (Id, Bezeichnung, Beginn, Ende, Stundenindex)
  VALUES (10, '10+', '16:25','20:00',10);
  GO
  SET IDENTITY_INSERT [Unterrichtsstunden] OFF
  GO
