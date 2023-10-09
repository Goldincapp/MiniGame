Anti Teleportation:
----------------------

Players or objects position are stored in the memory, like the most other variables.
A user can easily access the memory with known cheat or hack tool and teleport himself
or increase his speed. AntiCheats AntiTeleporation prevents this and also let you know if a user tried too.

Using the anti teleport feature is straightforward:
1) Create a GameObject and attach the OPS.AntiCheat.Detector.TeleportationCheatDetector Component.

2) Include the namespace: OPS.AntiCheat.Behaviour - Here you find the AntiCheat Behaviours for runtime objects.

3) Attach the OPS.AntiCheat.Behaviour.AntiTeleportation Behaviour to the GameObject (mostly a player object) you want to protect against teleportation.

4) Create a custom listener script and attach to the OPS.AntiCheat.Behaviour.AntiTeleportation.OnTeleportationCheatDetected event to get notified when a cheated teleportation got detected.