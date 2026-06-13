const assert = require('assert');
const { buildScoresFile, EMPTY_ENTRY, TOP_N } = require('./server');

let passed = 0;
let failed = 0;

function test(name, fn) {
  try {
    fn();
    console.log(`[PASS] ${name}`);
    passed++;
  } catch (e) {
    console.error(`[FAIL] ${name}: ${e.message}`);
    failed++;
  }
}

test('buildScoresFile pads empty input to 5 placeholder entries', () => {
  const result = buildScoresFile([]);
  assert.strictEqual(result.scores.length, TOP_N);
  result.scores.forEach(e => {
    assert.strictEqual(e.initials, EMPTY_ENTRY.initials);
    assert.strictEqual(e.score, EMPTY_ENTRY.score);
  });
});

test('buildScoresFile assigns rank 1-5 in order', () => {
  const entries = [
    { initials: 'AAA', score: 500 },
    { initials: 'BBB', score: 300 },
    { initials: 'CCC', score: 100 },
  ];
  const result = buildScoresFile(entries);
  assert.strictEqual(result.scores.length, TOP_N);
  assert.strictEqual(result.scores[0].rank, 1);
  assert.strictEqual(result.scores[1].rank, 2);
  assert.strictEqual(result.scores[2].rank, 3);
  assert.strictEqual(result.scores[3].rank, 4);
  assert.strictEqual(result.scores[4].rank, 5);
});

test('buildScoresFile sorts descending by score', () => {
  const entries = [
    { initials: 'CCC', score: 100 },
    { initials: 'AAA', score: 900 },
    { initials: 'BBB', score: 500 },
  ];
  const result = buildScoresFile(entries);
  assert.strictEqual(result.scores[0].initials, 'AAA');
  assert.strictEqual(result.scores[1].initials, 'BBB');
  assert.strictEqual(result.scores[2].initials, 'CCC');
});

test('buildScoresFile trims to top 5 when given more than 5 entries', () => {
  const entries = [
    { initials: 'A', score: 600 },
    { initials: 'B', score: 500 },
    { initials: 'C', score: 400 },
    { initials: 'D', score: 300 },
    { initials: 'E', score: 200 },
    { initials: 'F', score: 100 },
  ];
  const result = buildScoresFile(entries);
  assert.strictEqual(result.scores.length, TOP_N);
  assert.strictEqual(result.scores[4].initials, 'E');
  assert.ok(!result.scores.some(e => e.initials === 'F'));
});

test('buildScoresFile new qualifying score displaces last place', () => {
  const existing = [
    { initials: 'A', score: 600 },
    { initials: 'B', score: 500 },
    { initials: 'C', score: 400 },
    { initials: 'D', score: 300 },
    { initials: 'E', score: 200 },
  ];
  const updated = buildScoresFile([...existing, { initials: 'X', score: 250 }]);
  assert.strictEqual(updated.scores.length, TOP_N);
  assert.ok(updated.scores.some(e => e.initials === 'X' && e.score === 250));
  assert.ok(!updated.scores.some(e => e.initials === 'E'));
});

test('buildScoresFile non-qualifying score does not appear in result', () => {
  const existing = [
    { initials: 'A', score: 600 },
    { initials: 'B', score: 500 },
    { initials: 'C', score: 400 },
    { initials: 'D', score: 300 },
    { initials: 'E', score: 200 },
  ];
  const updated = buildScoresFile([...existing, { initials: 'X', score: 50 }]);
  assert.ok(!updated.scores.some(e => e.initials === 'X'));
  assert.strictEqual(updated.scores[4].initials, 'E');
});

test('buildScoresFile full board of 5 real entries has no placeholder', () => {
  const entries = Array.from({ length: 5 }, (_, i) => ({ initials: String.fromCharCode(65 + i), score: (5 - i) * 100 }));
  const result = buildScoresFile(entries);
  assert.ok(!result.scores.some(e => e.initials === EMPTY_ENTRY.initials));
});

console.log(`\n${passed} passed, ${failed} failed`);
if (failed > 0) process.exit(1);
