Detect Modified Computer Time:
------------------------------

Using the current date time is for many application useful. Not always you can use an internet time, 
so you have to trust the user / client, which is not be trusted. Many of us remember the old trick to 
reuse the windows xp 30 days trial, by just reseting the computer clock.
Or even increase the date time to get advantages in games.

All of this is not possible anymore for the user / client by using AntiCheats ComputerTimeHackDetector.
It will notify you when somebody modifies the date time and tries to get advantages.

Using the modified computer time detector is straightforward:
1) Create a GameObject, and attach the OPS.AntiCheat.Detector.ComputerTimeHackDetector Component.

2) Create a custom listener script and attach to the OPS.AntiCheat.Detector.ComputerTimeHackDetector.OnComputerTimeHackDetected event to get notified when the computer time got modified.