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


CREATE TABLE servers (
    UUID	uuid	DEFAULT gen_random_uuid()	PRIMARY KEY,
    name	TEXT	UNIQUE	NOT NULL,
    IP TEXT NOT NULL,
    nb_players INTEGER DEFAULT 0 NOT NULL
);
INSERT INTO servers (name, IP) VALUES
    ('Server1', '192.10.10.1'),
    ('Server2', '192.10.10.2');


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


CREATE TABLE ranks (
    UUID	uuid	DEFAULT gen_random_uuid()	PRIMARY KEY,
    name	TEXT	UNIQUE	NOT NULL
);
INSERT INTO ranks (name) VALUES
	('Copper'),
    ('Bronze'),
	('Silver'),
	('Gold'),
	('Platinium'),
	('Diamond');


CREATE TABLE player_stats (
    user_uuid	uuid	REFERENCES users(UUID)  PRIMARY KEY,
    rank_uuid	uuid	REFERENCES ranks(UUID)  NOT NULL,
    kill	INTEGER DEFAULT 0	NOT NULL,
    death	INTEGER DEFAULT 0   NOT NULL
);
INSERT INTO player_stats (user_uuid, kill, death, rank_uuid) VALUES
    ((SELECT UUID FROM users WHERE username = 'Julien'), 0, 0, (SELECT UUID FROM ranks WHERE name = 'Diamond')),
	((SELECT UUID FROM users WHERE username = 'Samuel'), -1, 2, (SELECT UUID FROM ranks WHERE name = 'Bronze')),
	((SELECT UUID FROM users WHERE username = 'Thomas'), 15, 20, (SELECT UUID FROM ranks WHERE name = 'Silver')),
	((SELECT UUID FROM users WHERE username = 'Virginie'), 10, 10, (SELECT UUID FROM ranks WHERE name = 'Gold')),
	((SELECT UUID FROM users WHERE username = 'Mathieu'), -5, -10, (SELECT UUID FROM ranks WHERE name = 'Platinium')),
	((SELECT UUID FROM users WHERE username = 'Sebastien'), 500000, 0, (SELECT UUID FROM ranks WHERE name = 'Copper'));



CREATE TABLE player_state (
    user_uuid	uuid	REFERENCES users(UUID)  PRIMARY KEY,
    --player_type TEXT    CHECK (player_type IN ('Server', 'Client')),
    is_in_game  boolean DEFAULT FALSE   NOT NULL, 
    map_name    TEXT,
    server_uuid uuid    REFERENCES servers(UUID),
    friends uuid[],
    CONSTRAINT valid_in_game CHECK (
        (is_in_game = FALSE AND map_name IS NULL AND server_uuid IS NULL) OR
        (is_in_game = TRUE AND map_name IS NOT NULL AND server_uuid IS NOT NULL)
    )
);
INSERT INTO player_state (user_uuid, is_in_game, map_name, server_uuid, friends) VALUES
    ((SELECT UUID FROM users WHERE username = 'Julien'),   
            TRUE, 'YuGiOh', (SELECT UUID FROM servers WHERE name = 'Server1'),  -- game
            NULL),  -- friends

    ((SELECT UUID FROM users WHERE username = 'Samuel'), 
            TRUE, 'Baldur', (SELECT UUID FROM servers WHERE name = 'Server1'),  -- game
            NULL),  -- friends

	((SELECT UUID FROM users WHERE username = 'Thomas'), 
            FALSE, NULL, NULL,  -- game
            NULL),  -- friends

	((SELECT UUID FROM users WHERE username = 'Virginie'), 
            FALSE, NULL, NULL,  -- game
            ARRAY[(SELECT UUID FROM users WHERE username = 'Julien')]), -- friends

	((SELECT UUID FROM users WHERE username = 'Mathieu'), 
            TRUE, 'Noita', (SELECT UUID FROM servers WHERE name = 'Server2'),   -- game
            ARRAY[(SELECT UUID FROM users WHERE username = 'Samuel'), (SELECT UUID FROM users WHERE username = 'Sebastien')]), -- friends

	((SELECT UUID FROM users WHERE username = 'Sebastien'), 
            TRUE, 'Magic', (SELECT UUID FROM servers WHERE name = 'Server2'),   -- game
            NULL);  -- friends


--UPDATE player_state
--SET friends = array_append(friends, (SELECT UUID FROM users WHERE username = 'Julien'))
--WHERE user_uuid = (SELECT UUID FROM users WHERE username = 'Sebastien')


