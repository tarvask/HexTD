# HexTD
Single and PvP Tower Defence game on hexagonal field with usage of Photon Engine

Game has 2 parallel fields, acting independently in strict order.
In PvP mode one process is client and the other is master-client that confirms requests from plain client commands (BuildTower, UpgradeTower, SellTower, etc) and applies them to self too.

Extensive usage of dependency injection with constructors (creating Context structs to pack multiple parameters in single container).
Zenject was added later during the project life, so is's mostly used to manage MainMenu, UI and Level Loading scene.
Talking about core gameplay, architecture hierarchy is as following: PhotonMatchBridge <= TestMatchEngine (+services) <= MatchController (+services) <= FieldController (+services)
ECS-like approach is applied in "systems" like MobsManager, ShootingProcessManager where multiple objects are processed in several methods with loops.

There are different tools to assure better synchronizing between master and client:
- PingDamper (basing on current ping, it provides damper in engine frames to allow deferred application of commands)
- snapshots of game state (restores state after disconnect, helps after both processes were disconnected for a while)
- MatchStateCheckSumComputer (calculates checksums of game state)
- MatchStateCheckSumHistoryHolder (capable of storing history of several checksums, because the client's current frame can be a little behind the master's one)
- MatchStateCheckSumVerification (compares checksums on the client with those received from the master, it requests state on client if only several neighbouring checksums were corrupted)
