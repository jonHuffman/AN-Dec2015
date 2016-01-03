# AN-Dec2015

This repository has been set up to track changes and streamline submission of ArenaNet's Mobile Test.

Priority Queue Notes
	- According to the test the method signatures for DequeueMin and DequeueMax were to return void. I assumed and adjusted these to return the the element being dequeued.
	
Flappy Bird Notes
	- Game was made in Unity 5.1.1p2
	- The game runs on Android (and likely iOS) however there are UI issues do do with asset resolution and uGUI anchors.
	- There may be an issue compiling the game. My home computer is set up to work on projects for my current employer and we run a custom build of the Unity UI libraries. I do not believe this will be an issue as I have not made use of anything of the modifications but I must re-install unity before I can be certain.