Genuine Check:
------------------------------

A common way of cheating or stealing a whole application is by removing the 
app stores DRM protection or just changing the package name and then republish it.
The genuine check can be used as an anti-piracy check to detect if the application was 
altered after being built.

Using the genuine check is straightforward:
1) Create a GameObject, and attach the OPS.AntiCheat.Detector.GenuineCheckDetector Component.

2) Create a custom listener script and attach to the OPS.AntiCheat.Detector.GenuineCheckDetector.OnGenuineCheckFailedDetected event to get notified when the application is or was modified.