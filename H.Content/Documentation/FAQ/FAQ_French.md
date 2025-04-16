<p align="center">
<img src="../../Images/logo.png" alt="Holos Logo" width="550" />
<br>
</p>

# Foire aux questions

Le présent document a été créé pour que les utilisateurs puissent trouver toutes les réponses à leurs questions au même endroit.

<br>

# FAQ générales

### 1. Je ne sais pas comment utiliser le programme Holos.
Un guide de formation est disponible. En suivant ce guide étape par étape, vous apprendrez à utiliser le programme. Les liens à suivre sont les suivants:

<a href="https://github.com/holos-aafc/Holos/blob/main/H.Content/Documentation/Training/Holos_4_Training_Guide-fr.md">Guide de formation [FRA]</a>

<a href="https://github.com/holos-aafc/Holos/blob/main/H.Content/Documentation/Training/Holos_4_Training_Guide.md">Guide de formation [ANG]</a>

### 2. Y a-t-il des vidéos que je peux regarder?
Oui, un canal YouTube Holos offre des tutoriels destinés aux nouveaux utilisateurs. Cliquez sur ce lien:&nbsp;&nbsp;<a href="https://www.youtube.com/channel/UCHDORmZ73VICHzqm_yVpM_Q">Tutoriels vidéo</a>


### 3. Comment puis-je participer au forum de discussion Holos?
Holos a un babillard dans lequel les utilisateurs peuvent laisser leurs commentaires et poser des questions. Pour utiliser le forum de discussion, il vous faut d’abord créer un compte GitHub. Lorsque vous aurez créé votre compte GitHub, vous pourrez commencer à clavarder dans le forum. Il existe des guides étape par étape sur la façon de créer un compte GitHub et de créer une publication simple dans le forum de discussion:       

<a href="https://github.com/holos-aafc/Holos/discussions">Forum de discussion Holos</a>

<a href="https://github.com/holos-aafc/Holos/blob/main/H.Content/Documentation/GitHub%20Guide/GitHub%20Guide.md#creating-an-account">Comment créer un compte GitHub</a>

<a href="https://github.com/holos-aafc/Holos/blob/main/H.Content/Documentation/GitHub%20Guide/GitHub%20Guide.md#how-to-write-a-post-in-the-discussion-forum">Comment créer un message simple dans le forum de discussion</a>


### 4.  Comment puis-je ajouter ou modifier des entrées dans la présente FAQ?
Les utilisateurs peuvent ajouter de nouvelles entrées à la FAQ et modifier des entrées existantes. Veuillez noter que cette page utilise Markdown. Pour ajouter une entrée ou modifier une entrée dans la FAQ, vous procédez en deux étapes: 1. Créez un compte GitHub 2. Créez une demande de retrait dans le référentiel Holos. Une fois votre demande approuvée par l’administrateur du référentiel, les modifications que vous avez apportées sont appliquées à la page FAQ.

<a href="https://github.com/holos-aafc/Holos/blob/main/H.Content/Documentation/GitHub%20Guide/GitHub%20Guide.md#creating-an-account">Comment créer un compte GitHub</a>

<a href="https://github.com/holos-aafc/Holos/blob/main/H.Content/Documentation/GitHub%20Guide/GitHub%20Guide.md#contributing-changes-to-the-original-repository">Comment faire une demande de retrait dans le référentiel Holos</a>

---

# FAQ sur la configuration du modèle

### 1. Pourquoi n’y a-t-il (actuellement) pas d’option pour les unités de mesure impériales?
Holos a été conçu pour accepter les intrants mesurés selon le système de mesure métrique et selon le système impérial. L’option impériale peut toutefois être temporairement désactivée lors du déploiement des mises à jour du modèle. L’équipe travaille fort pour s’assurer que l’option impériale redevienne disponible le plus rapidement possible.


