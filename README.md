# DiceyParty

A cross-platform multiplayer party game for 3–6 players, playable in the browser on mobile and PC.

**Live:** [diceyparty.com](https://www.diceyparty.com/) *(not maintained anymore — the server may be offline)*

## The idea

Traditional party games take turns one player at a time, which means most of the match is spent watching someone else play. DiceyParty resolves turns **simultaneously**: everyone plans their move at the same time, the server collects all inputs and resolves them together.

That makes matches faster, and it adds a layer of tactics — since you can't see what the others picked, good play means predicting their choices rather than reacting to them.

Six realtime minigames sit on top of the board.

## How it works

The server is authoritative. Clients send intents rather than state, the server waits for every player's input for the round, resolves them as one step, and broadcasts the result. No client can advance the game on its own, and a slow player can't desync the others.

## Stack

- **Unity / C#** — game clients
- **FishNet** — networking layer, authoritative state sync
- **Edgegap** — dedicated game server deployments
- **C# minimal API** — custom matchmaking service that pairs players and hands out server allocations

## Status

Not maintained. Playtested across several live sessions with 3–6 players; never opened up beyond that. The repo is here as a record of the work.
