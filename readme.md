# Car Rental API

## Så här kör du projektet
1. Starta applikationen i Visual Studio
2.Apiet körs på din lokala HTTPS-port som du ser i `Properties\launchsettings.json`

Kör en klient, t ex Bruno eller använd scalar på adressen https://localhost:<din lokala port>/scalar/v1

## JWT säkerhet
Alla rental endpoints kräver en giltig JWT innehållande bokarens personummer.

## Skapa en lokal token
Öppna en terminal i solution foldern och kör:
dotnet user-jwts create --name demouser --claim "nationalid=19700101-1234"
Kopiera token och lägg in den i en Authorization header i din valda klient (Bruno, Scalar)

Skapa ev en andra token för att prova att återlämna bilar för någon annan person
dotnet user-jwts create --name demouser --claim "nationalid=19600101-1234"