### 2. L’emplacement de Ma ferme dans le polygone des Pédo-paysages du Canada (PPC) est-il important?
Lorsque vous localisez votre ferme dans un polygone PPC, l’emplacement précis de la ferme dans le polygone ne change rien aux types de sol par défaut qui apparaissent dans le côté droit de l’écran (c.-à-d. que, peu importe où la ferme est située dans le polygone, ce sont les mêmes options de données sur les sols qui apparaissent). L’emplacement précis de la ferme dans un polygone n’a pas non plus d’incidence sur les zones de rusticité par défaut. Toutefois, les données quotidiennes sur le climat utilisées par le modèle par défaut sont téléversées automatiquement depuis la NASA; ces données sont présentées dans une grille de 10 km et peuvent donc varier dans le polygone du PPC, selon l’emplacement précis de la ferme. Par conséquent, l’utilisateur devrait tout de même choisir l’emplacement de sa ferme avec la plus grande précision possible. Pour ce faire, il peut utiliser différentes vues (p. ex., la vue aérienne), qu’il peut sélectionner à l’aide de l’icône Œil, au bas de la carte, dans l’écran Emplacement de la ferme.


### 3. Lorsque je clique à droite pour sélectionner l’emplacement de Ma ferme sur la carte, j’obtiens un message d’erreur indiquant que Holos ne peut pas télécharger les données climatiques quotidiennes de la NASA. Que dois-je faire?
Le téléchargement des données climatiques quotidiennes à partir du site Web de la NASA nécessite une connexion Internet stable. À l’occasion, s’il y a un problème avec la connexion Internet de l’utilisateur du modèle ou avec le site Web de la NASA lui-même, les données ne sont pas téléchargées. Le modèle utilise dans ce cas un ensemble de données intégré de 30 ans (1981-2010) sur les normales climatiques, disponible à l’échelle des polygones des PPC.

### 4. Comment puis-je téléverser mes propres données climatiques?
Dans l’écran de sélection des cartes, une fois qu’un emplacement est choisi, un panneau d’information apparaît à droite de l’écran; il présente les types de sol disponibles pour le polygone sélectionné des Pédo-paysages du Canada (PPC). Ce panneau comporte deux autres onglets, dont un pour les données climatiques. Par défaut, lorsque l’utilisateur du modèle sélectionne l’emplacement de sa ferme, Holos télécharge automatiquement les données climatiques de la NASA (données météorologiques quotidiennes, grille de 10 km). Cependant, dans la page d’onglet Données climatiques, l’utilisateur a la possibilité de remplacer les données climatiques par défaut en téléchargeant ses propres données personnalisées (il s’agit généralement de données locales mesurées).

> *Il faut souligner qu’un ensemble de données climatiques complet pour toute la période de simulation est alors requis (c.-à-d. que l’ensemble ne comporte pas de période ni de données manquantes, car Holos ne vérifie pas son exhaustivité et tentera de travailler avec les données telles qu’elles sont fournies); les données doivent également respecter le format obligatoire (voir ici).*

### 5. Les données sur les sols par défaut pour l’emplacement choisi de Ma ferme ne correspondent pas exactement aux conditions de mon exploitation. Comment puis-je corriger la situation?
Après avoir choisi l’emplacement de votre ferme, sélectionnez le type de sol par défaut qui correspond le plus étroitement au type de sol ou aux caractéristiques du sol de votre ferme. Cliquez sur Suivant. Dans l’écran de sélection des composantes, sélectionnez les options de menu Paramètres > Valeurs par défaut de la ferme > Sol. Dans cette page d’onglet, vous pouvez modifier certaines des caractéristiques du type de sol pour qu’elles correspondent mieux aux conditions du sol dans votre exploitation.
>*Remarque : Vous ne pouvez pas modifier la catégorie fonctionnelle du sol ni le grand groupe. Lorsque vous avez terminé, cliquez sur Fermer; vos modifications sont alors automatiquement enregistrées.*


