Introduction
-------------------

Cheating has been around since the beginning of time. Whether it is athletes using performance-enhancing drugs, 
students cheating on exams, or politicians lying to the public, people have always found ways to get ahead dishonestly.

Also in video games cheating is unfortunately common, by using various methods to take advantage oneself over other players. 
This can include techniques such as cheating through the use of mods, hacks, or exploits; or simply playing the 
game in an unintended way that gives an unfair advantage. It can be performed on single-player or multiplayer games. 
In multiplayer games, cheating can involve either manipulating the game itself to gain an advantage 
or playing with other players who are cheating.

To prevent cheating / hacking or manipulating your game data and memory, GuardingPearSoftwares AntiCheat was developed.


AntiCheat features
-------------------

+ Variable protection: Encrypts your variables/fields to prevent memory manipulation.

+ PlayerPrefs protection: Encrypts the Unity PlayerPrefs to secure your data and settings against manipulation.

+ Time protection: Protect your Unity time (like deltaTime) against slowing / stopping / speeding.

+ Computer time protection: Detect when the computer time gets manipulated to gain advantage.

+ Teleportation protection: Detect and prevent player teleportation.

+ Genuine check: Detect if your application was modified or rebuilt.


Variable protection
-------------------
Most of your data, for example positions or health, is stored in the runtime memory.
This memory is easy accessible by cheat tools or data sniffer.
To prevent unwanted modification of the data in the memory, you have to encrypt it.
To do so, you can use AntiCheats protected fields, to protect your application runtime data
against cheater.

Using the protected fields is straightforward:
1) Create a GameObject and attach the OPS.AntiCheat.Detector.FieldCheatDetector Component.

2) Include the namespace OPS.AntiCheat.Field in your scripts - Here you find all the protected field types.

3) Replace your unprotected field types with the protected one.
Example: int to ProtectedInt32

4) To get a callback if a cheater got detected, attach to the OPS.AntiCheat.Detector.FieldCheatDetector.Singleton.OnFieldCheatDetected event.

5) More examples can be found in the Protected_Fields demo.


PlayerPrefs protection
-------------------
Unitys PlayerPrefs are useful to store user settings and user data.
Unfortunately these are not protected or encrypted and can be easily modified.
To prevent this, you can use the AntiCheat Protected PlayerPrefs.

Using the protected player prefs is straightforward:
1) Include the namespace: OPS.AntiCheat.Prefs

2) Here you find 2 classes.
-> ProtectedPlayerPrefs: Replaces the default functions of the Unity PlayerPrefs and adds some new. The protected prefs will be stored at the know default location.

-> ProtectedFileBasedPlayerPrefs: Is a custom implementation of the Unity PlayerPrefs allowing to store the player prefs protected at a custom file path.
   To assign a custom file path, set ProtectedFileBasedPlayerPrefs.FilePath. Now use ProtectedFileBasedPlayerPrefs like you would use the default Unity PlayerPrefs.
   
3) For some more examples, have a look at the Protected_PlayerPrefs demo.


Time protection
-------------------
Unitys frame based calculation are mostly based on the UnityEngine.Time.deltaTime.
This time very susceptible for cheating or hacking tools.
Easily they can speed up or slow down your application.
To prevent speeding / pausing / slowing down, you can use the AntiCheat ProtectedTime.

Using the protected time is straightforward:
1) Create a GameObject and attach the OPS.AntiCheat.Detector.SpeedHackDetector Component.

2) Include the namespace: OPS.AntiCheat.Speed - Here you find the new ProtectedTime class that replaces your UnityEngine.Time class.

3) Replace your used UnityEngine.Time class with ProtectedTime.
Example: UnityEngine.Time.deltaTime to OPS.AntiCheat.Speed.ProtectedTime.deltaTime.

4) To get a callback if a cheater got detected, attach to the OPS.AntiCheat.Detector.SpeedHackDetector.Singleton.OnSpeedHackDetected event.
You will also receive what kind of time cheating (speeding / pausing / slowing down) was used. 
   
5) For some more examples, have a look at the Protected_Time demo.


Computer time protection
-------------------
Using the current date time is for many application useful. Not always you can use an internet time, 
so you have to trust the user / client, which is not be trusted. Many of us remember the old trick to 
reuse the windows xp 30 days trial, by just reseting the computer clock.
Or even increase the date time to get advantages in games.

All of this is not possible anymore for the user / client by using AntiCheats ComputerTimeHackDetector.
It will notify you when somebody modifies the date time and tries to get advantages.

Using the modified computer time detector is straightforward:
1) Create a GameObject, and attach the OPS.AntiCheat.Detector.ComputerTimeHackDetector Component.

2) Create a custom listener script and attach to the OPS.AntiCheat.Detector.ComputerTimeHackDetector.Singleton.OnComputerTimeHackDetected event to get notified when the computer time got modified.

3) For some more examples, have a look at the Detect_ComputerTimeModified demo.


Teleportation protection
-------------------
Players or objects position are stored in the memory, like the most other variables.
A user can easily access the memory with known cheat or hack tool and teleport himself
or increase his speed. AntiCheats AntiTeleporation prevents this and also let you know if a user tried too.

Using the anti teleport feature is straightforward:
1) Create a GameObject and attach the OPS.AntiCheat.Detector.TeleportationCheatDetector Component.

2) Include the namespace: OPS.AntiCheat.Behaviour - Here you find the AntiCheat Behaviours for runtime objects.

3) Attach the OPS.AntiCheat.Behaviour.AntiTeleportation Behaviour to the GameObject (mostly a player object) you want to protect against teleportation.

4) Create a custom listener script and attach to the OPS.AntiCheat.Behaviour.AntiTeleportation.Singleton.OnTeleportationCheatDetected event to get notified when a cheated teleportation got detected.

5) For some more examples, have a look at the Detect_Teleportation demo.


Genuine check
-------------------
A common way of cheating or stealing a whole application is by removing the 
app stores DRM protection or just changing the package name and then republish it.
The genuine check can be used as an anti-piracy check to detect if the application was 
altered after being built.

Using the genuine check is straightforward:
1) Create a GameObject, and attach the OPS.AntiCheat.Detector.GenuineCheckDetector Component.

2) Create a custom listener script and attach to the OPS.AntiCheat.Detector.GenuineCheckDetector.Singleton.OnGenuineCheckFailedDetected event to get notified when the application is or was modified.

3) For more examples, have a look at the Detect_NotGenuine demo.