/* Loads the snapshot, answers a word, draws every bit, and runs the 64-bit grid. */
(function () {
  'use strict';

  var Bloom = window.Bloom;
  // Written by tools/SiteFilter and asserted in C# by SiteVectorTests.
  var VECTORS = window.SITE_VECTORS || [];

  var $ = function (id) { return document.getElementById(id); };
  var root = document.documentElement;
  var reduceMotion = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)');
  function still() { return !!(reduceMotion && reduceMotion.matches); }

  function fmt(n) { return Math.round(n).toLocaleString('en-US'); }
  function quote(word) { return '“' + word + '”'; }
  function escapeHtml(text) {
    return text.replace(/[&<>"]/g, function (ch) { return { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' }[ch]; });
  }
  function bold(word) { return '<strong>' + escapeHtml(quote(word)) + '</strong>'; }
  function list(items) {
    if (items.length < 2) return items.join('');
    return items.slice(0, -1).join(', ') + ' and ' + items[items.length - 1];
  }
  function csharp(word) {
    return word.replace(/[\\"\u0000-\u001f\u007f-￿]/g, function (ch) {
      if (ch === '\\' || ch === '"') return '\\' + ch;
      return '\\u' + ('0000' + ch.charCodeAt(0).toString(16)).slice(-4);
    });
  }
  function cssVar(name) { return getComputedStyle(root).getPropertyValue(name); }

  /* ---------- The real filter ---------- */

  var input = $('word'), form = $('ask'), verdict = $('verdict'), reason = $('reason');
  var probeList = $('probes'), marks = $('marks'), fieldBox = $('field'), canvas = $('field-canvas');
  var readout = $('readout'), reproduce = $('reproduce-code'), shareButton = $('share'), shareStatus = $('share-status');
  var READOUT_IDLE = readout ? readout.textContent : '';
  var afterRow = shareButton ? shareButton.parentNode : null, proof = document.querySelector('.proof');

  var filter = null, current = null, layout = { cols: 1, rows: 1 }, token = 0;
  var loupe = $('loupe'), loupeCells = [], LOUPE = 7;
  var state = { word: '', decisive: -1 };

  // One canvas pixel per device pixel, redrawn when the picture changes size or shape.
  // Bits run row by row, so a phone's short wide picture holds the same bits as the square.
  var drawn = '';
  function drawField() {
    if (!filter) return;
    var box = canvas.getBoundingClientRect(), ratio = window.devicePixelRatio || 1;
    if (!box.width || !box.height) return;
    var shape = window.BitField.shape(filter, Math.round(box.width / box.height * 4) / 4);
    var width = Math.max(1, Math.min(Math.round(box.width * ratio), shape.cols * 2));
    var height = Math.max(1, Math.min(Math.round(box.height * ratio), shape.rows * 2));
    var key = [shape.cols, width, height].join('x');
    if (key === drawn) return;
    drawn = key;
    var moved = shape.cols !== layout.cols;
    layout = shape;
    window.BitField.paint(filter, canvas, layout, width, height, cssVar('--on'), cssVar('--off'));
    if (moved) Array.prototype.forEach.call(marks.children, function (mark, index) { place(mark, current.positions[index]); });
  }

  function place(node, position) {
    node.style.left = ((position % layout.cols) + 0.5) / layout.cols * 100 + '%';
    node.style.top = (Math.floor(position / layout.cols) + 0.5) / layout.rows * 100 + '%';
  }

  // The loupe: the bits around one position, at a size where each is a cell.
  function look(position) {
    if (!filter || position < 0 || position >= filter.m) { loupe.hidden = true; return; }
    var half = (LOUPE - 1) / 2, col = position % layout.cols, row = Math.floor(position / layout.cols);
    loupeCells.forEach(function (cell, index) {
      var x = col + (index % LOUPE) - half, y = row + Math.floor(index / LOUPE) - half, at = y * layout.cols + x;
      var inside = x >= 0 && x < layout.cols && y >= 0 && at < filter.m;
      var on = inside && filter.bit(at) === 1;
      cell.className = 'cell' + (on ? ' on' : '') + (inside ? '' : ' none') + (at === position ? ' aim' : '');
      cell.textContent = inside ? (on ? '1' : '0') : '';
    });
    place(loupe, position);
    loupe.hidden = false;
    // Open on the side that stays inside the picture and covers the fewest marks.
    var box = canvas.getBoundingClientRect(), size = loupe.offsetWidth, reach = 16;
    var px = (col + 0.5) / layout.cols * box.width, py = (row + 0.5) / layout.rows * box.height, best = null;
    [[false, false], [true, false], [false, true], [true, true]].forEach(function (side) {
      var left = side[0] ? px - reach - size : px + reach, top = side[1] ? py - reach - size : py + reach;
      var cost = (left < 0 || left + size > box.width ? 10 : 0) + (top < 0 || top + size > box.height ? 10 : 0);
      Array.prototype.forEach.call(marks.children, function (mark, index) {
        if (current && current.positions[index] === position) return;
        var mx = parseFloat(mark.style.left) / 100 * box.width, my = parseFloat(mark.style.top) / 100 * box.height;
        if (mx > left - 13 && mx < left + size + 13 && my > top - 13 && my < top + size + 13) cost++;
      });
      if (!best || cost < best.cost) best = { cost: cost, side: side };
    });
    loupe.classList.toggle('left', best.side[0]);
    loupe.classList.toggle('up', best.side[1]);
    readout.textContent = 'Bit ' + fmt(position) + ' is ' + (filter.bit(position) ? 'on.' : 'off.');
  }
  function lookAway() { loupe.hidden = true; readout.textContent = READOUT_IDLE; }

  function probeItem(index) {
    var item = document.createElement('li'), cell = document.createElement('span'), addr = document.createElement('span');
    item.className = 'probe unread';
    cell.className = 'cell'; addr.className = 'addr';
    item.appendChild(cell); item.appendChild(addr);
    item.addEventListener('mouseenter', function () { heat(index, true); });
    item.addEventListener('mouseleave', function () { heat(index, false); });
    return item;
  }

  // Pointing at one of the seven cells shows where it lives and what surrounds it.
  function heat(index, on) {
    var mark = marks.children[index];
    if (!mark) return;
    mark.classList.toggle('hot', on);
    if (on) look(current.positions[index]); else lookAway();
  }

  function reveal(index, landed) {
    var item = probeList.children[index], isOn = current.set[index], position = current.positions[index];
    item.className = 'probe ' + (isOn ? 'on' : 'off') + (landed ? ' landed' : '');
    item.firstChild.className = 'cell' + (isOn ? ' on' : '');
    item.firstChild.textContent = isOn ? '1' : '0';
    item.lastChild.textContent = fmt(position);
    item.setAttribute('aria-label', 'Bit ' + fmt(position) + ' is ' + (isOn ? 'on' : 'off'));

    var mark = document.createElement('span');
    mark.className = 'mark' + (isOn ? '' : ' zero');
    mark.textContent = String(index + 1);
    place(mark, position);
    if (!landed) mark.style.animation = 'none';
    marks.appendChild(mark);
  }

  function clearProbes(waiting) {
    marks.textContent = '';
    loupe.hidden = true;
    Array.prototype.forEach.call(probeList.children, function (item) {
      item.className = 'probe ' + (waiting ? 'waiting' : 'unread');
      item.firstChild.className = 'cell'; item.firstChild.textContent = '';
      item.lastChild.textContent = '';
      item.removeAttribute('aria-label');
    });
  }

  function conclude(word) {
    var k = filter.k, decisive = state.decisive;
    verdict.classList.remove('pending'); reason.classList.remove('pending');
    var pair = current.present ? impossible(word) : '';
    if (pair) {
      // The page can call this one: no entry in the list contains this letter pair.
      verdict.textContent = 'Probably a word.';
      verdict.classList.add('yes');
      reason.innerHTML = '<span class="tag">False positive</span> All ' + k + ' bits for ' + bold(word) +
        ' are on, so the filter has to say probably. It is wrong. No word in the list has the letters ' + quote(pair) +
        ' together, which is the one thing the page can check without the words. This is the 1 in 100.';
    } else if (current.present) {
      verdict.textContent = 'Probably a word.';
      verdict.classList.add('yes');
      reason.innerHTML = 'All ' + k + ' bits for ' + bold(word) + ' are on. Either it is one of the ' + fmt(filter.n) +
        ' words, or it is one of the roughly 1 in 100 strangers whose bits were all switched on by other words. ' +
        'The page cannot tell you which. It never kept the words.';
    } else {
      verdict.textContent = 'Definitely not a word.';
      verdict.classList.remove('yes');
      reason.innerHTML = (decisive === 0 ? 'The first bit' : 'Bit ' + (decisive + 1) + ' of ' + k) + ' for ' + bold(word) +
        ' is off: number ' + fmt(current.positions[decisive]) + '. Adding ' + bold(word) +
        ' would have switched it on, so it was never added. One 0 is proof, and the lookup stops there.';
    }
    for (var i = decisive + 1; i < k; i++) probeList.children[i].className = 'probe unread';
  }

  // Letter pairs that occur in no entry of the list (nonsense.js; verified by check-nonsense.mjs).
  function impossible(word) {
    var joins = window.Nonsense.joins;
    for (var i = 0; i < joins.length; i++) if (word.indexOf(joins[i]) >= 0) return joins[i];
    return '';
  }

  function answer(raw, animate) {
    if (!filter) return;
    var word = raw.trim().toLowerCase();
    state.word = word;
    token++;
    shareStatus.textContent = '';

    if (!word) {
      current = null; state.decisive = -1;
      clearProbes(false);
      verdict.textContent = 'Type a word.';
      verdict.classList.remove('yes', 'pending');
      reason.textContent = 'Any word, in any language. The filter only knows English, and only ' + fmt(filter.n) + ' words of it.';
      reason.classList.remove('pending');
      reproduce.textContent = '';
      afterRow.hidden = true; proof.hidden = true;
      syncAddress('');
      return;
    }

    current = filter.probe(word);
    var decisive = state.decisive = current.present ? filter.k - 1 : current.firstBlank;
    afterRow.hidden = false; proof.hidden = false;
    reproduce.innerHTML =
      'var filter = FilterBuilder.Build(' + String(filter.n).replace(/\B(?=(\d{3})+(?!\d))/g, '_') + ', 0.01);\n' +
      'int[] bits = filter.ComputeHash("' + escapeHtml(csharp(word)) + '");\n' +
      '<span class="c">// ' + current.positions.join(', ') + '</span>';
    syncAddress(word);

    if (!animate || still()) {
      clearProbes(false);
      for (var i = 0; i <= decisive; i++) reveal(i, false);
      conclude(word);
      return;
    }

    var mine = token, step = 0;
    clearProbes(true);
    verdict.classList.add('pending'); reason.classList.add('pending');
    (function next() {
      if (mine !== token) return;
      reveal(step, true);
      if (step++ < decisive) setTimeout(next, 150);
      else setTimeout(function () { if (mine === token) conclude(word); }, 180);
    })();
  }

  var addressTimer = 0;
  function address(word) {
    var url = new URL(window.location.href);
    if (word) url.searchParams.set('w', word); else url.searchParams.delete('w');
    url.hash = '';
    return url;
  }
  function syncAddress(word) {
    clearTimeout(addressTimer);
    addressTimer = setTimeout(function () {
      try { history.replaceState(null, '', address(word)); }
      catch (error) { /* file:// or a locked-down frame: the address simply stays put */ }
    }, 400);
  }

  function fill(name, value) {
    Array.prototype.forEach.call(document.querySelectorAll('[data-stat="' + name + '"]'), function (node) { node.textContent = value; });
  }

  function selfCheck() {
    return VECTORS.length > 0 && VECTORS.every(function (vector) {
      var got = Bloom.positions(vector.word, filter.m, filter.k).positions;
      return got.length === vector.positions.length && got.every(function (p, i) { return p === vector.positions[i]; }) &&
        filter.contains(vector.word) === vector.present;
    });
  }

  function down(title, detail) {
    filter = null; current = null;
    verdict.textContent = title;
    verdict.classList.remove('yes', 'pending'); reason.classList.remove('pending');
    reason.textContent = detail;
    root.classList.add('filter-down');
    input.disabled = true;
    form.querySelector('button').disabled = true;
    if ($('run')) $('run').disabled = true;
  }

  function start(snapshot) {
    try { filter = Bloom.load(snapshot); }
    catch (error) { down('The filter did not load.', error.message + ' The reference below does not depend on it.'); return; }
    if (!selfCheck()) {
      down('The filter is switched off.',
        'In this browser the hash does not reproduce the C# library’s test vectors, so no answers are shown. The reference below is unaffected.');
      return;
    }

    var ratio = filter.countSetBits() / filter.m;
    fill('words', fmt(filter.n));
    fill('bits', fmt(filter.m));
    fill('kilobytes', fmt(Math.ceil(filter.m / 8) / 1000));
    fill('fill', (ratio * 100).toFixed(1) + '%');
    fill('estimate', (Math.pow(ratio, filter.k) * 100).toFixed(2) + '%');

    for (var i = 0; i < filter.k; i++) probeList.appendChild(probeItem(i));
    for (var c = 0; c < LOUPE * LOUPE; c++) { loupeCells.push(document.createElement('span')); loupe.appendChild(loupeCells[c]); }
    drawField();

    var asked = '';
    try { asked = new URL(window.location.href).searchParams.get('w') || ''; } catch (error) { asked = ''; }
    input.value = asked || input.value || 'bloom';
    answer(input.value, true);
  }

  if (form) {
    form.addEventListener('submit', function (event) {
      event.preventDefault();
      answer(input.value, true);
      // On a small screen the verdict can be below the fold: bring the answer into view.
      var box = verdict.getBoundingClientRect();
      if (box.bottom > window.innerHeight || box.top < 0) {
        input.blur();
        probeList.scrollIntoView({ behavior: still() ? 'auto' : 'smooth', block: 'center' });
      }
    });
    input.addEventListener('input', function () { answer(input.value, false); });
    Array.prototype.forEach.call(form.querySelectorAll('[data-try]'), function (button) {
      button.addEventListener('click', function () { input.value = button.getAttribute('data-try'); answer(input.value, true); });
    });

    function read(event) {
      if (!filter) return;
      var box = canvas.getBoundingClientRect();
      var x = Math.floor((event.clientX - box.left) / box.width * layout.cols), y = Math.floor((event.clientY - box.top) / box.height * layout.rows);
      if (x < 0 || y < 0 || x >= layout.cols || y >= layout.rows) { lookAway(); return; }
      look(y * layout.cols + x);
    }
    fieldBox.addEventListener('pointermove', read);
    fieldBox.addEventListener('pointerdown', read);
    fieldBox.addEventListener('pointerleave', lookAway);

    var resizeTimer = 0;
    window.addEventListener('resize', function () {
      clearTimeout(resizeTimer);
      resizeTimer = setTimeout(drawField, 150);
    });

    shareButton.addEventListener('click', function () {
      if (!current) return;
      var word = state.word, url = String(address(word));
      var bits = current.set.slice(0, state.decisive + 1).map(function (on) { return on ? '1' : '0'; }).join('');
      var text = quote(word) + ' → ' + bits + (current.present ? ', probably a word' : ', definitely not a word') +
        '. This page knows ' + fmt(filter.n) + ' English words and stores none of them.';
      var copied = function () { shareStatus.textContent = 'Copied. Paste it anywhere.'; };
      var blocked = function () { shareStatus.textContent = 'Copying is blocked here. Copy the address bar instead.'; };
      var copy = function () {
        if (navigator.clipboard && navigator.clipboard.writeText) navigator.clipboard.writeText(text + ' ' + url).then(copied, blocked);
        else blocked();
      };
      // Phones get the system share sheet; a desktop gets the clipboard.
      var touch = window.matchMedia && window.matchMedia('(pointer: coarse)').matches;
      if (touch && navigator.share) navigator.share({ text: text, url: url }).catch(function (error) { if (error && error.name !== 'AbortError') copy(); });
      else copy();
    });

    verdict.textContent = 'Loading the filter…';
    fetch('words.json').then(function (response) {
      if (!response.ok) throw new Error('words.json returned ' + response.status + '.');
      return response.json();
    }).then(start).catch(function (error) {
      down('The filter did not load.', (error && error.message ? error.message + ' ' : '') + 'The reference below does not depend on it.');
    });
  }

  /* ---------- The 64-bit grid ---------- */

  // FilterBuilder.Build(14, 0.113) in C#: 64 bits, 3 hashes. vectors.js carries that size, and
  // SiteVectorTests asserts it against the library.
  var TOY_BITS = 64, TOY_HASHES = 3, TOY_KEYS = 14;
  var SAMPLES = ['apple', 'kiwi', 'plum', 'fig', 'pear', 'mango', 'lemon', 'peach', 'grape', 'melon', 'cherry', 'olive',
    'guava', 'papaya', 'quince', 'banana', 'orange', 'apricot', 'coconut', 'lychee', 'nectarine', 'currant', 'damson',
    'mulberry', 'pomelo', 'tangelo', 'yuzu', 'lime', 'date', 'raisin', 'medlar', 'loquat', 'persimmon', 'satsuma',
    'clementine', 'kumquat', 'rhubarb', 'sloe', 'bilberry', 'greengage'];

  var grid = $('grid'), toyForm = $('toy'), toyInput = $('toy-key'), toySays = $('toy-says');
  var addedBox = grid ? document.querySelector('.added') : null, addedList = $('added');
  var gridCount = $('grid-count'), gridOdds = $('grid-odds'), resetButton = $('toy-reset');
  var bits = [], owner = [], keys = [], cells = [];

  function toyPositions(key) {
    var seen = {}, out = [];
    Bloom.positions(key, TOY_BITS, TOY_HASHES).positions.forEach(function (p) { if (!seen[p]) { seen[p] = true; out.push(p); } });
    return out.sort(function (a, b) { return a - b; });
  }
  function bitWord(positions) { return (positions.length === 1 ? 'bit ' : 'bits ') + list(positions.map(String)); }
  function owners(positions) {
    var names = [];
    positions.forEach(function (p) { var name = bold(owner[p]); if (names.indexOf(name) < 0) names.push(name); });
    return list(names);
  }
  function odds() {
    var set = bits.filter(Boolean).length, p = Math.pow(set / TOY_BITS, TOY_HASHES) * 100;
    return { set: set, text: p === 0 ? '0%' : p < 10 ? p.toFixed(1) + '%' : p.toFixed(0) + '%' };
  }

  function paint(aim, flipped) {
    var on = [];
    cells.forEach(function (cell, i) {
      var order = flipped ? flipped.indexOf(i) : -1;
      cell.className = 'cell' + (bits[i] ? ' on' : '') + (aim && aim.indexOf(i) >= 0 ? ' aim' : '');
      cell.textContent = bits[i] ? '1' : '0';
      cell.style.animationDelay = order > 0 ? order * 70 + 'ms' : '';
      if (order >= 0) { void cell.offsetWidth; cell.classList.add('flip'); }
      if (bits[i]) on.push(i);
    });
    var now = odds();
    gridCount.textContent = now.set + ' of ' + TOY_BITS + ' bits on';
    gridOdds.textContent = 'A stranger gets through ' + now.text + ' of the time' +
      (keys.length > TOY_KEYS ? ', and it is ' + (keys.length - TOY_KEYS) + ' over its ' + TOY_KEYS + ' keys' : '');
    grid.setAttribute('aria-label', 'A 64-bit Bloom filter. ' + (on.length ? 'Bits on: ' + on.join(', ') + '.' : 'No bits are on.') +
      (aim && aim.length ? ' This key points at ' + bitWord(aim) + '.' : ''));
    resetButton.disabled = keys.length === 0;
  }

  function say(tag, html) {
    toySays.innerHTML = (tag ? '<span class="tag">' + tag + '</span> ' : '') + html;
  }

  function chip(key) {
    var item = document.createElement('li'), button = document.createElement('button');
    button.type = 'button'; button.className = 'chip'; button.textContent = key;
    button.setAttribute('aria-label', 'Check ' + key);
    button.addEventListener('click', function () { toyInput.value = key; check(key); });
    item.appendChild(button); addedList.appendChild(item);
    addedBox.hidden = false;
  }

  function insert(key) {
    var positions = toyPositions(key), fresh = positions.filter(function (p) { return !bits[p]; });
    fresh.forEach(function (p) { bits[p] = true; owner[p] = key; });
    keys.push(key);
    chip(key);
    return { positions: positions, fresh: fresh, shared: positions.filter(function (p) { return fresh.indexOf(p) < 0; }) };
  }

  function nextSample() {
    for (var i = 0; i < SAMPLES.length; i++) if (keys.indexOf(SAMPLES[i]) < 0) return SAMPLES[i];
    return '';
  }

  function add(key) {
    if (!key) { say('', 'Type a key first.'); toyInput.focus(); return; }
    if (keys.indexOf(key) >= 0) {
      paint(toyPositions(key));
      say('', 'You already added ' + bold(key) + '. Adding a key twice changes nothing: its bits are on already.');
      return;
    }
    var made = insert(key);
    paint(made.positions, made.fresh);
    if (!made.shared.length) say('', 'Added ' + bold(key) + '. It switched on ' + bitWord(made.fresh) + '.');
    else if (made.fresh.length) say('', 'Added ' + bold(key) + '. It switched on ' + bitWord(made.fresh) + '; ' + bitWord(made.shared) +
      (made.shared.length === 1 ? ' was' : ' were') + ' already on, from ' + owners(made.shared) + '. Keys share bits.');
    else say('', 'Added ' + bold(key) + ', and nothing changed: ' + bitWord(made.shared) + ' were already on, from ' + owners(made.shared) +
      '. The filter would have said possibly present before you added it.');
    toyInput.value = nextSample();
  }

  function check(key) {
    if (!key) { say('', 'Type a key first.'); toyInput.focus(); return; }
    var positions = toyPositions(key), off = positions.filter(function (p) { return !bits[p]; });
    paint(positions);
    if (off.length) say('Definitely absent', bold(key) + ' was never added. ' + (off.length === 1 ? 'Bit ' + off[0] + ' is' : 'Bits ' + list(off.map(String)) + ' are') +
      ' off, and adding ' + bold(key) + ' would have switched ' + (off.length === 1 ? 'it' : 'them') + ' on.');
    else if (keys.indexOf(key) >= 0) say('Possibly present', 'Every bit for ' + bold(key) + ' is on. You did add it, but the filter cannot know that. It sees ' +
      positions.length + ' ones and nothing else.');
    else say('False positive', 'You never added ' + bold(key) + ', yet ' + bitWord(positions) + ' are all on, switched on by ' + owners(positions) +
      '. The filter has to answer possibly present.');
  }

  function findStranger() {
    if (!keys.length) { say('', 'Add some keys first. With every bit 0, nothing can be mistaken for anything.'); return; }
    var tries = 20000, candidate = '';
    function passes(key) { return keys.indexOf(key) < 0 && toyPositions(key).every(function (p) { return bits[p]; }); }
    for (var s = 0; s < SAMPLES.length && !candidate; s++) if (passes(SAMPLES[s])) candidate = SAMPLES[s];
    for (var i = 0; i < tries && !candidate; i++) { var made = window.Nonsense.make(Math.random); if (passes(made)) candidate = made; }
    if (!candidate) {
      say('', 'None of ' + fmt(tries) + ' made-up keys got through. With only ' + odds().set + ' bits on, that is what should happen. Add more keys and try again.');
      return;
    }
    toyInput.value = candidate;
    check(candidate);
  }

  if (grid) {
    for (var c = 0; c < TOY_BITS; c++) {
      var cell = document.createElement('span');
      cell.setAttribute('data-i', String(c));
      grid.appendChild(cell); cells.push(cell); bits.push(false); owner.push('');
    }
    addedBox.hidden = true;
    paint();

    toyForm.addEventListener('submit', function (event) { event.preventDefault(); add(toyInput.value.trim()); });
    $('toy-check').addEventListener('click', function () { check(toyInput.value.trim()); });
    $('toy-find').addEventListener('click', findStranger);
    $('toy-five').addEventListener('click', function () {
      var names = [], fresh = [], touched = [];
      for (var i = 0; i < SAMPLES.length && names.length < 5; i++) {
        if (keys.indexOf(SAMPLES[i]) >= 0) continue;
        var made = insert(SAMPLES[i]);
        names.push(bold(SAMPLES[i])); fresh = fresh.concat(made.fresh); touched = touched.concat(made.positions);
      }
      if (!names.length) { say('', 'That is every word on the page’s list. Type your own.'); return; }
      paint(null, fresh);
      say('', 'Added ' + list(names) + '. ' + odds().set + ' of ' + TOY_BITS + ' bits are on now. Check a key you did not add.');
      toyInput.value = nextSample();
    });
    resetButton.addEventListener('click', function () {
      for (var i = 0; i < TOY_BITS; i++) { bits[i] = false; owner[i] = ''; }
      keys = []; addedList.textContent = ''; addedBox.hidden = true;
      paint();
      toyInput.value = SAMPLES[0];
      say('', 'Cleared. Every bit is 0 again, so every key is definitely absent.');
    });
  }

  /* ---------- Feed it nonsense ---------- */

  var runButton = $('run'), runCanvas = $('run-canvas'), runSays = $('run-says'), passedList = $('passed'), passedNote = $('passed-note');
  var RUN = 10000, RUN_SIDE = 100;
  var totals = { runs: 0, read: 0, passed: 0 }, runState = null, runScale = 0;

  // One square per made-up word: filled if the filter let it through.
  function drawRun(from) {
    var box = runCanvas.getBoundingClientRect(), ratio = window.devicePixelRatio || 1;
    var cell = Math.max(2, Math.floor(box.width * ratio / RUN_SIDE));
    var ctx = runCanvas.getContext('2d');
    if (cell !== runScale) { runScale = cell; runCanvas.width = runCanvas.height = cell * RUN_SIDE; from = 0; ctx.clearRect(0, 0, runCanvas.width, runCanvas.height); }
    var gap = cell > 3 ? 1 : 0, off = cssVar('--off').trim(), on = cssVar('--on').trim();
    for (var i = from || 0; i < RUN; i++) {
      ctx.fillStyle = runState && i < runState.read && runState.hit[i] ? on : off;
      ctx.fillRect((i % RUN_SIDE) * cell, Math.floor(i / RUN_SIDE) * cell, cell - gap, cell - gap);
    }
  }

  function runSummary() {
    var read = runState.read, passed = runState.words.length;
    var text = fmt(read - passed) + ' turned away, ' + fmt(passed) + ' let through: ' + (read ? (passed / read * 100).toFixed(2) : '0.00') +
      '%. Sized for ' + (filter.p * 100).toFixed(2) + '%.';
    if (totals.runs > 1 && read === RUN) text += ' Over all ' + totals.runs + ' runs: ' + (totals.passed / totals.read * 100).toFixed(2) + '% of ' + fmt(totals.read) + '.';
    runSays.textContent = text;
  }

  function passedChips(words) {
    passedList.textContent = '';
    words.slice(0, 12).forEach(function (word) {
      var item = document.createElement('li'), button = document.createElement('button');
      button.type = 'button'; button.className = 'chip'; button.textContent = word;
      button.setAttribute('aria-label', 'Check ' + word + ' at the top of the page');
      button.addEventListener('click', function () {
        input.value = word;
        answer(word, true);
        form.scrollIntoView({ behavior: still() ? 'auto' : 'smooth', block: 'start' });
        input.focus({ preventScroll: true });
      });
      item.appendChild(button); passedList.appendChild(item);
    });
    var tail = ' None is a word, and the filter says “probably” to every one. Press one to see its seven bits.';
    passedNote.textContent = words.length === 0 ? 'None got through this time. That is rare; run it again.' :
      words.length > 12 ? 'Here are 12 of the ' + fmt(words.length) + ' that got through.' + tail :
      'These ' + words.length + ' got through.' + tail;
  }

  if (runButton) {
    drawRun(0);
    window.addEventListener('resize', function () { drawRun(0); });
    runButton.addEventListener('click', function () {
      if (!filter) return;
      var batch = still() ? RUN : 400;
      runState = { read: 0, hit: new Uint8Array(RUN), words: [] };
      runButton.disabled = true;
      passedList.textContent = ''; passedNote.textContent = '';
      drawRun(0);
      (function step() {
        var from = runState.read;
        for (var i = 0; i < batch && runState.read < RUN; i++, runState.read++) {
          var word = window.Nonsense.make(Math.random);
          if (filter.contains(word)) { runState.hit[runState.read] = 1; runState.words.push(word); }
        }
        drawRun(from);
        if (runState.read < RUN) { requestAnimationFrame(step); return; }
        totals.runs++; totals.read += RUN; totals.passed += runState.words.length;
        runSummary();
        passedChips(runState.words);
        runButton.disabled = false;
        runButton.textContent = 'Feed it 10,000 more';
      })();
    });
  }

  /* ---------- Size your own ---------- */

  var keysInput = $('keys'), rateInput = $('rate'), sized = $('sized');
  var INT_MAX = 2147483647;

  function bytes(count) {
    if (count < 1000) return fmt(count) + ' bytes';
    if (count < 1e6) return (count / 1e3).toFixed(1) + ' kB';
    if (count < 1e9) return (count / 1e6).toFixed(2) + ' MB';
    return (count / 1e9).toFixed(2) + ' GB';
  }

  function resize() {
    var n = Number(String(keysInput.value).replace(/[,_\s]/g, '')), p = Number(rateInput.value);
    var valid = Number.isInteger(n) && n >= 1;
    if (!valid) { sized.textContent = 'Enter a whole number of keys, 1 or more.'; return; }
    if (n > INT_MAX) { sized.textContent = 'FilterBuilder.Build takes an Int32 count, so one filter holds at most 2,147,483,647 keys.'; return; }
    var made = Bloom.size(n, p);
    if (!made) {
      sized.textContent = 'That needs more than 2,147,483,647 bits. The library throws ArgumentOutOfRangeException rather than quietly building a smaller filter. Split the keys across several filters.';
      return;
    }
    sized.innerHTML = '<strong>' + bytes(Math.ceil(made.m / 8)) + '</strong>: ' + fmt(made.m) + ' bits, ' + made.k + ' looked at per key. Each key costs ' +
      (made.m / n).toFixed(1) + ' bits whether it is four bytes or four kilobytes.<br><code>FilterBuilder.Build(' +
      String(n).replace(/\B(?=(\d{3})+(?!\d))/g, '_') + ', ' + rateInput.value + ')</code>';
  }

  if (keysInput) {
    keysInput.addEventListener('input', resize);
    rateInput.addEventListener('change', resize);
    resize();
  }

  /* ---------- Copy buttons ---------- */

  Array.prototype.forEach.call(document.querySelectorAll('[data-copy]'), function (button) {
    var status = document.getElementById(button.getAttribute('aria-describedby'));
    button.addEventListener('click', function () {
      var done = function () { if (status) status.textContent = 'Copied.'; };
      var failed = function () { if (status) status.textContent = 'Copying is blocked here. Select the text instead.'; };
      if (navigator.clipboard && navigator.clipboard.writeText) navigator.clipboard.writeText(button.getAttribute('data-copy')).then(done, failed);
      else failed();
      if (status) setTimeout(function () { status.textContent = ''; }, 4000);
    });
  });
})();