### 6. Puis-je sélectionner différents types de sol pour les différents champs de Ma ferme?
À l’heure actuelle, il n’est pas possible de sélectionner différents types de sol pour les différents champs d’une même ferme simulée. Une fois que le type de sol est sélectionné dans l’écran de sélection des composantes, les données sur les sols sont appliquées à tous les champs ajoutés à la ferme. Si votre ferme comporte différents types de sols, vous pourriez en tenir compte en définissant plusieurs fermes (une pour chaque type de sol), et en définissant pour chacune d’elles les champs appropriés. Vous pourrez ensuite additionner les extrants du modèle pour les différentes fermes afin d’obtenir une estimation globale des émissions pour l’ensemble de l’exploitation


### 7. Que dois-je faire si Ma ferme s’étend sur plusieurs polygones PPC (Pédo-paysages du Canada)?
Actuellement, l’utilisateur du modèle doit situer sa ferme dans un unique polygone des PPC. Si l’utilisateur veut modéliser séparément les parties de sa ferme qui se trouvent dans différents polygones des PPC, il peut suivre la méthode décrite plus haut dans la réponse à la question 6. 


### 8. Une fois dans l’écran Détails ou dans l’écran Résultats, puis-je retourner aux écrans précédents pour apporter des modifications additionnelles à Ma ferme?
Oui. Si vous avez déjà configuré votre ferme et que vous êtes passé à l’écran Détails ou à l’écran Résultats, vous pouvez retourner aux écrans précédents et apporter toutes les modifications additionnelles voulues. Cependant, si vous avez une configuration pluriannuelle, après avoir apporté vos modifications, vous devrez accéder à l’écran Détails et cliquer sur le bouton Recharger les données de l’écran précédent, dans le coin supérieur gauche de l’écran. Holos écrasera ainsi l’ensemble précédent de données d’entrée. Veuillez noter qu’après cette opération, vous devrez apporter de nouveau toutes les modifications précédentes aux données de l’écran Détails (données personnalisées sur le rendement, etc.).

---

# FAQ sur les cultures 

### 1. Où se trouve la culture X, je ne la trouve pas dans la liste?
Si une culture ne figure pas dans la liste, cela signifie que nous n’avons pas pu trouver de données canadiennes publiées sur celle-ci. Il y a alors trois façons de corriger le problème (si vous avez les données nécessaires):
- Cas unique: Choisissez une culture figurant dans la liste et ajustez les valeurs du coefficient de carbone (C), qui décrivent la proportion de biomasse végétale totale ou de carbone contenue dans les différentes parties de la plante, ainsi que leurs concentrations en azote (N) et leurs teneurs en lignine; cela peut être fait dans la page d’onglet Résidus pour le champ ou la rotation des cultures en question;
- Réglage continu: Dans l’écran de sélection des composantes, sélectionnez les options de menu Paramètres > Valeurs par défaut des cultures pour accéder à l’endroit où vous pouvez écraser les paramètres par défaut associés à chaque culture. Lorsque vous avez terminé, les nouveaux paramètres et les nouvelles valeurs par défaut s’affichent chaque fois que la culture est choisie;
- Ajout permanent : Ajoutez une nouvelle culture à la table de consultation Holos, avec les données connexes requises (c.-à-d. coefficients C, concentrations d’azote, teneurs en lignine, teneur en humidité). [(tableau 9)](https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_9_Default_Values_For_Nitrogen_Lignin_In_Crops.csv). 
>*À noter, pour les ajouts permanents de cultures, des publications évaluées par des pairs doivent être fournies à titre de référence; ces publications doivent faire état d’études canadiennes.*


### 2. Quelle est la différence entre la composante Champ et la composante Rotation des cultures?
Bref, la composante Champ établit une rotation/séquence des cultures pour un champ, tandis que la composante Rotation des cultures établit un certain nombre de champs pour une rotation des cultures (le nombre de champs = le nombre de cultures dans la rotation). Cela mis à part, les deux composantes ont la même fonction, mais pour la composante Rotation des cultures, tous les intrants particuliers sont copiés dans tous les champs de la rotation. Si un champ est géré d’une façon nettement différente de celle des autres, il faudrait plutôt créer une composante de champ pour cet unique champ.


