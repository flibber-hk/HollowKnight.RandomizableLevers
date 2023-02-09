# HollowKnight.RandomizableLevers
Hollow Knight ItemChanger and Randomizer add-on to allow changing and randomizing levers.

Features:
- Includes all 62 non-godhome non-reversible levers. The lever in Fungus2_31 (the Mantis Village bench room, that is hit by the mantis there) is excluded.
- Levers will be replaced if a specific container (e.g. a grub, rock or totem) is requested; otherwise, hitting the lever will give its item(s). 
- Levers will not be instantiated where there is not usually a lever.
- Obtaining the item associated with a lever while in that lever's room will open the relevant gate(s) immediately. This does
not necessarily apply when not in the room with the lever - for instance, obtaining the Shaman Mound Pillar item from Cornifer in
Crossroads will *not* open the pillar immediately.
- The levers in White Palace and/or Path of Pain will be excluded according to the White Palace long location setting.
- The lever group will behave as the base rando split groups (so will be randomized on start if the group is set to a value between 0 and 2).
To restrict levers to be randomized among themselves, choose a positive number for this field that is different to the numbers being used
for the other pools.
- When randomizing levers, the Dirtmouth and Resting Grounds stag items will be replaced with lever items that behave identically, except
the item is collected by hitting the lever rather than picking up a shiny (unless replaced). In particular, the stag can be called from Resting
Grounds even without finding the item (just as in regular stag rando). This behaviour does not depend on the Stag randomization setting.
- Transition fixes will apply as usual; for instance, entering Ruins1_31 from the [left2] transition will open both sides of the
shade soul exit gate.

Notes:
- By default, with lever randomization off, the Randomizer will not be told that the levers exist. To define custom pools, logic and groups involving levers,
the Define Refs option must be selected (if the Randomize Levers option is not). This will cause the randomizer to treat the levers as if they were
all placed vanilla.
- To treat the Dirtmouth or Resting Grounds levers as levers with custom pools:
  - Set stags to vanilla
  - Create a custom pool containing all stags besides Dirtmouth and Resting Grounds (if you want stags randomized)
  - Exclude Dirtmouth Stag and Resting Grounds Stag (rather than the levers) from vanilla in the pool containing the Dirtmouth and Resting Grounds lever items.
- All logic edits must be made with a priority greater than 0.3.


The Lever Stag Locations toggle concerns when levers are *not* randomized; if enabled, the Dirtmouth and Resting Grounds stag locations will be presented as levers
(rather than shinies). These locations will be treated as the regular location rather than the lever location, and will appear as e.g. 
`Dirtmouth_Stag` rather than `Switch-Dirtmouth_Stag` in the randomizer logs.
