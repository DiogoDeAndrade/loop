# LOOP

This project is for my [Game Makers ToolKit Game Jam'25] entry.

## Gameplay

This is some preliminary ideas to get started:
* Action, top-down, twin-stick game
* Inspired by Minnit, without the time limit
* Player 
  * controls a hero that wakes up at the start of the day
  * Starts with a sword and a shield
  * Can swipe, charge attack, block, parry and dodge from the start  
  * Has skill tree - improves skills/attacks/stats
    * Souls can be used to buy/upgrade skills
    * Can only upgrade on the start area
  * When player dies, he drops the souls he's carrying, and he starts the loop
    * All enemies respawn (limited respawn on some enemies?)
    * All items respawn
  * Player can recover souls, but it also gets back all the debuffs he was carrying at the time
  * Buffs/Debuffs can only be clear at specific altars in exchange of souls
  * Player can get buffs from different sources on the map
  * Can find wands for magic
    * Wands spend either spend mana, or have limited charges
    * Player has inventory to choose what wand to use
    * Can carry as many wands as he wants
* Day and night cycle
  * At night it gets dark
  * Player has a small light radius (can be improved on skills)
  * Drain something over time?
* Enemies
  * Enemies get stronger at night
  * The further from the center, the stronger enemies are
  * When killed, they can drop health pellets, mana pellets or "souls"
    * The more souls you have, the more souls you get
  * Enemies can apply debufs
* Objective is to reach the big baddy and destroy him

## Abilities

* Theoretically this can be used by player and enemies
* Swipe with weapon (melee attack) - don't forget telegraph
  * W/ Charge Attack
* Block/Parry - just difference in timmings
* Dodge
* Fire projectiles (i.e. fireball, acidbolt, etc)
* AOE projectiles (i.e. rain of meteors) - don't forget telegraph
* Channeled projectiles (i.e. magic beam)
* Channeled spells (i.e. heal) - can be thought as a charged attack
* Temporary invulnerability
* Rush charge - don't forget telegraph 

## Tasks

* Knife ability
  * Attack
* Add basic enemy
  * Take damage
  * Get killed
  * Attack player
  * Pathfinding (this is the hard one)
  * Apply debuffs
* Block/Parry
* Dodge
* Charge attack


## Art

- Font "Alagard" by [Hewett Tsoi]
- Palette "Retrocal" by [Polyphrog](https://lospec.com/poly-phrog)
- 1-Bit Pack by [KenneyNL], licensed under [CC0]
- Sci-Fi UI Pack by [KenneyNL], licensed under [CC0]
- Everything else done by [Diogo de Andrade], licensed through the [CC0] license.

## Sound

- Everything else done by [Diogo de Andrade], licensed through the [CC0] license.

## Code

- Some code was adapted/refactored from [Okapi Kit], licensed under the [MIT] license.
- Uses [Naughty Attributes], licensed under the [MIT] license.
- Uses [Unity Common], licensed under the [MIT] license.
- All remaining game source code by Diogo de Andrade is licensed under the [MIT] license.

## Metadata

- Autor: [Diogo de Andrade]

[Diogo de Andrade]:https://github.com/DiogoDeAndrade
[Game Makers ToolKit Game Jam'25]:https://itch.io/jam/gmtk-2025
[CC0]:https://creativecommons.org/publicdomain/zero/1.0/
[Naughty Attributes]:https://github.com/dbrizov/NaughtyAttributes
[Unity Common]:https://github.com/DiogoDeAndrade/UnityCommon
[Hewett Tsoi]:https://www.dafont.com/pt/profile.php?user=698002
[KenneyNL]:https://kenney.nl/
[CC-BY 3.0]:https://creativecommons.org/licenses/by/3.0/
[CC-BY-SA 4.0]:http://creativecommons.org/licenses/by-sa/4.0/
[CC-BY 4.0]:https://creativecommons.org/licenses/by/4.0/
[MIT]:LICENSE
