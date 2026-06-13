# Leaderboard Server

Minimal Node.js HTTP server for the NES-Style Tetris global top-5 leaderboard.
Reads and writes a local `scores.json` file. No external database required.

## Requirements

- Node.js >= 18.0.0

## Setup

```
cd LeaderboardServer
npm install
npm start
```

No npm dependencies are used; `npm install` is a no-op but included for convention.

## Configuration

| Variable | Default | Description |
|----------|---------|-------------|
| `PORT`   | `3000`  | Port the server listens on |

Override the port:

```
PORT=8080 npm start
```

## Endpoints

### GET /scores

Returns the current top-5 leaderboard. Entries are padded with `{"initials":"---","score":0}` if fewer than 5 scores have been submitted.

Response (200):
```json
{
  "scores": [
    { "rank": 1, "initials": "AAA", "score": 999999 },
    { "rank": 2, "initials": "BBB", "score": 500000 },
    { "rank": 3, "initials": "---", "score": 0 },
    { "rank": 4, "initials": "---", "score": 0 },
    { "rank": 5, "initials": "---", "score": 0 }
  ]
}
```

### POST /scores

Submit a new score. If the score qualifies for the top 5 it is persisted; otherwise the current leaderboard is returned unchanged.

Request body:
```json
{ "initials": "ABC", "score": 123456 }
```

Response (200): the updated (or unchanged) scores array in the same format as GET /scores.

### OPTIONS /scores

Returns 200 with CORS headers for browser preflight requests.

## CORS

All responses include `Access-Control-Allow-Origin: *` so the Unity WebGL build can call the server from any browser origin.

## scores.json

Created automatically on first startup in the `LeaderboardServer/` directory with 5 empty placeholder entries. Do not edit by hand while the server is running.
