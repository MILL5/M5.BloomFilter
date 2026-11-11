/*
 * Draws every bit of a filter as one image: row by row, bit 0 at the top left.
 *
 * The canvas is given exactly one pixel per device pixel, and each pixel is the
 * area-weighted share of the bits it covers that are on. Letting the browser scale
 * a one-pixel-per-bit image instead leaves beat patterns across the noise.
 */
(function (root) {
  'use strict';

  function channels(hex) {
    var value = parseInt(hex.trim().replace('#', ''), 16);
    return [value >>> 16, (value >>> 8) & 0xff, value & 0xff];
  }

  // For each output pixel: the first source index it covers and the weight of each one.
  function spans(side, size) {
    var scale = side / size, first = new Int32Array(size), count = new Int32Array(size), weights = [];
    for (var x = 0; x < size; x++) {
      var from = x * scale, to = from + scale, start = Math.floor(from), end = Math.min(side - 1, Math.ceil(to) - 1);
      first[x] = start; count[x] = end - start + 1;
      for (var i = start; i <= end; i++) weights.push((Math.min(to, i + 1) - Math.max(from, i)) / scale);
    }
    return { first: first, count: count, weights: new Float32Array(weights) };
  }

  // The grid of bits for a picture `aspect` times as wide as it is tall: 1 is a square.
  function shape(filter, aspect) {
    var cols = Math.ceil(Math.sqrt(filter.m * (aspect || 1)));
    return { cols: cols, rows: Math.ceil(filter.m / cols) };
  }

  function paint(filter, canvas, grid, width, height, onColor, offColor) {
    var mapX = spans(grid.cols, width), mapY = spans(grid.rows, height), x, y, i, w, sum;
    var across = new Float32Array(grid.rows * width);
    for (y = 0; y < grid.rows; y++) {
      var row = y * grid.cols;
      for (x = 0, w = 0; x < width; x++) {
        for (sum = 0, i = 0; i < mapX.count[x]; i++, w++) {
          var position = row + mapX.first[x] + i;
          if (position < filter.m && filter.bit(position)) sum += mapX.weights[w];
        }
        across[y * width + x] = sum;
      }
    }
    canvas.width = width; canvas.height = height;
    var ctx = canvas.getContext('2d'), image = ctx.createImageData(width, height), data = image.data;
    var on = channels(onColor), off = channels(offColor);
    for (x = 0; x < width; x++) {
      for (y = 0, w = 0; y < height; y++) {
        for (sum = 0, i = 0; i < mapY.count[y]; i++, w++) sum += mapY.weights[w] * across[(mapY.first[y] + i) * width + x];
        var at = (y * width + x) * 4;
        data[at] = off[0] + (on[0] - off[0]) * sum;
        data[at + 1] = off[1] + (on[1] - off[1]) * sum;
        data[at + 2] = off[2] + (on[2] - off[2]) * sum;
        data[at + 3] = 255;
      }
    }
    ctx.putImageData(image, 0, 0);
  }

  root.BitField = { shape: shape, paint: paint };
})(typeof self !== 'undefined' ? self : this);