### 3. Comment l’utilisateur du modèle peut-il composer avec différentes configurations de champs?
À l’heure actuelle, il n’y a aucune façon de traiter automatiquement ces différentes configurations dans Holos, mais l’utilisateur du modèle peut contourner la situation en créant différentes composantes de champ pour différentes périodes, avec les pratiques de gestion appropriées pour chaque champ et chaque période.


### 4.Mes champs changent de taille au fil du temps. Comment puis-je refléter cela dans Holos?
Comme c’est le cas pour les configurations de champs modifiées au fil du temps, les tailles de champ modifiées au fil du temps ne peuvent pas être traitées automatiquement dans Holos. Si la taille du champ change au cours de la période visée par la simulation, l’utilisateur peut résoudre ce problème en subdivisant le champ en composantes dont la taille ne changera pas durant toute la période de simulation, et qui auront chacune leur propre historique. Il faut procéder de cette façon parce que, dans Holos, les modèles de carbone estiment la variation du carbone en fonction de la superficie des terres, et ils supposent que cette superficie ne sera jamais modifiée. 


### 5. Comment puis-je déterminer la superficie nécessaire pour produire suffisamment d’aliments pour les animaux de Ma ferme? Est-ce que Holos la calcule automatiquement?
Holos ne calcule pas automatiquement la superficie de chaque culture nécessaire pour nourrir les animaux dans la ferme simulée. L’utilisateur peut calculer cette superficie à l’extérieur de Holos à l’aide des données sur la quantité requise de chaque type d’aliments, du rendement des cultures et des pertes de récolte ou d’aliments, etc., ou alors il peut entrer la superficie des champs réels utilisés pour cultiver des aliments pour les animaux inclus dans la ferme (p. ex., si la simulation est fondée sur une exploitation agricole réelle).

### 6. J’applique un engrais qui ne figure pas dans la liste déroulante. Que puis-je faire?
La page d’onglet Engrais de la composante du champ ou de la rotation des cultures offre la possibilité d’ajouter des engrais synthétiques ou biologiques personnalisés. Le choix de l’engrais aura une incidence sur la fraction d’oxyde nitreux (N2O) émise. Au moment de choisir le type d’engrais personnalisé à ajouter, nous recommandons que les engrais contenant une grande quantité d’azote réactif soient utilisés comme engrais synthétique personnalisé; pour les engrais dans lesquels l’azote est lié à la biomasse et libéré par décomposition (c.-à-d. le compost), c’est l’option biologique personnalisée qui est appropriée. Les engrais à libération lente sont toujours considérés comme étant synthétiques, car ils libèrent de l’azote réactif au fil du temps. Il faut mesurer le N/C/P et la teneur en humidité de l’engrais personnalisé.


### 7. J’importe du fumier et des engrais organiques à la ferme. Les émissions figurant dans les rapports Holos ne se rapportent-elles pas à la source ou à l’origine de ces engrais?
Lorsqu’il importe du fumier ou des engrais organiques d’autres exploitations, l’agriculteur profite de l’ajout de carbone dans le sol. L’application de ces matières organiques entraînera toutefois des émissions. Comme le choix des matières appliquées revient à l’agriculteur et que les émissions se produisent depuis la ferme les ayant importées/appliquées, Holos déclare les émissions connexes (aux fins du budget des GES pour l’ensemble de la ferme). Si l’objectif de la simulation est une approche d’analyse du cycle de vie (pour calculer l’efficacité et l’intensité des GES d’un produit), il pourrait être nécessaire de recourir à la répartition des émissions.


