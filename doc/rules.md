# Aperçu
Vous avez 8 heures pour concevoir une intelligence artificielle (IA ou AI) permettant d’accumuler un maximum de points à notre petit jeu.

Vous serez évalués sur le talent de votre AI à accumuler des points, à attaquer ses adversaires, et à boire de la bière...

# Vous aurez à votre disposition
- Cette documentation
- Différents outils et exemples de code dans les langages supportés
  - Simulateur de parties
  - Exemple d’AI

# Quelques règles pour une saine compétition
- L’utilisation de librairies externes doit être autorisée.
  - Aucune librairie d’intelligence artificielle, de parcours de graphe ou de ce type n’est permise. L’objectif du concours est que vous codiez votre propre intelligence artificielle!
- Tout le code doit être écrit sur place la journée même.
- Toute action portant à nuire à une autre équipe est interdite.
- Nous nous gardons le droit d’inspecter le code source des solutions pour s’assurer que les règles ont été respectées.
- Le non-respect des règles peut mener à l’élimination de l’équipe.
- Un test antidopage sera réalisé à la fin du défi pour s'assurer que le code a été réalisé sans l’aide de produits dopants. :P

# Horaire
- 9h00        Arrivée et installation des participants
- 9h30        Présentation du défi
- 9h45        Début de la compétition
- 12h00       Ronde #1, dîner
- 14h00       Ronde #2
- 16h00       Phase éliminatoire
- 17h30       Remise des prix et direction bière

Les informations de dernière minute vous seront transmises via Slack sur le canal #ai. Assurez-vous d’être inscrit sur Slack et d’avoir les notifications appropriées pour ne pas manquer de messages importants!

# Les rondes
**Vous ne pouvez participer qu’une seule fois par match.
Assurez-vous donc que votre code fonctionne!**

# Généralités
## Saison régulière
Durant les 2 premières rondes, votre AI devra faire plusieurs matchs par ronde
et chaque match oppose 4 équipes.  La deuxième ronde aura plus d'impact sur les
points que la première.

| Ronde | Position de l’équipe | Points attribués |
| --- | --- | --- |
1 | 1 | 3 |
  | 2 | 2 |
  | 3 | 1 |
  | 4 | 0 |
2 | 1 | 6 |
  | 2 | 4 |
  | 3 | 2 |
  | 4 | 0 |

En cas d'égalité dans un match, les équipes égales auront le nombre de points
attribués au plus bas rang commun (ex: s’il y a égalité en 2e place, les deux
équipes se partageront les points donnés à la 3e place).

Les points des rondes 1 et 2 servent au classement des équipes pour la phase
éliminatoire. En cas d’égalité entre des équipes après la saison régulière,
l’équipe ayant accumulé le plus de points dans le ou les matchs les mettant en
vedette sera favorisée par rapport à l’autre.

## Phase éliminatoire
À 16h commenceront les éliminatoires. Pendant qu’elles se déroulent, nous allons
inviter les équipes à venir décrire leur AI.

**À noter que nous allons surement devoir changer les chiffres ici, si nous
n'avons pas le bon nombre d'équipe.**

Basés sur le classement des équipes suivant les rondes précédentes, les premiers
matchs opposeront les meilleures équipes aux pires). Les deux premières équipes
de chacun de ces quatre matchs s’affronteront entre elles dans deux parties
éliminatoires qui éliminera les deux équipes qui finiront 4e. Quant à elles, les
deux dernières équipes s’affronteront dans deux parties suicides où seules les
deux équipes qui finiront en tête survivront. Ensuite, les deux meilleures
équipes des demi-finales se feront face en finale pour finalement déterminer
celle qui remportera les grands honneurs.

**Note**: En cas d’égalité dans le cadre d’un match de phase éliminatoire,
l’équipe ayant remporté le plus de points dans les rondes 1 et 2 l’emporte.

### Comment gagner?
**L'équipe gagnante est celle qui termine première lors du dernier match.**

Dans la phase éliminatoire, les points ne servent qu’à déterminer les équipes
gagnantes et perdantes.

# Détails et bidules utiles
Cette section contient des informations détaillées pour vous aider durant le concours.

## Projet de base
Pour votre langage choisi, il y a un projet de base. Celui-ci inclut déjà toute la communication nécessaire avec le serveur. L’AI y est cependant légèrement simpliste (RNG style).

## Interface de communication
La communication avec le serveur passe par le protocole REST. Votre AI communique son prochain mouvement au serveur, et lorsque c’est votre tour, l’appel revient avec l’état du jeu. Vous devez donc rappeler et fournir votre mouvement, et ce, tous les tours.

