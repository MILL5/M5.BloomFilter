'use strict';
(() => {
  const grid = document.getElementById('bit-grid');
  const keyInput = document.getElementById('demo-key');
  const result = document.getElementById('demo-result');
  const occupancy = document.getElementById('occupancy');
  const bits = new Array(64).fill(false);
  const cells = bits.map((_, i) => {
    const cell = document.createElement('span');
    cell.className = 'bit';
    cell.setAttribute('aria-hidden', 'true');
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
      cell.title = `Bit ${i}: ${bits[i] ? 1 : 0}`;
    });
    const count = bits.filter(Boolean).length;
    occupancy.textContent = `${count} / 64 bits set`;
    grid.setAttribute('aria-label', `Illustrative 64-bit Bloom filter: ${count} bits set. Selected positions: ${selected.length ? selected.join(', ') : 'none'}.`);
  }
  function readKey() {
    if (!keyInput.reportValidity()) return null;
    return keyInput.value;
  }
  document.getElementById('demo-form').addEventListener('submit', event => {
    event.preventDefault();
    const key = readKey();
    if (key === null) return;
    const selected = positions(key);
    draw(selected);
    result.textContent = bits.length && selected.every(i => bits[i])
      ? `“${key}” is possibly present. All selected bits are set; confirm with the original set.`
      : `“${key}” is definitely absent. At least one selected bit is not set.`;
  });
  document.getElementById('add-key').addEventListener('click', () => {
    const key = readKey();
    if (key === null) return;
    const selected = positions(key);
    selected.forEach(i => { bits[i] = true; });
    draw(selected);
    result.textContent = `Added “${key}”. Its bit positions are ${selected.join(', ')}. Other keys can share these bits.`;
  });
  document.getElementById('reset-demo').addEventListener('click', () => {
    bits.fill(false);
    draw();
    result.textContent = 'Filter cleared. Check a key to see which bits it needs.';
  });
  document.getElementById('copy-install').addEventListener('click', async event => {
    const button = event.currentTarget;
    try {
      await navigator.clipboard.writeText('dotnet add package M5.BloomFilter');
      button.textContent = 'Copied';
    } catch {
      button.textContent = 'Select command';
      const range = document.createRange();
      range.selectNodeContents(document.querySelector('.install code'));
      const selection = window.getSelection();
      selection.removeAllRanges();
      selection.addRange(range);
    }
  });
  draw();
})();