### 8. Comment puis-je ajouter un champ dans lequel je cultive des mélanges de cultures de couverture?
À l’heure actuelle, Holos n’offre pas la possibilité d’entrer des mélanges de cultures de couverture. Le développement et l’ajout de cette fonctionnalité sont toutefois prévus. À l’heure actuelle, une seule culture de couverture peut être sélectionnée dans la liste des options disponibles, et les coefficients de carbone et les concentrations d’azote peuvent être ajustés pour mieux refléter le mélange voulu, par exemple à l’aide de valeurs moyennes pour le mélange.


### 9. Comment puis-je ajouter un champ avec des cultures intercalaires?
À l’heure actuelle, Holos n’offre pas la possibilité d’entrer des systèmes de cultures intercalaires. Le développement et l’ajout de cette fonctionnalité sont toutefois prévus. Entretemps, on peut utiliser des composantes de champ distinctes pour les différentes cultures. 

### 10. Comment puis-je explorer les effets de la gestion des nutriments 4B dans Holos?
Nous suivons la méthode employée pour les inventaires nationaux de GES et nous ne savons pas encore comment tenir compte des effets de la gestion des nutriments 4B. Nous avons cependant des facteurs préliminaires pour certaines pratiques; vous pouvez les voir en sélectionnant l’option « Oui » à côté de la mention Afficher les renseignements additionnels, dans la page d’onglet Engrais, puis en sélectionnant l’additif voulu. La possibilité de définir un additif personnalisé a été ajoutée au modèle pour permettre la mise à l’essai des résultats préliminaires d’autres applications.

### 11. Lorsque j’ajoute une culture d’automne/hiver à Holos, quelle culture dois-je préciser pour ce champ au printemps de l’année suivante au moment de sa récolte?
Actuellement, dans Holos V4, lorsque lors de la croissance d’une culture comme celle du seigle d’automne ou du blé d’hiver (par l’entremise de la section sur les cultures d’hiver/de couverture), les résidus de la culture d’hiver/de couverture sont ajoutés à la culture principale durant l’année de la plantation (année t); c’est la méthode par défaut. S’il n’y a pas de récolte de la culture d’hiver au cours de l’année t, l’utilisateur du modèle doit entrer la valeur « 0 » comme rendement de la culture d’hiver durant cette année, puis ajouter un rendement non nul au cours de l’année t+1. L’utilisateur du modèle peut aussi omettre la culture d’hiver/de couverture au cours de l’année t, puis la préciser au cours de l’année t+1 avec un rendement non nul; l’effet sur les estimations Holos sera le même dans les deux cas.

---

# FAQ sur le carbone

### 1. À quoi servent les valeurs Année de début et Année de fin?
Les années de début et de fin déterminent la durée de la simulation du modèle de carbone dans le sol. Cela dépendra en grande partie des données antérieures disponibles sur les activités agricoles, mais nous recommandons à l’utilisateur de commencer la simulation le plus tôt possible à partir de l’année 1985. Cela parce que la variation du carbone est un processus à long terme, les pédologues affirmant fréquemment que les changements de carbone dans le sol liés à la mise en œuvre d’une pratique ou d’un ensemble de pratiques de gestion particulières peuvent être mesurés au plus tôt 10 ans après la mise en œuvre du changement de gestion. Lors de la définition de l’historique de gestion d’une composante de champ ou de rotation des cultures, Holos a principalement besoin de la séquence de culture et de données approximatives sur la gestion des engrais, du fumier et des résidus. Les données sur le rendement des cultures sont également importantes. Par défaut, Holos télécharge les données annuelles sur le rendement propre aux cultures à l’échelle de la région de données intraprovinciales d’une base de données, mais l’utilisateur peut annuler ces rendements par défaut dans la page d’onglet Général de l’écran de sélection des composantes ou dans l’écran Détails.


