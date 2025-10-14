CREATE DATABASE ETrgovina;
GO
USE ETrgovina;
GO

CREATE TABLE Kategorija (
    KategorijaID INT IDENTITY PRIMARY KEY,
    Naziv NVARCHAR(100) NOT NULL UNIQUE
);
GO

CREATE TABLE Proizvod (
    ProizvodID INT IDENTITY PRIMARY KEY,
    Naziv NVARCHAR(100) NOT NULL,
    Opis NVARCHAR(MAX) NULL,
    Cijena DECIMAL(10,2) NOT NULL CHECK (Cijena >= 0),
    KategorijaID INT NOT NULL,
    CONSTRAINT FK_Proizvod_Kategorija FOREIGN KEY (KategorijaID)
        REFERENCES Kategorija(KategorijaID) ON DELETE CASCADE
);
GO

CREATE TABLE Drzava (
    DrzavaID INT IDENTITY PRIMARY KEY,
    Naziv NVARCHAR(100) NOT NULL UNIQUE
);
GO

CREATE TABLE ProizvodDrzava (
    ProizvodID INT NOT NULL,
    DrzavaID INT NOT NULL,
    PRIMARY KEY (ProizvodID, DrzavaID),
    CONSTRAINT FK_ProizvodDrzava_Proizvod FOREIGN KEY (ProizvodID)
        REFERENCES Proizvod(ProizvodID) ON DELETE CASCADE,
    CONSTRAINT FK_ProizvodDrzava_Drzava FOREIGN KEY (DrzavaID)
        REFERENCES Drzava(DrzavaID) ON DELETE CASCADE
);
GO

CREATE TABLE Korisnik (
    KorisnikID INT IDENTITY PRIMARY KEY,
    Ime NVARCHAR(50) NOT NULL,
    Prezime NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    LozinkaHash NVARCHAR(255) NOT NULL,
    Uloga NVARCHAR(20) DEFAULT 'Korisnik'
);
GO

CREATE TABLE Narudzba (
    NarudzbaID INT IDENTITY PRIMARY KEY,
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    KorisnikID INT NOT NULL,
    UkupnaCijena DECIMAL(10,2) CHECK (UkupnaCijena >= 0),
    CONSTRAINT FK_Narudzba_Korisnik FOREIGN KEY (KorisnikID)
        REFERENCES Korisnik(KorisnikID) ON DELETE CASCADE
);
GO

CREATE TABLE NarudzbaProizvod (
    NarudzbaID INT NOT NULL,
    ProizvodID INT NOT NULL,
    Kolicina INT DEFAULT 1 CHECK (Kolicina > 0),
    PRIMARY KEY (NarudzbaID, ProizvodID),
    CONSTRAINT FK_NarudzbaProizvod_Narudzba FOREIGN KEY (NarudzbaID)
        REFERENCES Narudzba(NarudzbaID) ON DELETE CASCADE,
    CONSTRAINT FK_NarudzbaProizvod_Proizvod FOREIGN KEY (ProizvodID)
        REFERENCES Proizvod(ProizvodID) ON DELETE CASCADE
);
GO
-- Dohvati sve proizvode s paginacijom
CREATE OR ALTER PROCEDURE GetProizvodi
    @Page INT = 1,
    @Count INT = 10
AS
BEGIN
    SELECT * FROM Proizvod
    ORDER BY ProizvodID
    OFFSET (@Page - 1) * @Count ROWS
    FETCH NEXT @Count ROWS ONLY;
END;
GO

-- Pretraži proizvode po nazivu ili kategoriji
CREATE OR ALTER PROCEDURE SearchProizvodi
    @Search NVARCHAR(100)
AS
BEGIN
    SELECT p.*
    FROM Proizvod p
    INNER JOIN Kategorija k ON p.KategorijaID = k.KategorijaID
    WHERE p.Naziv LIKE '%' + @Search + '%'
       OR k.Naziv LIKE '%' + @Search + '%';
END;
GO

-- Kreiraj novu narudžbu
CREATE OR ALTER PROCEDURE KreirajNarudzbu
    @KorisnikID INT,
    @ProizvodID INT,
    @Kolicina INT = 1
AS
BEGIN
    DECLARE @NarudzbaID INT;

    INSERT INTO Narudzba (KorisnikID, UkupnaCijena)
    VALUES (@KorisnikID, 0);

    SET @NarudzbaID = SCOPE_IDENTITY();

    INSERT INTO NarudzbaProizvod (NarudzbaID, ProizvodID, Kolicina)
    VALUES (@NarudzbaID, @ProizvodID, @Kolicina);

    UPDATE Narudzba
    SET UkupnaCijena = (
        SELECT SUM(p.Cijena * np.Kolicina)
        FROM NarudzbaProizvod np
        INNER JOIN Proizvod p ON np.ProizvodID = p.ProizvodID
        WHERE np.NarudzbaID = @NarudzbaID
    )
    WHERE NarudzbaID = @NarudzbaID;
END;
GO

INSERT INTO Kategorija (Naziv)
VALUES 
('Elektronika'),
('Odjeća'),
('Knjige'),
('Igračke'),
('Kućanstvo');
GO

