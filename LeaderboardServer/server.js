const http = require('http');
const fs = require('fs');
const path = require('path');

const PORT = process.env.PORT || 3000;
const SCORES_FILE = path.join(__dirname, 'scores.json');
const TOP_N = 5;
const EMPTY_ENTRY = { initials: '---', score: 0 };

const CORS_HEADERS = {
  'Access-Control-Allow-Origin': '*',
  'Access-Control-Allow-Methods': 'GET, POST, OPTIONS',
  'Access-Control-Allow-Headers': 'Content-Type',
  'Content-Type': 'application/json',
};

function readScores() {
  if (!fs.existsSync(SCORES_FILE)) {
    const initial = buildScoresFile([]);
    fs.writeFileSync(SCORES_FILE, JSON.stringify(initial, null, 2));
    return initial;
  }
  return JSON.parse(fs.readFileSync(SCORES_FILE, 'utf8'));
}

function buildScoresFile(entries) {
  const sorted = entries.slice().sort((a, b) => b.score - a.score).slice(0, TOP_N);
  while (sorted.length < TOP_N) sorted.push({ ...EMPTY_ENTRY });
  return { scores: sorted.map((e, i) => ({ rank: i + 1, initials: e.initials, score: e.score })) };
}

function respond(res, status, body) {
  res.writeHead(status, CORS_HEADERS);
  res.end(JSON.stringify(body));
}

function readBody(req) {
  return new Promise((resolve, reject) => {
    let data = '';
    req.on('data', chunk => { data += chunk; });
    req.on('end', () => {
      try { resolve(JSON.parse(data)); }
      catch (e) { reject(e); }
    });
    req.on('error', reject);
  });
}

const server = http.createServer(async (req, res) => {
  const url = req.url.split('?')[0];

  if (req.method === 'OPTIONS') {
    res.writeHead(200, CORS_HEADERS);
    res.end();
    return;
  }

  if (req.method === 'GET' && url === '/scores') {
    const data = readScores();
    respond(res, 200, data);
    return;
  }

  if (req.method === 'POST' && url === '/scores') {
    let body;
    try {
      body = await readBody(req);
    } catch {
      respond(res, 400, { error: 'Invalid JSON body' });
      return;
    }

    const { initials, score } = body;
    if (typeof initials !== 'string' || typeof score !== 'number') {
      respond(res, 400, { error: 'initials (string) and score (number) are required' });
      return;
    }

    const current = readScores();
    const real = current.scores.filter(e => e.initials !== '---');
    const updated = buildScoresFile([...real, { initials, score }]);

    const qualifies = updated.scores.some(
      e => e.initials === initials && e.score === score
    );

    if (qualifies) {
      fs.writeFileSync(SCORES_FILE, JSON.stringify(updated, null, 2));
    }

    respond(res, 200, qualifies ? updated : current);
    return;
  }

  respond(res, 404, { error: 'Not found' });
});

if (require.main === module) {
  server.listen(PORT, () => {
    console.log(`Leaderboard server listening on port ${PORT}`);
    readScores();
  });
}

module.exports = { buildScoresFile, EMPTY_ENTRY, TOP_N };
