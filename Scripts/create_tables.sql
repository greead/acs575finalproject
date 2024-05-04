--
-- ENUMS
-- ENUM for Account table status column
DROP TYPE IF EXISTS account_status_enum CASCADE;
CREATE TYPE account_status_enum AS ENUM ('ONLINE', 'OFFLINE', 'BANNED', 'INACTIVE');
--
-- ENUM for Server table status column
DROP TYPE IF EXISTS server_status_enum CASCADE;
CREATE TYPE server_status_enum AS ENUM ('UP', 'DOWN');
--
-- ENUM for Item type rarity column
DROP TYPE IF EXISTS item_rarity_enum CASCADE;
CREATE TYPE item_rarity_enum AS ENUM ('Junk', 'Common', 'Uncommon', 'Rare', 'Mythic');
--
-- ENUM for Weapon handedness column
DROP TYPE IF EXISTS weapon_hands_enum CASCADE;
CREATE TYPE weapon_hands_enum AS ENUM ('Main-Hand', 'Off-Hand', 'Two-Hand', 'One-Hand');
--
-- ENUM for Weapon type column
DROP TYPE IF EXISTS weapon_type_enum CASCADE;
CREATE TYPE weapon_type_enum AS ENUM ('Sword', 'Axe', 'Spear', 'Hammer', 'Shield');
--
-- ENUM for Armor slot column
DROP TYPE IF EXISTS armor_slot_enum CASCADE;
CREATE TYPE armor_slot_enum AS ENUM ('Head', 'Body', 'Waist', 'Hands', 'Legs', 'Feet');
--
-- ENUM for Armor type column
DROP TYPE IF EXISTS armor_type_enum CASCADE;
CREATE TYPE armor_type_enum AS ENUM ('Light', 'Medium', 'Heavy');

--
-- SEQUENCES
--
-- SEQUENCE for Item id column
DROP SEQUENCE IF EXISTS item_id_seq CASCADE;
CREATE SEQUENCE item_id_seq;

--
-- TYPES
--
-- TYPE for the Item table
DROP TYPE IF EXISTS item_type CASCADE;
CREATE TYPE item_type AS (
	id 				INT,
	name 			TEXT,
	rarity 			item_rarity_enum,
	description 	TEXT,
	cost 			INT,
	is_stackable 	BOOLEAN,
	icon 			BYTEA
);

-- TYPE to represent an item stack in inventory 
DROP TYPE IF EXISTS item_stack_type CASCADE;
CREATE TYPE item_stack_type AS (
	item 			item_type,
	stackSize 		INT
);

--
-- TABLES
--
-- TABLE to represent an item
DROP TABLE IF EXISTS item CASCADE;
CREATE TABLE item OF item_type (
	id PRIMARY KEY DEFAULT nextval('item_id_seq'),
	name NOT NULL
);

-- TABLE to represent equipment items
DROP TABLE IF EXISTS equipment CASCADE;
CREATE TABLE equipment (
	durability		INT,
	remDurability	INT CHECK (remDurability <= durability),
	repairCost		INT
) INHERITS (item);

-- TABLE to represent weapon equipment items
DROP TABLE IF EXISTS weapon CASCADE;
CREATE TABLE weapon (
	hands			weapon_hands_enum,
	type			weapon_type_enum,
	damage			INT
) INHERITS (equipment);

-- TABLE to represent armor equipment items
DROP TABLE IF EXISTS armor CASCADE;
CREATE TABLE armor (
	slot			armor_slot_enum,
	type			armor_type_enum,
	defense			INT
) INHERITS (equipment);

-- TABLE to represent quest items
DROP TABLE IF EXISTS quest_item CASCADE;
CREATE TABLE quest_item () INHERITS (item);

-- TABLE to represent junk items
DROP TABLE IF EXISTS junk_item CASCADE;
CREATE TABLE junk_item () INHERITS (item);

-- TABLE to represent usable items
DROP TABLE IF EXISTS usable_item CASCADE;
CREATE TABLE usable_item (
	uses			INT,
	remUses			INT CHECK (remUses <= uses),
	cooldown		INT,
	remCooldown		INT CHECK (remCooldown <= cooldown)
) INHERITS (item);

-- TABLE to represent ingredient items
DROP TABLE IF EXISTS ingredient CASCADE;
CREATE TABLE ingredient () INHERITS (item);

--
-- SPECIAL TYPE
--
-- TYPE to represent a character's equipped equipment
DROP TYPE IF EXISTS equipment_set_type CASCADE;
CREATE TYPE equipment_set_type AS (
	mainHand		weapon,
	offHand			weapon,
	head			armor,
	body			armor,
	waist			armor,
	hands			armor,
	legs			armor,
	feet			armor
);

--
-- TABLES (Cont.)
--
-- TABLE for storing Account status and login information
DROP TABLE IF EXISTS account CASCADE;
CREATE TABLE account (
	email 			TEXT 			PRIMARY KEY,
	password 		TEXT			NOT NULL,
	status 			account_status_enum
);

-- TABLE for storing Server status
DROP TABLE IF EXISTS server CASCADE;
CREATE TABLE server (
	id 				INT 			PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	name 			TEXT			NOT NULL,
	status 			server_status_enum
);

-- TABLE for storing Character information
DROP TABLE IF EXISTS character CASCADE;
CREATE TABLE character (
	server_id 		INT 			REFERENCES server ON DELETE CASCADE,
	name 			TEXT			NOT NULL,
	account_email 	TEXT			REFERENCES account ON DELETE CASCADE,
	inventory 		item_stack_type[],
	bank			item_stack_type[],
	equipped		equipment_set_type,
	money			INT,
	PRIMARY KEY (server_id, name)
);




















