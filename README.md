TestBlock
=========

Training project - simple Minecraft-like online multiplayer game. Work is just beginning :)

* Server: .Net Framework 4.5 + TPL DataFlow (task processing) + MonoGame (for game math)

* Network: Lidgren Network Library (handy messaging over UDP, simple serialization) + LZ4 (large messages compression)

* Client: Unity3d (WebPlayer compartible) + Zenject (Unity friendly DI framework)

* Logging: (customized on client) NLog + Log4View (log viewer)

* Testing: NUnit + NSubstitute + FluentAssertions

Build and run
-------------

Works for sure on Visual Studio Pro 2013 + Visual Studio Tools for Unity 1.9 + Unity 4.6 (because of new Unity UI)

History
------

- 0.1 - Simplest possible noise-based world (hills only), make sure client works in Web-restricted environment. Connect, walk, jump, lookout, disconnect of several hundreds of bots.
- 0.2 - Implement Voronoi-partitioned world, make graph of world-zones, make some visualization tool of map generation process. Client: render zones in different colors.
