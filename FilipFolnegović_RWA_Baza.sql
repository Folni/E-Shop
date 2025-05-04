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

create table Dr�ava (
	Dr�avaID int identity,
	Naziv nvarchar(50),
	IDProizvod int,
	constraint PK_Dr�ava primary key (Dr�avaID),
	constraint FK_Proizvod foreign key (IDProizvod) references Proizvod(ProizvodID)
);
go

create table Korisnik (
	KorisnikID int identity,
	Ime nvarchar(50),
	Prezime nvarchar(50),
	IDNarud�ba int,
	constraint PK_Korisnik primary key (KorisnikID)
);
go

create table Narud�ba (
	Narud�baID int identity,
	BrojNarud�be nvarchar(10),
	IDKorisnik int,
	IDProizvod int,
	constraint PK_Narud�ba primary key (Narud�baID),
	constraint FK_Korisnik foreign key (IDKorisnik) references Korisnik(KorisnikID),
	constraint FK_Proizvod_2 foreign key (IDProizvod) references Proizvod(ProizvodID)
);
go

alter table Korisnik
add constraint FK_Narud�ba foreign key (IDNarud�ba) references Narud�ba(Narud�baID);
go

alter table Proizvod
add IDDr�ava int;
go

alter table Proizvod
add constraint FK_Dr�ava foreign key (IDDr�ava) references Dr�ava(Dr�avaID);
go

alter table Proizvod
add IDNarud�ba int;
go

alter table Proizvod
add constraint FK_Narud�ba_2 foreign key (IDNarud�ba) references Narud�ba(Narud�baID);

