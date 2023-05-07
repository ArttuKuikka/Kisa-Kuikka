# Kisa-Kuikka




- !!HUOM!! Kisa-Kuikka on vielä teon alle, eikä sisällä kaikkia ominaisuuksia joita siihen on tarkoitus lisätä, käytä ainoastaan testaamiseen!!

Kisa-Kuikka on asp.net core pohjainen ohjelmiston partitaitokisojen pisteiden laskuun ja vartioiden paikantamiseen NFC-tagien avulla. Kisa-Kuikka vähentää kisoissa tarvittavien papereiden määrää antamalla rastityöntekijöille mahdollisuuden syöttää rastien arvioinnit suoraan rastilta käsin puhelimen välityksessä.

Kisa-Kuikka tarkoitus on tehdä tulostoimiston toiminnasta helpompaa ja digitoida kisojen toimintaa muuten. Kisa-Kuikka tukee täysin digitaalista sekä paperista arviointia([Mitä eroa on "täysin digitaalisella" ja "paperisella" arvioinnilla](https://github.com/ArttuKuikka/Kisa-Kuikka/wiki/Mit%C3%A4-eroa-on-%22t%C3%A4ysin-digitaalisella%22-ja-%22paperisella%22-arvioinnilla)).

## Kisat joissa käytössä
- Kisa-Kuikka ensimmäinen testikerta oli Vainion Vesojen Kökäs kisan esi-kisassa 18.3.2023, jossa järjestelmä todettiin toimivaksi ja erittäin hyväksi lisäksi kisojen järjestämiseen.
- Ensimmäinen varsinainen käyttökerta 21.4.2023 KöKäs kisassa.

## Ominaisuuksia
- Tulosten syöttö suoraan rastilta käsin
- suunniteltu olemaan helppokäyttöinen puhelimilla
- vartioiden tilanneseuranta NFC-tagien avulla
- kätevä työkalu jonka avulla voi helposti seurata koko kisan edistymistä ja estää ruuhkia rasteilla (perustuu NFC-tagien dataan)
- rastien tilanteiden helppo hallinta (purettu, rakennettu yms)
- käyttäjä hallinta ja käyttäjien itserekisteröinti
- Ilmoitusten lähetys käyttäjille rooli tai rastikohdittain (Tukee myös WebPush ilmoitusten lähetystä)
- Täysin muokkattavat roolit jolloin käyttäjillä on oikeus vain määritettyihin asioihin
- Täysin muokattavat tehtäväpohjat jotka tehdään Google forms tapaisella työkalulla
- tulosten lataus excel muodossa
- Helppokäyttöinen käyttöliittymä ja määritys
- Helppo määritys palvelimelle



## Tulevavia ominaisuuksia
- Tuki tuloksien automaattiselli viennille normaaliin kipaan, jossa ne voidaan laskea (tällä hetkellä tulosten lataus excel muodossa on ainut vaihtoehto)
- Tuki monelle kisalle yhdessä järjestelmässä (tällä hetkellä järjestelmä toimii vain yhdellä kisalla vaikka monta kisaa voikin luoda)
- vartioiden tuonti csv tiedostosta
- järjestelmä logi jossa näkyy kaikki tärkeät tapahtumat
- PALJON pieniä korjailuja ja lisäilyjä
- Live tulosten jakaminen linkin kautta (saman lailla kuin Tilanneseuranta nyt)
- Tiedostonjen lataus tuki (tällä hetkellä keskeneräinen)
- videotykkinäkymä (ehkä)




## Simppeli asennus (paremmat ohjeet myöhemmin)
- Asenna Ubuntu server tai jokin muu linux käyttöjärjestelmä palvelintietokoneellesi
- asenna docker ja docker compose (https://docs.docker.com/engine/install/ubuntu/)
- luo kansio docker compose tiedostolle `mkdir KisaKuikka && cd KisaKuikka`
- lataa docker compose `wget https://raw.githubusercontent.com/ArttuKuikka/Kisa-Kuikka/Production/docker-compose.yml`
- (ei pakollista) vaihda tietokannan salasana docker-compose.yml tiedostossa komennolla `nano docker-compose.yml`
- suorita docker compose `sudo docker compose up -d`
- Valmista, mene selaimella osoitteeseen `http://palvelimen-ip:8001/` (jos tämä ei toimi, tarkista palvelimen palomuurin asetukset ja salli portti 8001)
- Kisa Kuikka on suunniteltu käytettäväksi reverse proxyn kanssa, joka huolehtii https suojaksesta, ohjeet tähän tulevat myöhemmin


## Muuta
Projektiin kuuluu myös toinen repository jossa on hieman muokattu versio kevinchappel/formbuilder:ista https://github.com/ArttuKuikka/KipaFormBuilder


Pull requesteja otetaan mielellään vastaan ja Jos sinulla on jotai kysyttävää, ota yhteyttä:
- Discord: Arttu#6180
- Sähköposti: arttu.juhani.kuikka@gmail.com