INSERT INTO Drzava (Naziv)
VALUES 
('Hrvatska'),
('Srbija'),
('Bosna i Hercegovina'),
('Slovenija'),
('Crna Gora'),
('Mađarska'),
('Njemačka'),
('Austrija'),
('Italija'),
('Francuska');
GO

INSERT INTO Proizvod (Naziv, Opis, Cijena, KategorijaID)
VALUES
-- ELEKTRONIKA
('Smartphone Galaxy S24', 'Pametni telefon s 6.5" zaslonom i trostrukom kamerom.', 899.99, 1),
('Laptop Lenovo IdeaPad 5', 'Tanak i lagan laptop s AMD Ryzen procesorom i 16GB RAM-a.', 749.00, 1),
('Bluetooth slušalice JBL Tune 720', 'Bežične slušalice s aktivnim poništavanjem buke.', 119.99, 1),
('Televizor LG OLED55', '55" OLED 4K televizor s HDR podrškom.', 1099.99, 1),
('Pametni sat Garmin Venu 3', 'Fitness sat s GPS-om i monitorom otkucaja srca.', 349.99, 1),

-- ODJEĆA
('Muška jakna NorthWind', 'Vodootporna zimska jakna od visokokvalitetnih materijala.', 129.99, 2),
('Ženska majica ActiveFit', 'Sportska majica od prozračnog materijala.', 34.99, 2),
('Traperice UrbanStyle', 'Moderne traperice ravnog kroja, tamnoplave boje.', 59.99, 2),
('Sportske tenisice AirMove', 'Udobne tenisice za svakodnevno nošenje.', 89.99, 2),
('Zimska kapa Classic Knit', 'Topla vunena kapa, dostupna u više boja.', 19.99, 2),

-- KNJIGE
('1984 – George Orwell', 'Klasični distopijski roman o totalitarizmu i nadzoru.', 12.99, 3),
('Harry Potter i Kamen mudraca', 'Prvi nastavak popularnog serijala o mladom čarobnjaku.', 14.99, 3),
('Bogati otac, siromašni otac', 'Knjiga o financijskoj pismenosti Roberta Kiyosakija.', 11.49, 3),
('Ubiti pticu rugalicu', 'Roman o pravdi, rasi i odrastanju u SAD-u 30-ih godina.', 13.99, 3),
('Mali princ', 'Bezvremenska priča o djetinjstvu i ljubavi, autora A. de Saint-Exupéryja.', 9.99, 3),

-- IGRAČKE
('LEGO Star Wars set', 'Veliki LEGO set s preko 1000 dijelova za sve uzraste.', 89.99, 4),
('Lutka Barbie Fashionista', 'Barbie lutka s modernom odjećom i dodacima.', 24.99, 4),
('Društvena igra Monopoly', 'Klasik među društvenim igrama za cijelu obitelj.', 29.99, 4),
('Puzzle 1000 dijelova – Slapovi Niagare', 'Predivne puzzle s prirodnim motivima.', 14.99, 4),
('RC Auto SpeedRacer', 'Daljinski upravljani automobil s punjivom baterijom.', 39.99, 4),

-- KUĆANSTVO
('Aparat za kavu Philips Senseo', 'Kompaktan aparat za kapsule, idealan za jutarnju kavu.', 79.99, 5),
('Usisavač Dyson V11', 'Bežični usisavač visoke snage i HEPA filtracijom.', 499.99, 5),
('Tava Tefal Titanium', 'Neljepljiva tava s dugotrajnom površinom.', 29.99, 5),
('Mikser Bosch ErgoMixx', 'Snažan ručni mikser s više brzina i nastavaka.', 49.99, 5),
('Set čaša Luminarc', 'Elegantan set od 6 čaša za vino.', 19.99, 5),
('Pegla Rowenta ProSteam', 'Pegla na paru s keramičkom pločom.', 59.99, 5),
('Mini frižider Klarstein', 'Kompaktan frižider za ured ili sobu.', 149.99, 5),
('Set noževa Zwilling', 'Profesionalni kuhinjski noževi od nehrđajućeg čelika.', 89.99, 5),
('Grijač vode Bosch Compact', 'Električni grijač vode kapaciteta 10 litara.', 129.99, 5),
('Aparat za palačinke Severin', 'Ploča za pečenje palačinki s neljepljivom površinom.', 44.99, 5);
GO

-- 🔹 4. POVEZIVANJE PROIZVODA I DRŽAVA (M:N)
-- Svaki proizvod će biti dostupan u nekoliko država (2–5)
DECLARE @ProizvodID INT = 1;
DECLARE @DrzavaID INT;

WHILE @ProizvodID <= 30
BEGIN
    DECLARE @BrojDrzava INT = (CAST(RAND() * 4 AS INT) + 2);
    DECLARE @Brojac INT = 0;

    WHILE @Brojac < @BrojDrzava
    BEGIN
        SET @DrzavaID = (CAST(RAND() * 10 AS INT) + 1);
        IF NOT EXISTS (SELECT 1 FROM ProizvodDrzava WHERE ProizvodID = @ProizvodID AND DrzavaID = @DrzavaID)
            INSERT INTO ProizvodDrzava (ProizvodID, DrzavaID)
            VALUES (@ProizvodID, @DrzavaID);
        SET @Brojac += 1;
    END;
    SET @ProizvodID += 1;
END;
GO
