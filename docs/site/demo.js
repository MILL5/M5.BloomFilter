'use strict';
(() => {
  const grid = document.getElementById('bit-grid');
  const keyInput = document.getElementById('demo-key');
  const result = document.getElementById('demo-result');
  const occupancy = document.getElementById('occupancy');
  const addButton = document.getElementById('add-key');
  const resetButton = document.getElementById('reset-demo');
  const bits = new Array(64).fill(false);
  const cells = bits.map((_, i) => {
    const cell = document.createElement('span');
    cell.className = 'bit';
    cell.setAttribute('aria-hidden', 'true');
    cell.textContent = '0';
    cell.title = `Bit ${i}: 0`;
    grid.appendChild(cell);
    return cell;
  });
  // Teaching hash only. Not the library's Murmur3KirschMitzenmacher.
  function positions(key) {
    return [17, 31, 47].map(seed => {
      let hash = seed;
      for (const byte of new TextEncoder().encode(key)) hash = (Math.imul(hash, 33) ^ byte) >>> 0;
      return hash % 64;
    });
  }
  function draw(selected = []) {
    cells.forEach((cell, i) => {
      cell.className = `bit${bits[i] ? ' set' : ''}${selected.includes(i) ? ' selected' : ''}`;
      cell.textContent = bits[i] ? '1' : '0';
      cell.title = `Bit ${i}: ${bits[i] ? 1 : 0}${selected.includes(i) ? ', selected' : ''}`;
    });
    const occupied = bits.flatMap((set, i) => set ? [i] : []);
    occupancy.textContent = `${occupied.length} / 64 bits set`;
    resetButton.disabled = occupied.length === 0;
    grid.setAttribute('aria-label', `Illustrative 64-bit Bloom filter. Set positions: ${occupied.length ? occupied.join(', ') : 'none'}. Selected positions: ${selected.length ? selected.map(i => `${i} (${bits[i] ? 'set' : 'unset'})`).join(', ') : 'none'}.`);
  }
  function announce(state, message) {
    result.dataset.state = state;
    result.textContent = message;
  }
  function readKey() {
    if (!keyInput.reportValidity()) return null;
    return keyInput.value;
  }
  keyInput.addEventListener('input', () => {
    addButton.disabled = keyInput.value.length === 0;
    draw();
    announce('ready', keyInput.value.length ? 'Check this key or add it to the filter.' : 'Enter a key to check or add.');
  });
  document.getElementById('demo-form').addEventListener('submit', event => {
    event.preventDefault();
    const key = readKey();
    if (key === null) return;
    const selected = positions(key);
    draw(selected);
    if (selected.every(i => bits[i])) {
      announce('possible', `“${key}” is possibly present. All selected bits are set; confirm with the original set.`);
    } else {
      announce('absent', `“${key}” is definitely absent. At least one selected bit is not set.`);
    }
  });
  addButton.addEventListener('click', () => {
    const key = readKey();
    if (key === null) return;
    const selected = positions(key);
    selected.forEach(i => { bits[i] = true; });
    draw(selected);
    announce('added', `Added “${key}”. Its bit positions are ${selected.join(', ')}. Other keys can share these bits.`);
  });
  resetButton.addEventListener('click', () => {
    bits.fill(false);
    draw();
    announce('empty', 'Filter cleared. Check a key to see which bits it needs.');
  });
  document.getElementById('copy-install').addEventListener('click', async event => {
    const button = event.currentTarget;
    const status = document.getElementById('copy-status');
    button.disabled = true;
    button.textContent = 'Copying…';
    try {
      await navigator.clipboard.writeText(document.querySelector('.install code').textContent);
      button.textContent = 'Copied';
      status.textContent = 'Install command copied.';
    } catch {
      button.textContent = 'Copy';
      const range = document.createRange();
      range.selectNodeContents(document.querySelector('.install code'));
      const selection = window.getSelection();
      selection.removeAllRanges();
      selection.addRange(range);
      status.textContent = 'Clipboard unavailable. The command is selected; use your device’s copy action.';
    } finally {
      button.disabled = false;
    }
  });
  draw();
})();
