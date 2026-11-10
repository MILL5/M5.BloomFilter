/* The reader: loads the snapshot, answers a word, and draws the cards it read. */
(function () {
  'use strict';

  var Bloom = window.Bloom;
  var COLUMNS = 80, ROWS = 12, PER_CARD = COLUMNS * ROWS;
  var ROW_NAMES = ['12', '11', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];
  var CARD_THICKNESS_INCHES = 0.007;

  // Written by tools/SiteFilter and asserted in C# by SiteVectorTests.
  var VECTORS = window.SITE_VECTORS || [];

  var $ = function (id) { return document.getElementById(id); };
  var input = $('word'), form = $('ask'), verdict = $('verdict'), reason = $('reason');
  var probeFigure = $('probe'), cardCanvas = $('card'), cardCaption = $('card-caption');
  var trayCanvas = $('tray'), probeBody = $('probe-rows'), probeCaption = $('probe-caption');
  var reproduce = $('reproduce-code'), lamp = $('lamp');
  var reduceMotion = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)');

  var filter = null, current = null, shown = 0, feedToken = 0, blankCard = null;
  var palette = readPalette();

  function readPalette() {
    var style = getComputedStyle(document.documentElement);
    function v(name) { return style.getPropertyValue(name).trim(); }
    return {
      tray: v('--tray'), manila: v('--manila'), edge: v('--manila-edge'), formInk: v('--form-ink'),
      ink: v('--ink'), pencil: v('--pencil'), soft: v('--tray-soft')
    };
  }

  function fmt(n) { return Math.round(n).toLocaleString('en-US'); }
  function pad(n, width) { var s = String(n); while (s.length < width) s = '0' + s; return s; }
  function quote(word) { return '“' + word + '”'; }

  function locate(position) {
    var inCard = position % PER_CARD;
    return {
      card: Math.floor(position / PER_CARD) + 1,
      column: Math.floor(inCard / ROWS) + 1,
      rowIndex: inCard % ROWS,
      row: ROW_NAMES[inCard % ROWS],
      inCard: inCard
    };
  }

  function cardCount() { return Math.ceil(filter.m / PER_CARD); }

  /* ---------- Drawing ---------- */

  function fit(canvas) {
    var ratio = Math.min(window.devicePixelRatio || 1, 3);
    var width = Math.max(1, Math.round(canvas.clientWidth * ratio));
    var height = Math.max(1, Math.round(canvas.clientHeight * ratio));
    if (canvas.width !== width || canvas.height !== height) { canvas.width = width; canvas.height = height; }
    return { width: width, height: height, ratio: ratio };
  }

  // Card geometry in inches: 7 3/8 by 3 1/4, eighty columns, twelve rows.
  function geometry(width, height) {
    var s = width / 7.375;
    return {
      s: s,
      x: function (column) { return (0.2515 + 0.087 * column) * s; },
      y: function (row) { return (0.25 + 0.25 * row) / 3.25 * height; },
      holeW: 0.055 * s,
      holeH: 0.125 * s
    };
  }

  // The unpunched card: stock, printed digits, column numbers. Cached per size.
  function blank(width, height) {
    if (blankCard && blankCard.width === width && blankCard.height === height) return blankCard;
    var canvas = document.createElement('canvas');
    canvas.width = width; canvas.height = height;
    var ctx = canvas.getContext('2d');
    var g = geometry(width, height);
    ctx.fillStyle = palette.manila;
    ctx.fillRect(0, 0, width, height);

    var pitch = 0.087 * g.s;
    if (pitch >= 5.5) {
      ctx.fillStyle = palette.formInk;
      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';
      ctx.font = '500 ' + Math.round(0.118 * g.s) + 'px "Barlow Condensed", "Arial Narrow", sans-serif';
      for (var row = 2; row < ROWS; row++) {
        var digit = String(row - 2), y = g.y(row);
        for (var column = 0; column < COLUMNS; column++) ctx.fillText(digit, g.x(column), y);
      }
      if (pitch >= 11) {
        ctx.font = '500 ' + Math.max(6, Math.round(0.05 * g.s)) + 'px "Barlow Condensed", "Arial Narrow", sans-serif';
        var between = (g.y(2) + g.y(3)) / 2, foot = g.y(11) + 0.135 * g.s;
        for (var c = 0; c < COLUMNS; c++) {
          ctx.fillText(String(c + 1), g.x(c), between);
          ctx.fillText(String(c + 1), g.x(c), foot);
        }
      }
    } else {
      // Too small for type: print the digit rows as faint rules.
      ctx.fillStyle = palette.formInk;
      ctx.globalAlpha = 0.35;
      for (var r = 2; r < ROWS; r++) ctx.fillRect(g.x(0) - pitch / 2, g.y(r) - 0.5, pitch * COLUMNS, 1);
      ctx.globalAlpha = 1;
    }
    blankCard = canvas;
    return canvas;
  }

  function paintCard(target, size, cardIndex, mark, printed) {
    var ctx = target.getContext('2d');
    var g = geometry(size.width, size.height);
    ctx.drawImage(blank(size.width, size.height), 0, 0);

    if (filter) {
      ctx.fillStyle = palette.tray;
      var base = cardIndex * PER_CARD, limit = Math.min(PER_CARD, filter.m - base);
      for (var bit = 0; bit < limit; bit++) {
        if (!filter.bit(base + bit)) continue;
        var column = (bit / ROWS) | 0, row = bit % ROWS;
        ctx.fillRect(g.x(column) - g.holeW / 2, g.y(row) - g.holeH / 2, g.holeW, g.holeH);
      }
    }

    // The keypunch prints along the top edge.
    var printSize = Math.max(8, Math.round(0.085 * g.s));
    ctx.fillStyle = palette.ink;
    ctx.textBaseline = 'middle';
    ctx.font = '500 ' + printSize + 'px ui-monospace, "Cascadia Mono", Consolas, Menlo, monospace';
    ctx.textAlign = 'left';
    if (printed) ctx.fillText(printed.slice(0, 44).toUpperCase(), g.x(3), 0.085 * g.s);
    if (filter) {
      ctx.textAlign = 'right';
      ctx.fillText('CARD ' + pad(cardIndex + 1, 4) + ' OF ' + cardCount(), g.x(COLUMNS - 1) + g.holeW, 0.085 * g.s);
    }

    if (!mark) return;
    var mc = (mark.inCard / ROWS) | 0, mr = mark.inCard % ROWS;
    var cx = g.x(mc), cy = g.y(mr);
    var line = Math.max(2, 0.019 * g.s);
    ctx.strokeStyle = palette.pencil;
    ctx.lineWidth = line * 0.5;
    ctx.globalAlpha = 0.7;
    ctx.beginPath();
    ctx.moveTo(cx, 0); ctx.lineTo(cx, cy - g.holeH * 0.95);
    ctx.moveTo(0, cy); ctx.lineTo(cx - g.holeW * 1.9, cy);
    ctx.stroke();
    ctx.globalAlpha = 1;
    ctx.lineWidth = line;
    ctx.beginPath();
    ctx.ellipse(cx, cy, g.holeW * 1.75, g.holeH * 0.92, -0.12, 0, Math.PI * 2);
    ctx.stroke();

    var label = mark.punched ? 'PUNCHED' : 'BLANK';
    var labelSize = Math.max(11, Math.round(0.11 * g.s));
    ctx.font = '600 ' + labelSize + 'px "Barlow Condensed", "Arial Narrow", sans-serif';
    var w = ctx.measureText(label).width + labelSize * 0.9, h = labelSize * 1.5;
    var lx = cx + g.holeW * 2.4;
    if (lx + w > size.width - 4) lx = cx - g.holeW * 2.4 - w;
    var ly = Math.min(Math.max(cy - h / 2, 2), size.height - h - 2);
    ctx.fillStyle = mark.punched ? palette.ink : palette.pencil;
    ctx.fillRect(lx, ly, w, h);
    ctx.fillStyle = '#fff';
    ctx.textAlign = 'center';
    ctx.fillText(label, lx + w / 2, ly + h / 2 + 1);
  }

  // On a narrow screen the whole card would be a strip of ticks, so show a
  // window of it at reading size: thirty columns around the hole in question.
  var WINDOW_COLUMNS = 30;
  var windowCard = null;

  function drawCard(cardIndex, mark, printed) {
    var size = fit(cardCanvas);
    // Must switch with the frame proportion in style.css (max-width: 620px).
    if (!window.matchMedia('(max-width: 620px)').matches) { paintCard(cardCanvas, size, cardIndex, mark, printed); return; }
    var full = { width: Math.round(size.width * COLUMNS / WINDOW_COLUMNS), height: size.height };
    if (!windowCard) windowCard = document.createElement('canvas');
    windowCard.width = full.width; windowCard.height = full.height;
    paintCard(windowCard, full, cardIndex, mark, printed);
    var centre = mark ? geometry(full.width, full.height).x((mark.inCard / ROWS) | 0) : 0;
    var left = Math.min(Math.max(Math.round(centre - size.width / 2), 0), full.width - size.width);
    cardCanvas.getContext('2d').drawImage(windowCard, left, 0, size.width, size.height, 0, 0, size.width, size.height);
  }

  // The whole deck edge-on, with the cards this word pulled.
  function drawTray() {
    var size = fit(trayCanvas);
    var ctx = trayCanvas.getContext('2d');
    var r = size.ratio, W = size.width, H = size.height;
    ctx.clearRect(0, 0, W, H);
    var deckTop = H - 30 * r, deckHeight = 28 * r;
    ctx.fillStyle = palette.manila;
    ctx.fillRect(0, deckTop, W, deckHeight);
    ctx.fillStyle = palette.edge;
    for (var x = 0; x < W; x += 3 * r) ctx.fillRect(x, deckTop, r, deckHeight);
    if (!filter || !current) return;

    var cards = cardCount(), lastLabel = -1e9, level = 0;
    var order = current.positions.map(function (p, i) { return { i: i, card: Math.floor(p / PER_CARD) }; })
      .sort(function (a, b) { return a.card - b.card; });
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.font = '600 ' + Math.round(13 * r) + 'px "Barlow Condensed", "Arial Narrow", sans-serif';
    order.forEach(function (probe) {
      var px = Math.min(Math.max((probe.card + 0.5) / cards * W, 6 * r), W - 6 * r);
      var punched = current.set[probe.i], active = probe.i === shown;
      level = px - lastLabel < 18 * r ? (level + 1) % 2 : 0;
      lastLabel = px;
      var top = deckTop - (14 + level * 14) * r;
      ctx.fillStyle = palette.tray;
      ctx.fillRect(px - 4.5 * r, top - r, 9 * r, deckTop + deckHeight - top + r);
      ctx.fillStyle = active ? '#fff' : palette.manila;
      ctx.fillRect(px - 3 * r, top, 6 * r, deckTop + deckHeight - top);
      if (!punched) { ctx.fillStyle = palette.pencil; ctx.fillRect(px - 3 * r, top, 6 * r, 5 * r); }
      ctx.fillStyle = active ? '#fff' : palette.soft;
      ctx.fillText(String(probe.i + 1), px, top - 9 * r);
    });
  }

  /* ---------- Answering ---------- */

  function show(index, animate) {
    if (!current) return;
    shown = index;
    var place = locate(current.positions[index]);
    drawCard(place.card - 1, { inCard: place.inCard, punched: current.set[index] }, state.word);
    drawTray();
    cardCaption.innerHTML = 'Hole ' + (index + 1) + ' of ' + filter.k + ': bit <b>' + fmt(current.positions[index]) +
      '</b>, card <b>' + fmt(place.card) + '</b>, column <b>' + place.column + '</b>, row <b>' + place.row +
      '</b>. ' + (current.set[index] ? '<b>Punched.</b>' : '<b>Blank.</b>');
    Array.prototype.forEach.call(probeBody.rows, function (row, i) {
      if (i === index) row.setAttribute('aria-current', 'true'); else row.removeAttribute('aria-current');
    });
    if (animate && !(reduceMotion && reduceMotion.matches)) {
      probeFigure.classList.remove('feeding');
      void probeFigure.offsetWidth;
      probeFigure.classList.add('feeding');
    }
  }

  function csharp(word) {
    return word.replace(/[\\"\u0000-\u001f\u007f-￿]/g, function (ch) {
      if (ch === '\\' || ch === '"') return '\\' + ch;
      return '\\u' + ('0000' + ch.charCodeAt(0).toString(16)).slice(-4);
    });
  }

  var state = { word: '' };

  function answer(raw, feed) {
    if (!filter) return;
    var word = raw.trim().toLowerCase();
    state.word = word;
    feedToken++;

    if (!word) {
      current = null;
      verdict.textContent = 'Type a word.';
      reason.textContent = 'Any word, in any language. The deck only knows English, and only ' + fmt(filter.n) + ' words of it.';
      probeBody.textContent = '';
      probeCaption.textContent = 'The seven holes';
      cardCaption.textContent = 'Card 1 of ' + fmt(cardCount()) + ', as punched. About half of every card is holes.';
      reproduce.textContent = '';
      lamp.textContent = ''; lamp.className = 'lamp';
      drawCard(0, null, '');
      drawTray();
      syncAddress('');
      return;
    }

    current = filter.probe(word);
    var decisive = current.present ? filter.k - 1 : current.firstBlank;
    var place = locate(current.positions[decisive]);
    if (current.present) {
      verdict.textContent = 'Probably in the list.';
      reason.innerHTML = 'All ' + filter.k + ' holes for <strong>' + escapeHtml(quote(word)) + '</strong> are punched. ' +
        'Either it is one of the ' + fmt(filter.n) + ' words, or it is one of the roughly 1 in 100 strangers that find ' +
        filter.k + ' punched holes by luck. The deck cannot tell you which.';
    } else {
      var blanks = current.set.filter(function (s) { return !s; }).length;
      verdict.textContent = 'Definitely not in the list.';
      reason.innerHTML = 'Hole ' + (decisive + 1) + ' of ' + filter.k + ' for <strong>' + escapeHtml(quote(word)) +
        '</strong> is blank: card ' + fmt(place.card) + ', column ' + place.column + ', row ' + place.row + '. ' +
        'One blank hole is proof. ' + (blanks > 1 ? blanks + ' of its holes are blank; the reader stops at the first.' :
        'Every other hole is punched, and it still is not there.');
    }

    lamp.textContent = current.present ? 'Probably in the list' : 'Definitely not in the list';
    lamp.className = 'lamp ' + (current.present ? 'yes' : 'no');
    probeCaption.textContent = 'The ' + filter.k + ' holes for ' + quote(word);
    probeBody.textContent = '';
    current.positions.forEach(function (position, i) {
      var at = locate(position), row = probeBody.insertRow();
      var cells = [String(i + 1), fmt(position), fmt(at.card), String(at.column), at.row];
      cells.forEach(function (text, c) {
        var cell = c === 0 ? document.createElement('th') : document.createElement('td');
        if (c === 0) {
          cell.scope = 'row';
          var button = document.createElement('button');
          button.type = 'button';
          button.className = 'recall';
          button.textContent = text;
          button.setAttribute('aria-label', 'Show the card for hole ' + (i + 1));
          button.addEventListener('click', function () { feedToken++; show(i, true); });
          cell.appendChild(button);
        } else cell.textContent = text;
        row.appendChild(cell);
      });
      var stateCell = row.insertCell();
      stateCell.className = 'state' + (current.set[i] ? '' : ' blank');
      stateCell.textContent = current.set[i] ? 'punched' : 'BLANK';
    });

    reproduce.innerHTML =
      'var filter = FilterBuilder.Build(' + String(filter.n).replace(/\B(?=(\d{3})+(?!\d))/g, '_') + ', 0.01);\n' +
      'int[] holes = filter.ComputeHash("' + escapeHtml(csharp(word)) + '");\n' +
      '<span class="muted">// ' + current.positions.join(', ') + '</span>';

    syncAddress(word);
    if (feed && !(reduceMotion && reduceMotion.matches) && decisive > 0) {
      var token = feedToken, step = 0;
      (function next() {
        if (token !== feedToken) return;
        show(step, true);
        if (step++ < decisive) setTimeout(next, 210);
      })();
    } else show(decisive, false);
  }

  function escapeHtml(text) {
    return text.replace(/[&<>"]/g, function (ch) { return { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' }[ch]; });
  }

  var addressTimer = 0;
  function syncAddress(word) {
    clearTimeout(addressTimer);
    addressTimer = setTimeout(function () {
      try {
        var url = new URL(window.location.href);
        if (word) url.searchParams.set('w', word); else url.searchParams.delete('w');
        history.replaceState(null, '', url);
      } catch (error) { /* file:// or a locked-down frame: the address simply stays put */ }
    }, 400);
  }

  /* ---------- Start-up ---------- */

  function fill(name, value) {
    Array.prototype.forEach.call(document.querySelectorAll('[data-stat="' + name + '"]'), function (node) { node.textContent = value; });
  }

  function selfCheck() {
    return VECTORS.every(function (vector) {
      var got = Bloom.positions(vector.word, filter.m, filter.k).positions;
      return got.length === vector.positions.length && got.every(function (p, i) { return p === vector.positions[i]; }) &&
        filter.contains(vector.word) === vector.present;
    }) && VECTORS.length > 0;
  }

  function outOfOrder(title, detail) {
    filter = null; current = null;
    verdict.textContent = title;
    reason.textContent = detail;
    document.documentElement.classList.add('reader-down');
    input.disabled = true;
    form.querySelector('button').disabled = true;
    var run = $('run');
    if (run) run.disabled = true;
    drawCard(0, null, '');
    drawTray();
  }

  function start(snapshot) {
    try { filter = Bloom.load(snapshot); }
    catch (error) { outOfOrder('The deck did not load.', error.message + ' The reference below does not depend on it.'); return; }
    if (!selfCheck()) {
      outOfOrder('The reader is out of order.',
        'In this browser the hash does not reproduce the C# library’s test vectors, so no answers are shown. The reference below is unaffected.');
      return;
    }

    var setBits = filter.countSetBits(), ratio = setBits / filter.m;
    fill('words', fmt(filter.n));
    fill('bits', fmt(filter.m));
    fill('hashes', String(filter.k));
    fill('cards', fmt(cardCount()));
    fill('kilobytes', fmt(Math.ceil(filter.m / 8) / 1000));
    fill('fill', (ratio * 100).toFixed(1) + '%');
    fill('estimate', (Math.pow(ratio, filter.k) * 100).toFixed(2) + '%');
    fill('deck-height', height(cardCount()));

    var asked = '';
    try { asked = new URL(window.location.href).searchParams.get('w') || ''; } catch (error) { asked = ''; }
    input.value = asked || input.value || 'bloom';
    answer(input.value, false);
    document.documentElement.classList.add('reader-ready');
  }

  function height(cards) {
    var inches = cards * CARD_THICKNESS_INCHES;
    if (inches < 24) return inches.toFixed(1) + ' inches';
    var feet = Math.floor(inches / 12), rest = Math.round(inches - feet * 12);
    if (rest === 12) { feet++; rest = 0; }
    if (feet >= 5280) return (inches / 63360).toFixed(1) + ' miles';
    return fmt(feet) + ' ft' + (rest ? ' ' + rest + ' in' : '');
  }

  form.addEventListener('submit', function (event) {
    event.preventDefault();
    answer(input.value, true);
    // On a small screen the verdict is below the fold: bring it into view.
    var box = verdict.getBoundingClientRect();
    if (box.bottom > window.innerHeight || box.top < 0) {
      input.blur();
      verdict.scrollIntoView({ behavior: reduceMotion && reduceMotion.matches ? 'auto' : 'smooth', block: 'start' });
    }
  });
  input.addEventListener('input', function () { answer(input.value, false); });

  var resizeTimer = 0;
  window.addEventListener('resize', function () {
    clearTimeout(resizeTimer);
    resizeTimer = setTimeout(function () {
      blankCard = null;
      if (current) show(shown, false); else { drawCard(0, null, ''); drawTray(); }
    }, 120);
  });

  verdict.textContent = 'Loading the deck…';
  drawCard(0, null, '');
  drawTray();

  fetch('words.json').then(function (response) {
    if (!response.ok) throw new Error('words.json returned ' + response.status + '.');
    return response.json();
  }).then(start).catch(function (error) {
    outOfOrder('The deck did not load.', (error && error.message ? error.message + ' ' : '') + 'The reference below does not depend on it.');
  });

  if (document.fonts && document.fonts.ready) {
    document.fonts.ready.then(function () {
      blankCard = null;
      if (current) show(shown, false); else if (filter) { drawCard(0, null, ''); drawTray(); }
    });
  }

  /* ---------- Feed it nonsense ---------- */

  var runButton = $('run'), tallyBody = $('tally-rows'), passedList = $('passed'), passedNote = $('passed-note');
  var totals = { runs: 0, read: 0, passed: 0 };

  function tally(read, passed, done) {
    var rows = [
      ['Read', fmt(read)],
      ['Turned away', fmt(read - passed)],
      ['Let through', fmt(passed)],
      ['Rate', read ? (passed / read * 100).toFixed(2) + '%' : ''],
      ['Sized for', (filter.p * 100).toFixed(2) + '%']
    ];
    if (done && totals.runs > 1) {
      rows.push(['All ' + totals.runs + ' runs', (totals.passed / totals.read * 100).toFixed(2) + '% of ' + fmt(totals.read)]);
    }
    tallyBody.textContent = '';
    rows.forEach(function (cells) {
      var row = tallyBody.insertRow();
      var head = document.createElement('th'); head.scope = 'row'; head.textContent = cells[0]; row.appendChild(head);
      row.insertCell().textContent = cells[1];
    });
  }

  function chips(words) {
    passedList.textContent = '';
    words.slice(0, 48).forEach(function (word) {
      var item = document.createElement('li'), button = document.createElement('button');
      button.type = 'button'; button.className = 'chip'; button.textContent = word;
      button.setAttribute('aria-label', 'Read the cards for ' + word);
      button.addEventListener('click', function () {
        input.value = word;
        answer(word, true);
        verdict.scrollIntoView({ behavior: reduceMotion && reduceMotion.matches ? 'auto' : 'smooth', block: 'start' });
        input.focus({ preventScroll: true });
      });
      item.appendChild(button); passedList.appendChild(item);
    });
    var tail = ' None is a word, and the deck says “probably” to every one. Press one to see its seven punched holes.';
    passedNote.textContent = words.length === 0 ? 'None got through this time. That is rare; run it again.' :
      words.length > 48 ? fmt(words.length) + ' got through. Here are 48 of them.' + tail :
      'These ' + words.length + ' got through.' + tail;
  }

  if (runButton) {
    runButton.addEventListener('click', function () {
      if (!filter) return;
      var total = 10000, read = 0, passed = [], batch = reduceMotion && reduceMotion.matches ? total : 400;
      runButton.disabled = true;
      (function step() {
        for (var i = 0; i < batch && read < total; i++, read++) {
          var word = window.Nonsense.make(Math.random);
          if (filter.contains(word)) passed.push(word);
        }
        if (read < total) { tally(read, passed.length, false); requestAnimationFrame(step); return; }
        totals.runs++; totals.read += read; totals.passed += passed.length;
        tally(read, passed.length, true);
        chips(passed);
        runButton.disabled = false;
        runButton.textContent = 'Feed it 10,000 more';
      })();
    });
  }

  /* ---------- Size your own ---------- */

  var keysInput = $('keys'), rateInput = $('rate'), sizedBody = $('sized-rows'), overfillBody = $('overfill-rows');
  var sizedCode = $('sized-code'), sizedNote = $('sized-note');
  var INT_MAX = 2147483647;

  function bytes(count) {
    if (count < 1000) return fmt(count) + ' bytes';
    if (count < 1e6) return (count / 1e3).toFixed(1) + ' kB';
    if (count < 1e9) return (count / 1e6).toFixed(2) + ' MB';
    return (count / 1e9).toFixed(2) + ' GB';
  }

  function percent(p) {
    var value = p * 100;
    if (value >= 99.95) return '100%';
    if (value >= 10) return value.toFixed(0) + '%';
    if (value >= 1) return value.toFixed(1) + '%';
    return value.toPrecision(2) + '%';
  }

  function rows(body, list) {
    body.textContent = '';
    list.forEach(function (cells) {
      var row = body.insertRow();
      var head = document.createElement('th'); head.scope = 'row'; head.textContent = cells[0]; row.appendChild(head);
      row.insertCell().textContent = cells[1];
    });
  }

  function resize() {
    if (!keysInput) return;
    var n = Number(String(keysInput.value).replace(/[,_\s]/g, '')), p = Number(rateInput.value);
    var valid = Number.isInteger(n) && n >= 1;
    if (!valid || n > INT_MAX) {
      rows(sizedBody, []); rows(overfillBody, []);
      sizedCode.textContent = '';
      sizedNote.textContent = valid ? 'FilterBuilder.Build takes an Int32 count, so one filter holds at most 2,147,483,647 keys.' :
        'Enter a whole number of keys, 1 or more.';
      return;
    }
    var sized = Bloom.size(n, p);
    if (!sized) {
      rows(sizedBody, []); rows(overfillBody, []);
      sizedCode.textContent = '';
      sizedNote.textContent = 'That needs more than 2,147,483,647 bits. The library throws ArgumentOutOfRangeException rather than quietly building a smaller filter. Split the keys across several filters.';
      return;
    }
    var cards = Math.ceil(sized.m / PER_CARD);
    rows(sizedBody, [
      ['Bits (m)', fmt(sized.m)],
      ['Bits per key', (sized.m / n).toFixed(2)],
      ['Hashes (k)', String(sized.k)],
      ['Memory', bytes(Math.ceil(sized.m / 8))],
      ['As punched cards', fmt(cards)],
      ['Stacked', height(cards)]
    ]);
    rows(overfillBody, [[0.5, 'Half as many keys as planned'], [1, 'As planned'], [1.5, 'Half again as many'], [2, 'Twice as many'], [3, 'Three times as many']]
      .map(function (step) { return [step[1], percent(Bloom.falsePositiveRate(sized.k, sized.m, n * step[0]))]; }));
    sizedCode.textContent = 'var filter = FilterBuilder.Build(' + String(n).replace(/\B(?=(\d{3})+(?!\d))/g, '_') + ', ' + rateInput.value + ');';
    sizedNote.textContent = 'Each key costs ' + (sized.m / n).toFixed(1) + ' bits whether it is four bytes or four kilobytes.';
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
      var text = button.getAttribute('data-copy') === 'link' ? window.location.href : button.getAttribute('data-copy');
      var done = function () { if (status) status.textContent = 'Copied.'; };
      var failed = function () { if (status) status.textContent = 'Copying is blocked here. Select the text instead.'; };
      if (navigator.clipboard && navigator.clipboard.writeText) navigator.clipboard.writeText(text).then(done, failed);
      else failed();
      if (status) setTimeout(function () { status.textContent = ''; }, 4000);
    });
  });
})();
