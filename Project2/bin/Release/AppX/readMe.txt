Application Name: Tank Wars
Authors: Fei Tang, Kimple Ke, Ha Jin Song
Created for assessment purpose for Graphics And Interaction


Application Usage
	The application is made to provided entertainment for users using Windows 8.1 tablet device. 
	The application was built using 3D graphic rendering and haranessed Windows tablet device's 
	touchscreen and accelerometer. Using graphic models and device input, user is able to play 
	game which involves moving player's tank model using device input and eliminate enemy AI 
	tank models. 
	The player will be given a tank to control over plane which has been generated using diamond
	-square algorithm (Provided by Fei as he got the most satisfying looking texture generated)
	 and enemy players will be AI players which moves according to the algorithm (Written by 
	 Kimple) All object in the game is generated using custome effect with exception of 
	 Landscape. (Lighting and Shading done by Ha Jin). In addition to that, there are various UI
	 features that help player to see the in-game progress, such radar that is used to detect
	 enemies from distant region; lifebar showing the current health of the player; as well as
	 score which will be constantly updated.
	  

How to use
	The purpose of the game is destory every enemy tank in the game while keeping surviving. 
	Users can control a player tank and fire towards enemy tanks to destory them before enemy
	tanks destory player's tank. The life of player tank is repesented by a life bar at the 
	top of the screen. A radar is provided to show the position of enemies. Each time player
	destory a enemy tank, 10 score will be given. The final score is determined by the number
	 of enemies destoryed and the life left.
	User can either use accelerometer or touchscreen for tablet. These setting can be changed
	in the built-in setting page. User is provided with a UI for touchscreen controller and
	for accelerometer, user will have to tilt their device in direction of desired motion.
	The firing of weapon from tank is done by tab the screen in both mode. Keyboard input
    is always enabled for both mode. User can use A,W,S,D or arrow keys to move player 
	around, and use F to fire. User can also restart and end the game prematurely through 
	built-in game menu.

Approach for object model
	Our group obtained models for tanks and shells from internet, edited them with 3DS MAX and
	exported as fbx files in order to be used in our game.
	As for bounding the game world, we applied Skybox technique. The skybox technique involves
	wrapping the plane of game in a cube. After that, apply Skybox texture to the cube model to 
	generate scenery. This way, it was possible to avoid awkward edge of the game.
	For the plane of the game, we used Diamond-Square algorithm ot generate fractual plane. 
	For the simplcity of the game, we have not applied physics to the shell trajectory (It 
	travels straight) and therefore, it was necessary to have relatively flat plane of the 
	game.  After generating a flat plane, we applied ground texture file that we have obtained 
	to make the plane look more field-looking.

Apporach for graphic and camera motion
	Up on every input from user (e.g. accelerometer/touchscreen input), the player model of the 
	tank goes through transformation and moves. 
	We've used a object shader for every model in our game. And also, we've used a particle 
	system to simulate the hitting and explosion effect.
	The camera is a typcial third person following camera. It keeps a fix distance and a fix 
	look down angle to player, moving and rotating while player is moving or rotating.


Acknowledgement of outside materials
	http://rbwhitaker.wikidot.com/hlsl-tutorials - Used tutorial provided by this website to 
	understand custome lighting and shader. In addition, also learned Skybox technique from here. 
	Skybox texture is google image.
	All sounds, models and their textures are from internet.
	Code about particle system are from SharpDX sample.