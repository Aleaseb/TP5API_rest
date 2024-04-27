CREATE TABLE users (
	UUID	uuid	DEFAULT gen_random_uuid(),
	username	TEXT	UNIQUE	NOT NULL,
	email	TEXT	UNIQUE	NOT NULL,
	password	TEXT	NOT NULL,
	salt	TEXT	NOT NULL,
	PRIMARY KEY(UUID)
);
INSERT INTO users (username, email, password, salt) VALUES
    ('Samuel', 'Samuelgmail', 'passe-muraille', 'sel'),
    ('Julien', 'Julienyahooooo', 'passe-partout', 'sucre'),
    ('Sebastien', 'Sebastienorange', 'passe_de_combat', 'poivre'),
    ('Mathieu', 'Mathieujsp', 'passe temps', 'epice'),
    ('Thomas', 'Thomasdiscord', 'pas touche', 'cuivre'),
    ('Virginie', 'Virginiefacebook', 'passe miroir', 'golden nugget');

CREATE TABLE success (
    UUID	uuid	DEFAULT gen_random_uuid()	PRIMARY KEY,
    name	TEXT	UNIQUE	NOT NULL,
    description	TEXT	NOT NULL,
    image	TEXT	NOT NULL
);
INSERT INTO success (name, description, image) VALUES
    ('100-kill', 'tuer 100 personnes', 'png'),
    ('100-days-login', 'jouer 100 jours à la suite', 'gif'),
    ('1$', 'dépenser 1$', 'jpg');

CREATE TABLE rank (
    UUID	uuid	DEFAULT gen_random_uuid()	PRIMARY KEY,
    name	TEXT	UNIQUE	NOT NULL
);
INSERT INTO rank (name) VALUES
	('Copper'),
    ('Bronze'),
	('Silver'),
	('Gold'),
	('Platinium'),
	('Diamond');

CREATE TABLE player_stats (
    /*UUID	SERIAL	PRIMARY KEY,*/
    user_uuid	uuid	REFERENCES users(UUID)  PRIMARY KEY,
    rank_uuid	uuid	REFERENCES rank(UUID),
    kill	INTEGER	NOT NULL,
    death	INTEGER	NOT NULL
);
INSERT INTO player_stats (user_uuid, kill, death, rank_uuid) VALUES
    ((SELECT UUID FROM users WHERE username = 'Julien'), 0, 0, (SELECT UUID FROM rank WHERE name = 'Diamond')),
	((SELECT UUID FROM users WHERE username = 'Samuel'), -1, 2, (SELECT UUID FROM rank WHERE name = 'Bronze')),
	((SELECT UUID FROM users WHERE username = 'Thomas'), 15, 20, (SELECT UUID FROM rank WHERE name = 'Silver')),
	((SELECT UUID FROM users WHERE username = 'Virginie'), 10, 10, (SELECT UUID FROM rank WHERE name = 'Gold')),
	((SELECT UUID FROM users WHERE username = 'Mathieu'), -5, -10, (SELECT UUID FROM rank WHERE name = 'Platinium')),
	((SELECT UUID FROM users WHERE username = 'Sebastien'), 500000, 0, (SELECT UUID FROM rank WHERE name = 'Copper'));
	