CREATE DATABASE ETrgovina;
GO
USE ETrgovina;
GO

-- 1. KATEGORIJA
CREATE TABLE Kategorija (
    KategorijaID INT IDENTITY PRIMARY KEY,
    Naziv NVARCHAR(100) NOT NULL UNIQUE
);
GO

-- 2. DRZAVA
CREATE TABLE Drzava (
    DrzavaID INT IDENTITY PRIMARY KEY,
    Naziv NVARCHAR(100) NOT NULL UNIQUE
);
GO

-- 3. PROIZVOD
CREATE TABLE Proizvod (
    ProizvodID INT IDENTITY PRIMARY KEY,
    Naziv NVARCHAR(100) NOT NULL,
    Opis NVARCHAR(MAX) NULL,
    Cijena DECIMAL(10,2) NOT NULL CHECK (Cijena >= 0),
    SlikaPath NVARCHAR(255) NULL, 
    KategorijaID INT NOT NULL,
    CONSTRAINT FK_Proizvod_Kategorija FOREIGN KEY (KategorijaID)
        REFERENCES Kategorija(KategorijaID) ON DELETE CASCADE
);
GO

-- 4. PROIZVOD_DRZAVA
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

-- 5. KORISNIK (Dodao sam LozinkaSalt odmah u CREATE)
CREATE TABLE Korisnik (
    KorisnikID INT IDENTITY PRIMARY KEY,
    Guid UNIQUEIDENTIFIER DEFAULT NEWID() NOT NULL, 
    Ime NVARCHAR(50) NOT NULL,
    Prezime NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    LozinkaHash NVARCHAR(MAX) NOT NULL, -- Povećano na MAX zbog dugih hasheva
    LozinkaSalt NVARCHAR(MAX) NULL,     -- Dodano odmah
    Uloga NVARCHAR(20) DEFAULT 'Korisnik' 
);
GO

-- 6. NARUDZBA
CREATE TABLE Narudzba (
    NarudzbaID INT IDENTITY PRIMARY KEY,
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    KorisnikID INT NOT NULL,
    UkupnaCijena DECIMAL(10,2) DEFAULT 0 CHECK (UkupnaCijena >= 0),
    CONSTRAINT FK_Narudzba_Korisnik FOREIGN KEY (KorisnikID)
        REFERENCES Korisnik(KorisnikID) ON DELETE CASCADE
);
GO

-- 7. NARUDZBA_PROIZVOD
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

-- 8. LOGOVI 
CREATE TABLE Logovi (
    LogID INT IDENTITY PRIMARY KEY,
    Tip NVARCHAR(50), 
    Poruka NVARCHAR(MAX),
    Datum DATETIME DEFAULT GETDATE(),
    KorisnikID INT NULL,
    CONSTRAINT FK_Logovi_Korisnik FOREIGN KEY (KorisnikID) REFERENCES Korisnik(KorisnikID)
);
GO

-- STORED PROCEDURE
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

-- INITIAL DATA
INSERT INTO Kategorija (Naziv) VALUES ('Elektronika'), ('Odjeća'), ('Knjige');
INSERT INTO Drzava (Naziv) VALUES ('Hrvatska'), ('Njemačka'), ('Austrija');

INSERT INTO Proizvod (Naziv, Opis, Cijena, KategorijaID, SlikaPath)
VALUES ('Galaxy S24', 'Mobitel', 899.99, 1, '/images/s24.jpg');

INSERT INTO ProizvodDrzava (ProizvodID, DrzavaID) VALUES (1, 1), (1, 2);
GO

DELETE FROM Korisnik WHERE Email = 'admin@example.com';
GO

INSERT INTO Korisnik (
    Ime, 
    Prezime, 
    Email, 
    LozinkaHash, 
    LozinkaSalt, 
    Uloga, 
    Guid
)
VALUES (
    'Admin', 
    'Adminko', 
    'admin@example.com', 
    'Hk4aSEUkjKTMzRCsaz6lXsoh3zP/JazojaQAN2z64Yo=', 
    'JZ/EY1KkyQ2c6ZpfFOdPGA==',                    
    'Admin',                                       
    NEWID()
);
GO

PRINT 'Admin korisnik (admin@example.com) je uspješno kreiran u bazi.';