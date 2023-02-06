# Kipa-plus




- !!HUOM!! Kipa-plus on vielä teon alle, eikä sisällä kaikkia ominaisuuksia joita siihen on tarkoitus lisätä, käytä ainoastaan testaamiseen!!

Kipa-plus on asp.net core pohjainen ohjelmiston partitaitokisojen pisteiden laskuun ja vartioiden paikantamiseen NFC-tagien avulla. Kipa-plus vähentää kisoissa tarvittavien papereiden antamalla rastityöntekijöille mahdollisuuden syöttää rastien arvioinnit suoraan rastilta käsin puhelimen välityksessä.

Kipa-plus tarkoitus on tehdä tulostoimiston toiminnasta helpompaa ja digitoida kisojen toimintaa muuten. Kipa-plus tukee täysin digitaalista arviontia rastilla, mutta myös paperista jolloin Kipa-plussaan syötetään vain paperilta yhteenlaskettu tulos sekä muut tulostoimostolle tarpeelliset tiedot.

## Kisat joissa käytössä
Kipa-plus ensimmäinen testikerta tulee olemaan Vainion Vesojen Kökäs kisan esi kisassa 18.3.2023 ja ensimmäinen varsinainen käyttökerta 21.4.2023 virallisessa kisassa.




## Tulevavia ominaisuuksia
- Käyttäjä ja roolihallinta
- Kipa-plussaan on myöhemmin tarkoitus lisätä suora tuki normaaliin kipaan tulosten laskua varten, nyt Kipa-plus tukee vain tulosten syötettyjen vastausten latausta excel-muodossa, jota tullaan käyttämään Kökäs kisassa.


## Simppeli asennus (paremmat ohjeet myöhemmin)
- Asenna Ubuntu server tai jokin muu linux käyttöjärjestelmä palvelintietokoneellesi
- asenna docker ja docker compose (https://docs.docker.com/engine/install/ubuntu/)
- luo kansio docker compose tiedostolle `mkdir KipaPlus && cd KipaPlus`
- lataa docker compose `wget https://raw.githubusercontent.com/ArttuKuikka/Kipa-plus/Production/docker-compose.yml`
- (ei pakollista) vaihda tietokannan salasana docker-compose.yml tiedostossa komennolla `nano docker-compose.yml`
- suorita docker compose `sudo docker compose up -d`
- Valmista, mene palvemimen selaimella osoitteeseen `http://palvelimen-ip:8001/` (jos tämä ei toimi, tarkista palvelimen palomuurin asetukset ja salli portti 8001)
- Kipa plus on suunniteltu käytettyväksi reverse proxyn kanssa, joka huolehtii https suojaksesta, ohjeet tähän tulevat myöhemmin


## Muuta
Projektiin kuuluu myös toinen repository jossa on hieman muokattu versio kevinchappel/formbuilder:ista https://github.com/ArttuKuikka/KipaFormBuilder


Pull requesteja otetaan mielellään vastaan ja Jos sinulla on jotai kysyttävää, ota yhteyttä:
- Discord: Arttu#6180
- Sähköposti: arttu.juhani.kuikka@gmail.com
