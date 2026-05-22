# Documentation
## Planification du projet

|  | N ° semaine |  |  |  |  |  |  |  |  |  |  |  |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Lot | 46 | 48 | 50 | 2 | 3 | 5 | 7 | 10 | 12 | 14 | 18 | 21 |
| Gestion de projet | 3 | 2 | 1 | 1 | 1 | 1 | 1 | 1 | 1 | 1 | 2 | 2 |
| Plateforme de streaming | 1 | 1 | 1 | 1 | 1 | 1 | 1 | 1 | 1 | 1 | 1 | 1 |
| Jeux vidéo | 2 | 2 | 3 | 3 | 3 | 2 | 2 | 2 | 2 | 2 | 1 | 1 |
| Base de données | 0 | 0 | 0 | 0 | 0 | 2 | 2 | 2 | 2 | 2 | 2 | 2 |
| Génération de niveau par la musique | 0 | 1 | 1 | 1 | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 |
|  | 6 | 6 | 6 | 6 | 6 | 6 | 6 | 6 | 6 | 6 | 6 | 6 |

## Organisation de l'équipe

```mermaid
graph TD
    %% Titre principal
    T["<b>EQUIPE</b>"]
    style T fill:none,stroke:none,font-size:36px,color:#000,margin-bottom:40px

    %% Ligne 1 : Management
    QB("Quentin Brulé<br/><b>Scrum Master</b>")
    AB("Arthur Blamart<br/><b>Product Owner</b>")
    
    %% Ligne 2 : Développement
    FA("Florian Abadie<br/><i>Développeur<br/>et Git Master</i>")
    CA("Chloé Aubry<br/><i>Développeuse</i>")
    SH("Sothaline Huot<br/><i>Développeuse et<br/>Responsable UX/UI</i>")
    
    %% Ligne 3 : Développement
    CJ("Clem Jourdin<br/><i>Développeur</i>")
    VR("Victor Rouet<br/><i>Développeur</i>")

    %% Liens invisibles pour forcer le placement visuel
    T ~~~ QB & AB
    QB ~~~ FA
    AB ~~~ SH
    FA ~~~ CA ~~~ SH
    CA ~~~ CJ & VR

    %% Style global des bulles
    classDef member fill:#ffbd4a,stroke:none,color:#111,rx:25,ry:25,padding:15px;
    class QB,AB,FA,CA,SH,CJ,VR member;
```