## Faire des essais
Vous pourrez, à tout moment, faire des rondes d'essais pour voir comment votre AI se comporte. Vous serez mis contre 3 AI, aucune d’une autre équipe. Vous pouvez rouler vos essais de vos machines.

## Les rondes officielles
Pour les rondes officielles, nous vous fournirons un identifiant de partie qui vous permettra de vous connecter au bon groupe. La partie attendra pendant 1 minute que tous les joueurs soient connectés avant de commencer. Si vous n’êtes pas connectés après 1 minute, vous serez considéré comme ayant abandonné.

Si une partie subit un problème technique (tel que jugé par nous), les résultats seront annulés et la partie sera recommencée.

## Mais l’AI, il fait quoi?
Votre AI contrôle un héros. Celui-ci est sur une île. Il doit accumuler des pièces d’or (points) en ayant des mines d’or, tenter de les défendre, et survivre aux attaques des autres joueurs. Ils doivent aussi gérer leur énergie pour ne pas mourir.

### Héros
Il ne peut bouger que d’un espace par tour. Il a un maximum de 100 points d’énergie. Si son énergie tombe à 0, il meurt.

### Un tour
#### Mouvement du héros
Un héros peut faire un seul mouvement par tour, parmi 5 choix; un des points cardinaux, ou rester sur sa position.

Si le héros tente de :
- Sortir de la carte ou de marcher sur un arbre, il ne se passe rien
- Marcher sur un mine d’or, il reste sur place et :
  - Si la mine lui appartient déjà, il ne se passe rien
  - Si la mine n’appartient à aucun joueur, ou est à un autre joueur, le héros perd 25 points d’énergie. S’il survit, la mine lui appartient désormais.
- Marcher sur un héros, il ne se passe rien (les combats sont résolus plus tard)
- Marche sur une taverne, il paye 1 pièce d’or et reçoit 25 points d’énergie (il ne peut dépasser 100).
- Marcher sur un pic-pic, il se déplace dessus et perd 10 points d’énergie. Si son énergie est à 0 ou moins, il meurt.

#### Après le mouvement
##### Combat
Après le mouvement de votre héros, s’il est adjacent à un autre héros, il l’attaque automatiquement. Un héros est adjacent s’il occupe une case à gauche, à droite, en haut ou en bas de votre héros. Aucune diagonale. Un héros qui se fait attaquer perd 25 points d’énergie. S’il meurt, toutes ses mines sont transférées au tueur.

##### Mines d’or
À chaque tour, il gagne une pièce d’or pour chaque mine qu’il contrôle.

##### Énergie
Puisque c’est épuisant tout ça, le héros perd 1 d’énergie chaque tour. S’il est déjà à 1 d’énergie, il restera cependant à un (il ne peut mourir d’épuisement).

##### Mort d’un héros
Si son énergie est à 0 (ou moins), il meurt. Il revient à la vie immédiatement avec 100 d’énergie. Il perd toutes ses mines, mais conserve les pièces d’or déjà accumulées. Il revient à sa position initiale. Si un autre héros était à cette position, cet autre héro meurt.

##### Fin de match
Le match se termine après 300 tours. L’équipe qui possède le plus de pièces d’or remporte la partie.

## Attention!
Après avoir reçu l’état de la carte, vous avez au maximum 1 seconde pour répondre. Sinon, votre AI sera considérée comme déconnectée pour le reste de la partie. Le héros restera dans le jeu (et peut encore gagner), mais il agira comme si vous lui aviez dit de rester sur place pour le reste de la partie.

### La carte!
L’état de la carte vous est envoyé au complet à chaque tour. Elle est sous forme de string, 2 caractères par case. Par exemple :
```
+----------------------------------------+
|######$-    $-############$-    $-######|
|######        ##        ##        ######|
|####[]    ####            ####    []####|
|##      ####  ##        ##  ####      ##|
|####            $-    $-            ####|
|##########  @1    ^^^^    @4  ##########|
|############  ####    ####  ############|
|$-##$-        ############        $-##$-|
|  $-      $-################$-      $-  |
|        #########  #############        |
|        ########################        |
|  $-      $-################$-      $-  |
|$-##$-        ############        $-##$-|
|############  ####    ####  ############|
|##########  @2    ^^^^    @3  ##########|
|####            $-    $-            ####|
|##      ####  ##        ##  ####      ##|
|####[]    ####            ####    []####|
|######        ##        ##        ######|
|######$-    $-############$-    $-######|
+----------------------------------------+
## = Bois impassable
@1 = Héro #1
[] = Taverne
$- = Mine
$1 = Mine appartenant au Héros #1
^^ = Pic-Pic
```
