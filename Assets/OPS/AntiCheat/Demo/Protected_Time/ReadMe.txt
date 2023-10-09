Protected Time:
--------------------

Unitys frame based calculation are mostly based on the UnityEngine.Time.deltaTime.
This time is very susceptible for cheating or hacking tools.
Easily they can speed up or slow down your application.
To prevent speeding / pausing / slowing down, you can use the AntiCheat ProtectedTime.

Using the protected time is straightforward:
1) Create a GameObject and attach the OPS.AntiCheat.Detector.SpeedHackDetector Component.

2) Include the namespace: OPS.AntiCheat.Speed - Here you find the new ProtectedTime class that replaces your UnityEngine.Time class.

3) Replace your used UnityEngine.Time class with ProtectedTime.
Example: UnityEngine.Time.deltaTime to OPS.AntiCheat.Speed.ProtectedTime.deltaTime.
   
4) For some more examples, have a look at the ProtectedTimeDemo class.

5) To get a callback if a cheater got detected, attach to the OPS.AntiCheat.Detector.SpeedHackDetector.OnSpeedHackDetected event.
You will also receive what kind of time cheating (speeding / pausing / slowing down) was used. 