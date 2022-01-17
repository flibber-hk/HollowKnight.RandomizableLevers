# HollowKnight.RandomizableLevers
Hollow Knight ItemChanger and Randomizer add-on to allow changing and randomizing levers.

Features:
- Includes all 62 non-godhome non-reversible levers. The lever in Fungus2_31 (the Mantis Village bench room) is excluded.
- Levers will only be replaced if a specific container (e.g. a grub or totem) is requested. Levers will not be instantiated where there usually is not a lever.
- The levers in White Palace and/or Path of Pain will be excluded according to the White Palace long location setting.
- The lever group will behave as the base rando split groups (so will be randomized on start if it's set to a value between 0 and 2).
- When randomizing levers, the Dirtmouth Stag and Resting Grounds stag items will be replaced with lever items that behave identically, except
the item is collected by hitting the lever rather than picking up a shiny (unless replaced). In particular, the stag can be called from Resting
Grounds even without finding the item.

Notes:
- By default, with lever randomization off, the Randomizer will not be told about the levers. To define custom pools, logic and groups involving levers,
the Define Refs option must be selected (if the Randomize Levers option is not). This will cause the randomizer to treat the levers as if they were
all placed vanilla.
- All logic edits must be made with a priority greater than 0.3.