### 2. J’ai des données mesurées sur le carbone dans le sol; comment puis-je les entrer dans Holos?
Par défaut, Holos estime une valeur initiale de carbone organique dans le sol (kg COS ha -1) pour chaque champ. L’utilisateur peut toutefois annuler cette valeur en utilisant des données mesurées sur le carbone dans le sol pour lancer le modèle du carbone dans le sol. Pour ce faire, dans l’écran de sélection des composantes, sélectionnez les options de menu Paramètres > Paramètres de l’utilisateur. Activez le champ Utiliser la valeur personnalisée de carbone à l’équilibre, puis entrez la valeur voulue dans la case Carbone à l’équilibre; l’unité est exprimée en kg C ha-1. Quand vous avez terminé, cliquez sur OK; vos modifications sont alors automatiquement enregistrées. À l’heure actuelle, cette valeur de départ personnalisée s’applique à l’ensemble de la ferme (c.-à-d. à tous les champs et à toutes les rotations de cultures simulés). 


### 3. Holos produit de multiples résultats dans la page d’onglet Modélisation pluriannuelle du carbone; lequel dois-je utiliser?
Les modèles du carbone comportant des étapes annuelles (comme ceux utilisés par Holos) fournissent une estimation du stock total de carbone dans le sol pour chaque année de la simulation (kg C ha-1). Comme ces estimations annuelles représentent la somme du stock de COS de l’année précédente, plus les apports de C des résidus de cultures et du fumier, moins les pertes dues à la décomposition pour l’année en cours, les niveaux des stocks de COS peuvent varier d’une année à l’autre, ainsi que sur de plus longues périodes (p. ex., à cause d’un changement de système cultural), et ainsi entraîner des des variations estimées du carbone nettes positives ou négatives, selon la fenêtre temporelle évaluée. À titre de développeurs du modèle, nous ne pouvons pas prévoir les modifications qui se produiront à la ferme ni le moment où elles se produiront; le modèle fournit des estimations de la variation des stocks de COS sur plusieurs périodes différentes. L’utilisateur du modèle peut cependant calculer la variation dans les stocks de COS au cours d’autres périodes en utilisant les données déclarées sous forme de grille dans l’écran des résultats de la modélisation pluriannuelle du carbone, afin de représenter les changements et les tendances au cours de la période voulue.

---

# FAQ sur les animaux d’élevage 

### 1. Je ne trouve pas les options correspondant à mes systèmes de pâturage. Comment puis-je représenter un système de pâturage X dans Holos?
À l’heure actuelle, Holos n’offre pas la possibilité de simuler différents systèmes de pâturage, car les effets exacts de ces systèmes ne sont toujours pas clairs sur le plan scientifique. Il y a en outre une certaine confusion en ce qui concerne la terminologie. Notre équipe participe à plusieurs projets qui visent à clarifier la situation, et les futures mises à jour du modèle offriront les options appropriées. Pour l’instant, à l’aide de Holos, l’utilisateur du modèle peut placer des groupes d’animaux sur des champs de pâturage particuliers. De cette façon, on pourrait créer plusieurs champs pour représenter différents enclos dans un système de pâturage rotationnel, et détailler l’historique de gestion de chacun des champs. Cependant, dans le cas des champs et des enclos broutés, Holos estime la productivité de la biomasse aérienne en fonction de la consommation de biomasse de végétation par les animaux, combinée aux estimations de l’utilisation de la biomasse (efficacité du pâturage); par conséquent, des systèmes de pâturage plus efficaces pourraient être représentés au moyen d’un seul pâturage, plutôt que par un pâturage subdivisé en parcelles.
>*Remarque: Il a été démontré que les systèmes de pâturage plus intensifs améliorent la qualité des aliments, mais ces systèmes doivent être précisés pour chaque groupe d’animaux qui broutent dans un pâturage particulier et pour chaque période de gestion pertinente à l’aide de la page d’onglet Ration pour le groupe d’animaux et la période de gestion, p. ex., en créant une alimentation de pâturage personnalisée à l’aide de l’outil Créateur de ration personnalisée. 


