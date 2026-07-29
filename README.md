# System zarządzania rezerwacjami

Aplikacja ASP.NET Core MVC do zarządzania rezerwacjami sal. Administrator może obsługiwać rezerwacje i konta użytkowników, a zwykły użytkownik korzystać z uproszczonego panelu rezerwacji.

## Technologie

- .NET 10 i ASP.NET Core MVC
- Entity Framework Core 10
- SQL Server LocalDB
- Bootstrap

## Wymagania

- .NET SDK 10
- SQL Server Express LocalDB
- narzędzie `dotnet-ef`

Sprawdzenie środowiska:

```powershell
dotnet --version
dotnet ef --version
sqllocaldb info
```

## Szybkie uruchomienie

Przejdź do katalogu aplikacji:

```powershell
cd .\Projekt-Zarzadzanie-Rezerwacjami
```

Przywróć pakiety i utwórz bazę:

```powershell
dotnet restore
dotnet ef database update
```

Przy pierwszym uruchomieniu pustej bazy ustaw dane początkowego administratora. Hasło nie jest zapisywane w repozytorium:

```powershell
$env:InitialAdmin__Login = "admin"
$env:InitialAdmin__Password = "ZmienToHaslo123!"
dotnet run --launch-profile http
```

Aplikacja będzie dostępna pod adresem:

```text
http://localhost:5020
```

Początkowy administrator jest tworzony tylko wtedy, gdy tabela użytkowników jest pusta. Przy kolejnych uruchomieniach zmienne `InitialAdmin` nie zmieniają istniejącego konta.

## Role

- `admin` — zarządzanie rezerwacjami i użytkownikami,
- `user` — dostęp do panelu użytkownika i rezerwacji.

## Hasła użytkowników

Hasła są haszowane przez wbudowany `PasswordHasher` z ASP.NET Core:

- panel administratora haszuje hasło przed zapisem,
- logowanie porównuje podane hasło z zapisanym hashem,
- stare hasła zapisane jawnym tekstem są automatycznie haszowane przy pierwszym uruchomieniu nowej wersji,
- lista użytkowników nie wyświetla haseł ani hashy.

Nie dodawaj prawdziwych haseł do `appsettings.json` ani do repozytorium.

## Migracje bazy

Historia projektu została scalona do jednej migracji bazowej `InitialCreate`. Tworzy ona tabele `Room`, `Rezerwacja` i `Uzytkownik`, indeks unikalny loginu oraz sześć podstawowych sal.

Po kolejnej zmianie modelu użyj opisowej nazwy migracji:

```powershell
dotnet ef migrations add AddReservationStatus
dotnet ef database update
```

Przed zatwierdzeniem migracji warto sprawdzić:

```powershell
dotnet ef migrations list
dotnet ef migrations has-pending-model-changes
dotnet build
```

Nie edytuj ręcznie migracji, która została już zastosowana na współdzielonej lub produkcyjnej bazie.

## Konfiguracja bazy

Połączenie znajduje się w pliku `Projekt-Zarzadzanie-Rezerwacjami/appsettings.json` i domyślnie korzysta z instancji:

```text
(localdb)\mssqllocaldb
```

Jeżeli LocalDB nie działa, uruchom instancję:

```powershell
sqllocaldb start MSSQLLocalDB
```

## Struktura projektu

```text
Projekt-Zarzadzanie-Rezerwacjami/
├── Controllers/    kontrolery MVC
├── Data/           DbContext, kontrolery danych i migracja haseł
├── Migrations/     migracje Entity Framework Core
├── Models/         modele domenowe i walidacja
├── Views/          widoki Razor
└── wwwroot/        pliki statyczne
```

## Przydatne komendy

```powershell
dotnet build
dotnet run --launch-profile http
dotnet ef migrations list
dotnet ef database update
dotnet list package --vulnerable
```
