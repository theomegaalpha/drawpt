-- SQL Server init script

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'drawpt')
BEGIN
  CREATE DATABASE drawpt;
END;
GO

USE drawpt;
GO

IF OBJECT_ID(N'Adjectives', N'U') IS NULL
BEGIN
    CREATE TABLE [ref].[Adjectives]
    (
        [Adjective] NVARCHAR(255) NOT NULL PRIMARY KEY
    )
END;
GO

IF (SELECT COUNT(*) FROM [ref].[Adjectives]) = 0
BEGIN
    INSERT INTO [ref].[Adjectives] (Adjective) VALUES
    ('happy'), ('sad'), ('angry'), ('excited'), ('bored'),
    ('hungry'), ('thirsty'), ('sleepy'), ('energetic'), ('lazy'),
    ('brave'), ('cowardly'), ('curious'), ('shy'), ('confident'),
    ('nervous'), ('proud'), ('ashamed'), ('grumpy'), ('cheerful'),
    ('friendly'), ('unfriendly'), ('polite'), ('rude'), ('kind'),
    ('mean'), ('generous'), ('selfish'), ('honest'), ('dishonest'),
    ('loyal'), ('disloyal'), ('optimistic'), ('pessimistic'), ('creative'),
    ('unimaginative'), ('adventurous'), ('cautious'), ('ambitious'),
    ('patient'), ('impatient'), ('calm'), ('anxious'), ('organized'),
    ('disorganized'), ('neat'), ('messy'), ('tidy'), ('untidy'),
    ('hardworking'), ('intelligent'), ('unintelligent'), ('wise'),
    ('strong'), ('weak'), ('fast'), ('slow'), ('tall'), ('unambitious'),
    ('short'), ('big'), ('small'), ('young'), ('old'), ('foolish'),
    ('rich'), ('poor'), ('healthy'), ('sick'), ('clean'),
    ('dirty'), ('beautiful'), ('ugly'), ('handsome'), ('plain'),
    ('funny'), ('serious'), ('silly'), ('sensible'), ('noisy'),
    ('quiet'), ('loud'), ('bright'), ('dull'), ('colorful'),
    ('colorless'), ('warm'), ('cold'), ('hot'), ('cool'),
    ('wet'), ('dry'), ('smooth'), ('rough'), ('hard'),
    ('soft'), ('heavy'), ('light'), ('thick'), ('thin'),
    ('wide'), ('narrow');
END;
GO


IF OBJECT_ID(N'Nouns', N'U') IS NULL
BEGIN
    CREATE TABLE [ref].[Nouns]
    (
        [Noun] NVARCHAR(255) NOT NULL PRIMARY KEY
    )
END;
GO

IF (SELECT COUNT(*) FROM [ref].[Nouns]) = 0
BEGIN 
    INSERT INTO [ref].[Nouns] (Noun) VALUES
    ('alligator'), ('ant'), ('bear'), ('bee'), ('bird'),
    ('camel'), ('cat'), ('cheetah'), ('chicken'), ('chimpanzee'),
    ('cow'), ('crocodile'), ('deer'), ('dog'), ('dolphin'),
    ('duck'), ('eagle'), ('elephant'), ('fish'), ('fly'),
    ('fox'), ('frog'), ('giraffe'), ('goat'), ('goldfish'),
    ('hamster'), ('hippopotamus'), ('horse'), ('kangaroo'), ('kitten'),
    ('lion'), ('lobster'), ('monkey'), ('octopus'), ('owl'),
    ('panda'), ('pig'), ('puppy'), ('rabbit'), ('rat'),
    ('scorpion'), ('seal'), ('shark'), ('sheep'), ('snail'),
    ('snake'), ('spider'), ('squirrel'), ('tiger'), ('turtle'),
    ('wolf'), ('zebra'), ('bat'), ('beetle'), ('buffalo'),
    ('butterfly'), ('crab'), ('crow'), ('dove'), ('dragonfly'),
    ('eagle'), ('falcon'), ('flamingo'), ('grasshopper'), ('hawk'),
    ('hummingbird'), ('jellyfish'), ('koala'), ('ladybug'), ('lemur'),
    ('leopard'), ('lynx'), ('mole'), ('mosquito'), ('ostrich'),
    ('otter'), ('parrot'), ('peacock'), ('pelican'), ('penguin'),
    ('pigeon'), ('porcupine'), ('raccoon'), ('raven'), ('robin'),
    ('seagull'), ('sparrow'), ('swan'), ('toucan'), ('vulture'),
    ('walrus'), ('weasel'), ('whale'), ('woodpecker'), ('yak'),
    ('zebu');
END;