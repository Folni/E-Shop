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

-- 5. KORISNIK
CREATE TABLE Korisnik (
    KorisnikID INT IDENTITY PRIMARY KEY,
    Guid UNIQUEIDENTIFIER DEFAULT NEWID() NOT NULL, 
    Ime NVARCHAR(50) NOT NULL,
    Prezime NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    LozinkaHash NVARCHAR(MAX) NOT NULL,
    LozinkaSalt NVARCHAR(MAX) NULL,     
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

-- INITIAL DATA - 10 DRŽAVA
INSERT INTO Drzava (Naziv) VALUES 
('Hrvatska'), ('Njemačka'), ('Austrija'), ('Slovenija'), ('Italija'), 
('Francuska'), ('Španjolska'), ('Nizozemska'), ('Švicarska'), ('Belgija');

-- INITIAL DATA - 15 KATEGORIJA
INSERT INTO Kategorija (Naziv) VALUES 
('Pametni telefoni'), ('Prijenosna računala'), ('Televizori'), ('Audio oprema'), ('Fotoaparati'),
('Muška odjeća'), ('Ženska odjeća'), ('Obuća'), ('Sportska oprema'), ('Fitnes uređaji'),
('Beletristika'), ('Znanstvena fantastika'), ('Kuharice'), ('Dječje igračke'), ('Uredski pribor');

-- INITIAL DATA - 40 PROIZVODA
INSERT INTO Proizvod (Naziv, Opis, Cijena, KategorijaID, SlikaPath) VALUES 
('iPhone 15 Pro', 'Vrhunski pametni telefon s titanijskim kućištem.', 1249.00, 1, '/images/iphone15.jpg'),
('Samsung Galaxy S24', 'Najnoviji Android s AI značajkama.', 999.00, 1, '/images/s24.jpg'),
('Google Pixel 8', 'Čisti Android s najboljom kamerom.', 799.00, 1, '/images/pixel8.jpg'),
('Xiaomi 14 Ultra', 'Leica optika za profesionalne fotografije.', 1100.00, 1, '/images/xiaomi14.jpg'),
('MacBook Air M3', 'Lagan i snažan laptop za rad u pokretu.', 1450.00, 2, '/images/macbook.jpg'),
('Dell XPS 15', 'Premium laptop s 4K zaslonom.', 2100.00, 2, '/images/dellxps.jpg'),
('Lenovo Legion 5', 'Gaming laptop vrhunskih performansi.', 1300.00, 2, '/images/lenovo.jpg'),
('Sony Bravia 4K', 'OLED TV s realističnim prikazom boja.', 1800.00, 3, '/images/sony_tv.jpg'),
('LG C3 OLED', 'Savršen TV za gaming i filmove.', 1600.00, 3, '/images/lgtv.jpg'),
('AirPods Pro 2', 'Bežične slušalice s aktivnim poništavanjem buke.', 279.00, 4, '/images/airpods.jpg'),
('Sony WH-1000XM5', 'Najbolje over-ear slušalice na tržištu.', 350.00, 4, '/images/sony_audio.jpg'),
('Canon EOS R5', 'Mirrorless fotoaparat za profesionalce.', 3800.00, 5, '/images/canon.jpg'),
('Nikon Z6 II', 'Svestran fotoaparat za video i foto.', 2200.00, 5, '/images/nikon.jpg'),
('Muška kožna jakna', 'Klasična crna jakna od prave kože.', 150.00, 6, '/images/jakna.jpg'),
('Levi''s 501 Jeans', 'Originalni traper kroj koji nikad ne izlazi iz mode.', 90.00, 6, '/images/levis.jpg'),
('Ljetna haljina', 'Lagana cvjetna haljina za sunčane dane.', 45.00, 7, '/images/haljina.jpg'),
('Ženska torba', 'Elegantna kožna torba za svakodnevnu upotrebu.', 120.00, 7, '/images/torba.jpg'),
('Nike Air Max 270', 'Sportske tenisice s maksimalnom udobnošću.', 140.00, 8, '/images/nike.jpg'),
('Adidas Ultraboost', 'Idealne tenisice za trčanje na duge staze.', 160.00, 8, '/images/adidas.jpg'),
('Kožne cipele', 'Svečane muške cipele za poslovne prilike.', 110.00, 8, '/images/cipele.jpg'),
('Wilson teniska loptica', 'Set od 4 loptice za natjecanja.', 15.00, 9, '/images/wilson.jpg'),
('Nogometna lopta', 'Službena lopta za turnire.', 30.00, 9, '/images/lopta.jpg'),
('Garmin Fenix 7', 'Multisport pametni sat za avanturiste.', 650.00, 10, '/images/garmin.jpg'),
('Apple Watch Ultra 2', 'Robustan sat za ekstremne uvjete.', 899.00, 10, '/images/apple_watch.jpg'),
('Fitbit Charge 6', 'Napredni tracker aktivnosti i sna.', 160.00, 10, '/images/fitbit.jpg'),
('Sjena vjetra', 'Carlos Ruiz Zafon - Misteriozan roman.', 19.90, 11, '/images/knjiga1.jpg'),
('Harry Potter i Kamen mudraca', 'Prvi dio legendarne sage.', 25.00, 12, '/images/hp1.jpg'),
('Dune', 'Frank Herbert - Kultni SF klasik.', 22.00, 12, '/images/dune.jpg'),
('Talijanska kuharica', 'Tradicionalni recepti iz srca Italije.', 35.00, 13, '/images/kuharica.jpg'),
('LEGO Star Wars', 'Millennium Falcon set za slaganje.', 170.00, 14, '/images/lego.jpg'),
('Društvena igra Catan', 'Strateška igra za cijelu obitelj.', 40.00, 14, '/images/catan.jpg'),
('Mehanička tipkovnica', 'RGB tipkovnica s plavim prekidačima.', 85.00, 15, '/images/tipkovnica.jpg'),
('Logitech MX Master 3S', 'Ergonomski miš za produktivnost.', 100.00, 15, '/images/mis.jpg'),
('Bose QuietComfort', 'Slušalice s najboljom izolacijom.', 320.00, 4, '/images/bose.jpg'),
('Samsung Odyssey G7', 'Zakrivljeni gaming monitor 240Hz.', 600.00, 2, '/images/monitor.jpg'),
('Gopro Hero 12', 'Akcijska kamera otporna na vodu.', 450.00, 5, '/images/gopro.jpg'),
('Yoga prostirka', 'Neklizajuća podloga za vježbanje.', 25.00, 9, '/images/yoga.jpg'),
('Zimska kapa', 'Vunena kapa za hladne dane.', 15.00, 6, '/images/kapa.jpg'),
('Svilena marama', 'Luksuzan modni dodatak.', 55.00, 7, '/images/marama.jpg'),
('Stolna lampa', 'Moderna LED lampa s bežičnim punjačem.', 70.00, 15, '/images/lampa.jpg');

-- NASUMIČNA DOSTUPNOST PO DRŽAVAMA (Povezivanje prvih 10 proizvoda s par država)
INSERT INTO ProizvodDrzava (ProizvodID, DrzavaID) VALUES 
(1,1), (1,2), (1,3), (2,1), (2,4), (3,2), (3,5), (4,1), (5,3), (5,6), (6,1), (6,2), (7,4), (8,5), (9,1), (10,2);

-- ADMIN KORISNIK
INSERT INTO Korisnik (Ime, Prezime, Email, LozinkaHash, LozinkaSalt, Uloga, Guid)
VALUES ('Admin', 'Adminko', 'admin@example.com', 
'Hk4aSEUkjKTMzRCsaz6lXsoh3zP/JazojaQAN2z64Yo=', 
'JZ/EY1KkyQ2c6ZpfFOdPGA==', 'Admin', NEWID());
GO

PRINT 'Baza je uspješno popunjena.';