### 2. Je veux comparer les options de gestion du bétail. Comment puis-je le faire dans Holos?
Il y a trois façons de procéder: 
- Définir deux fermes différentes. 
- Définir deux composantes pour le bétail dans une unique ferme. 
- Définir deux groupes d’animaux d’élevage au sein d’une unique composante du bétail dans une unique ferme. 
Chacune des options permet à l’utilisateur du modèle de comparer les résultats du modèle pour les différentes options de gestion.
>*Remarque: Si l’utilisateur du modèle définit deux (ou plusieurs) fermes différentes, il peut comparer les résultats du modèle pour ces fermes en sélectionnant l’option « Oui » à côté de la mention Année de fin, dans l’écran Résultats, et en sélectionnant les fermes qu’il souhaite comparer dans la liste des fermes disponibles.*


### 3. Je veux connaître l’empreinte carbone de mon système d’élevage; que dois-je faire?
Le modèle Holos est conçu pour calculer le budget de gaz à effet de serre (GES) d’une ferme, ce qui signifie qu’il tient compte de toutes les sources de GES fondées sur la ferme que nous pouvons estimer en fonction des données disponibles. Pour calculer l’empreinte carbone d’un produit, nous devons tenir compte de toutes les émissions résultant de la production de ce produit. Dans le cas d’un système d’élevage, cela signifie qu’il faut tenir compte de la production d’aliments pour animaux, que ces aliments soient cultivés sur place ou non. Avant d’ajouter des champs de production d’aliments pour animaux à la ferme simulée, l’utilisateur doit d’abord calculer la superficie de chaque pâturage ou champ de culture nécessaire pour soutenir les animaux à la ferme. Holos produira un message d’avertissement s’il n’y a pas suffisamment d’aliments du bétail cultivés pour répondre aux besoins des animaux, à titre de vérification interne. Les émissions générées par les intrants du système de production d’aliments pour animaux (p. ex., pour la production d’engrais et de pesticides) sont également prises en compte. Dans Holos, les émissions en amont de ces intrants agricoles sont également rapportées, c.-à-d. le CO2 issu de la production en amont de matières synthétiques. Pour le système d’élevage lui-même, les émissions liées aux animaux reproducteurs doivent être incluses dans les calculs, ainsi que celles liées à leur progéniture. Holos calcule ensuite toutes les émissions de ce système jusqu’à la porte de la ferme. Toutes les émissions liée au transport, à la transformation, etc. devront être estimées par l’utilisateur à l’extérieur de Holos, puis ajoutées aux extrants de Holos, le cas échéant. 
>*Remarque: Il revient à l’utilisateur d’attribuer les émissions en fonction du produit; p. ex., dans un système de production de bœuf, les extrants pourraient facilement être répartis en éq. CO2 par carcasse d’animal, mais pour une quantité d’éq. CO2 par kg de viande, il faut décider si toutes les émissions sont attribuées à la partie « viande » de la carcasse ou si une portion des émissions est attribuée aux différentes parties de la carcasse (la consultation d’un expert en ACV est conseillée).*


### 4. Je veux ajouter à ma ration personnalisée un ingrédient qui ne figure pas dans la liste des ingrédients. Comment puis-je le faire?
À l’aide du Créateur de ration personnalisée, l’utilisateur du modèle peut créer de nouveaux ingrédients alimentaires qui pourront ensuite être ajoutés à une ration personnalisée. Ouvrez le Créateur de ration personnalisée dans la page d’onglet Ration et, à l’étape 2, cliquez sur Créer un ingrédient personnalisé. Une nouvelle ligne devrait apparaître en haut de la liste des ingrédients. Vous pouvez cliquer sur le nom de l’ingrédient pour le modifier. Pour définir cet ingrédient, vous devrez entrer les données pertinentes dans le reste de la ligne.
>*Remarque: les colonnes de données de ce tableau ne sont pas toutes nécessaires aux calculs de Holos, et les données requises varient selon le groupe d’animaux considéré.* 

