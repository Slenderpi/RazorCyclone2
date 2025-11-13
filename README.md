# RazorCyclone2 - shoot 2 move

Razor Cyclone but more content and optimized with DOTS!\
Hopefully I don't lose too much motivation...

# Sections
> - [Architecture](#architecture)
> - [Content](#content)
> - [Game Mechanics](#game-mechanics)
> - [Third Person](#third-person)
> - [Roguelike](#roguelike)
>   - [Vacuum Ideas](#vacuum-specific-ideas)
>   - [Cannon Ideas](#cannon-specific-ideas)
>   - [General Effect Ideas](#general-effect-ideas)


## Architecture

I want to make this optimized with DOTS, but this will likely be difficult and take me a while. 



## Content

- A "Classic" mode (the level we currently have) and a "Campaign" mode. Would be p cool to have these modes.
	- Campaign's first level would be our tutorial.
	- Campaign could have a zero-g level.
		- Classic could have specific rounds or perhaps a special enemy that makes the level a zero-g environment.
		- Could even have a negative-g level.
- Geometry Dash (rhythm game)
	- Mouse disabled. Camera set to side-of-shoulder third-person.
	- **A**, **D** directions allowed, but disabled from shooting.
	- Vacuum disabled.
	- Cannon first shot is raycast, ricochets are fixed speed.
	- The 'notes' are floating Fuel cells and targets in the world.
		- The timing for fuel cells is based on 
		- Target positioning will typically be based on 
	- The level setup will have targets/Fuel arranged such that it lines up with a song
	- Hold note: some visual element will represent hold notes.
		- Hit hold notes by spinning. Segments within the hold note will indicate the desired spinrate
		- There will always be a target (easy to hit) to shoot at the end, and this target will always have more targets that would get ricocheted to. The number of ricochet targets would be set up based on how many ricochets the hold should have charged.
- osu!standard (rhythm game)
	- Level is a simple arena. Player can freely travel around in the arena.
	- Targets will appear in the air.
		- Weapon requirements: some can be damaged with either, some only Vacuum or only Cannon
	- Ricochet note: on-screen number will appear indicating required number of ricochets with a shrinking circle around it (osu style). The first ricochet note
	    can only be shot by first Cannon shot, next ricochet notes require hit by ricochet.
		- Level setup should allow a break time (no notes spawn) to allow time for ricochet charging
		- Ricochet charges do not timeout



## Game Mechanics
~~~
DEPRECATED IDEA: the bike is under the Player for W and S, so the bike doesn't end up 'spinning' over the player.
- WASD horizontal spinning is pretty cool and buffs the Cannon. But...
- W Sp S Sh vertical spinning could be something else. Perhaps vertical spinning applies a buff to the Vacuum. Ideas:
	- Vert Spins grant a stack. Each stack increases Vacuum movement strength. Current stack's timer decays over decent duration (3 seconds). Subsequent stacks decay much quicker (one stack every 0.2 seconds). Does not affect Vacuum fuel consumption. Might have a max stack amount.
	- Vert Spins grant a stack. Consume all stacks on the next usage of the Vacuum (will not get consumed if the Vacuum is being used before stacks started), causing the Vacuum to start extra large, extra strong pull, extra strong move, scaling with stack count, for 1 second before decaying back to normal.
~~~



## Third Person?

- How does cannon aiming work? On-screen cursor doesn't seem feasible. A laser beam out of the cannon might not look great.
- Would prob have to consider some idea that's as weird as shooting to move.
- In the end, I think first person will be bette



## Roguelike...?

General notes:
- Would need to determine how/when buffs are granted.
	- Classic mode:
		- By round number. Would be simplist and most consistent.
	- Campaign mode:
		- By level reached, or perhaps reaching checkpoints within a level.
	- When the player can choose buffs, are they given 3 randomly chosen options? Are they given a larger list of options, reducing randomness and allowing more player choice?
	- Is there a 'set'? E.g. like Minecraft's armor slots.

### Vacuum-specific ideas
- Gaining continuous (not letting go of Vacuum) kills on the Vacuum increases Vacuum size, pull strength, move strength, and fuel consumption (up to a max?).
- Vacuum pull/move directions are flipped.
- Vacuum is now a burst weapon like the Cannon. For a duration after bursting, the Vacuum Killbox remains enabled (so that the Vacuum can still have a kill window).
- Vacuum is now a burst weapon like the Cannon. It is now also ranged, shooting a Vacuum projectile (big sphere). Vacuum Killbox is no longer activatable. The Vacuum is no longer a melee weapon.

### Cannon-specific ideas
- Hitscan Cannon (also affects ricochets (or maybe only applies to first shot?)). Move strength greatly increased, fuel consumption extremely increased.
- Cannon shoots multiple projectiles in a line based on last movement command (e.g. if going from **W** to **S**, Cannon will shoot horizontal spread, or if going from **Sp** to **WSp**, Cannon will shoot vertical spread). Ricochet only applies to middle projectile. Move and fuel consumption strength increased.
- If the Player is hit by a Cannon projectile, the Player gains that projectile's remaining ricochet charges + 1 as spin charges (and the projectile is consumed). Ricochet smart targeting can now also target the Player.
- When the player has ricochet spin charges, their next Cannon shot slows down time first (i.e. rmb down slows time until rmb up, and Cannon fires on rmb up instead of down).

### General effect ideas
- Gain 2 seconds of infinite fuel whenever killing methods are alternated between Vacuum and Cannon (ricochet does not interrupt).
- The Player can now over fuel. Overfuel will take double damage before health. However, Vacuum and Cannon movement forces are reduced the more overfuel the Player has.