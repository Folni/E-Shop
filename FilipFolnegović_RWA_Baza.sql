create database ETrgovina;
use ETrgovina;
go

create table Kategorija (
	KategorijaID int identity,
	Naziv nvarchar(50),
	constraint PK_Kategorija primary key (KategorijaID)
);
go

create table Proizvod (
	ProizvodID int identity,
	Naziv nvarchar(50),
	Cijena money,
	Opis nvarchar(max),
	IDKategorija int,
	constraint PK_Proizvod primary key (ProizvodID),
	constraint FK_Kategorija foreign key (IDKategorija) references Kategorija(KategorijaID)
);
go

create table Država (
	DržavaID int identity,
	Naziv nvarchar(50),
	IDProizvod int,
	constraint PK_Država primary key (DržavaID),
	constraint FK_Proizvod foreign key (IDProizvod) references Proizvod(ProizvodID)
);
go

create table Korisnik (
	KorisnikID int identity,
	Ime nvarchar(50),
	Prezime nvarchar(50),
	IDNarudžba int,
	constraint PK_Korisnik primary key (KorisnikID)
);
go

create table Narudžba (
	NarudžbaID int identity,
	BrojNarudžbe nvarchar(10),
	IDKorisnik int,
	IDProizvod int,
	constraint PK_Narudžba primary key (NarudžbaID),
	constraint FK_Korisnik foreign key (IDKorisnik) references Korisnik(KorisnikID),
	constraint FK_Proizvod_2 foreign key (IDProizvod) references Proizvod(ProizvodID)
);
go

alter table Korisnik
add constraint FK_Narudžba foreign key (IDNarudžba) references Narudžba(NarudžbaID);
go

alter table Proizvod
add IDDržava int;
go

alter table Proizvod
add constraint FK_Država foreign key (IDDržava) references Država(DržavaID);
go

alter table Proizvod
add IDNarudžba int;
go

alter table Proizvod
add constraint FK_Narudžba_2 foreign key (IDNarudžba) references Narudžba(NarudžbaID);