Au minimum, les données suivantes sont requises pour différents groupes d’animaux:

 - **Bovins laitiers et de boucherie**: MS (teneur en matière sèche de l’ingrédient en l’état, % EE), fourrage (% MS; cette valeur sera soit « 0 » si l’ingrédient personnalisé n’est pas un ingrédient fourrager, soit « 100 » s’il s’agit d’un ingrédient fourrager), PB (teneur en protéines brutes, % de MS), MDT (matières digestibles totales, % de MS), amidon (concentration d’amidon dans l’ingrédient, % de MS), cendres (teneur en cendres de l’aliment, % de MS), ÉNe (énergie nette d’entretien, Mcal kg -1), et ENc (énergie nette de croissance, Mcal kg -1). Ces deux derniers paramètres ne sont nécessaires que pour l’estimation des émissions de méthane dans le cas des veaux non soumis à une ration lactée; pour *le mouton, le porc, la volaille et les autres animaux d’élevage**: MS (% EE), fourrage (% MS), PB (% MS), MDT (% MS) et cendres (% MS).

Quand vous avez terminé, cliquez sur **OK**; vos modifications sont alors automatiquement enregistrées.


### 5. Où puis-je voir tous les détails des rations par défaut intégrées à Holos?
Certaines des données pour la ration sélectionnée sont visibles lorsque vous sélectionnez l’option Afficher les renseignements additionnels, dans la page d’onglet Ration, mais vous pouvez voir tous les détails de la ration si vous ouvrez le Créateur de ration personnalisée. Une fois cet outil ouvert, sélectionnez l’option « Oui » à côté de la mention Afficher les rations par défaut, à l’étape 1. Vous voyez maintenant dans cette section des données sur le contenu nutritionnel de chaque ration par défaut disponible pour le type d’élevage concerné, ainsi que des données liées au pourcentage de MS alimentaire totale composée des différents ingrédients de la ration (à l’étape 3).

### 6. Comment puis-je modifier l’une des rations par défaut dans Holos ou créer une nouvelle ration en fonction d’une ration par défaut?
Pour modifier l’une des rations par défaut dans Holos pour un groupe d’animaux et une période de gestion particuliers, faites une copie de la ration par défaut, puis modifiez les données entrées dans la copie. Pour faire une copie d’une ration par défaut, ouvrez le Créateur de ration personnalisée et, à l’étape 1, cliquez sur Oui, à côté de la mention « Afficher les rations par défaut ». Dans la liste des rations par défaut qui s’affiche, cliquez à droite sur la ration voulue, puis cliquez sur Créer une copie. Vous pouvez changer le nom de la ration copiée, au besoin. À l’étape 3, vous pouvez modifier le pourcentage des différents ingrédients de la ration ou supprimer des ingrédients précis, et à l’étape 2, vous pouvez ajouter des ingrédients. Lorsque vous avez fini de modifier la ration, cliquez sur OK pour revenir à la page d’onglet principale Ration et sélectionnez la ration copiée dans le menu déroulant Ration.

>*Remarque : Si l’utilisateur choisit la même ration (soit la ration originale par défaut ou une copie de celle-ci) pour plus d’une période de gestion, toute modification apportée à la ration au cours d’une période de gestion sera automatiquement reproduite pour les autres périodes de gestion utilisant la même ration. Par conséquent, si l’utilisateur doit apporter des modifications différentes à une ration utilisée pour plusieurs groupes d’animaux ou périodes de gestion, il est recommandé qu’une copie soit créée pour chaque période de gestion, puis modifiée en conséquence. L’utilisateur peut également créer une ration entièrement nouvelle pour chaque période de gestion à l’aide du Créateur de ration personnalisée.*

---

Pour télécharger Holos, obtenir de plus amples renseignements ou accéder à une liste récente des publications concernant Holos, accédez à la page que vous trouverez à l’adresse suivante:
https://agriculture.canada.ca/en/agricultural-science-and-innovation/agricultural-research-results/holos-software-program

Pour nous joindre, envoyez un courriel à:
aafc.holos.acc@canada.ca