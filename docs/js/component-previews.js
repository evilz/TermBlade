// Replaces static preview bodies with the .NET 10 WASM + xterm.js TermBlade preview app.
document.addEventListener('DOMContentLoaded', async () => {
  let wasmAvailable = false;
  try {
    const response = await fetch('wasm/index.html', { method: 'HEAD' });
    wasmAvailable = response.ok;
  } catch {
    wasmAvailable = false;
  }

  if (!wasmAvailable) return;

  document.querySelectorAll('.component-preview').forEach((preview) => {
    const title = preview.querySelector('.terminal-title');
    const body = preview.querySelector('.terminal-body');
    if (!title || !body) return;

    const component = title.textContent.trim().replace(/-preview$/, '');
    if (!component) return;

    const iframe = document.createElement('iframe');
    iframe.className = 'component-preview-frame';
    iframe.title = `${component} live TermBlade preview`;
    iframe.loading = 'lazy';
    iframe.src = `wasm/?preview=${encodeURIComponent(component)}`;

    body.textContent = '';
    body.appendChild(iframe);
    preview.classList.add('component-preview-live');
  });
});
