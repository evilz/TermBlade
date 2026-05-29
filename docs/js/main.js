// TermBlade docs — mobile nav toggle
document.addEventListener('DOMContentLoaded', () => {
  const toggle = document.querySelector('.nav-toggle');
  const links = document.querySelector('.nav-links');

  if (toggle && links) {
    toggle.addEventListener('click', () => {
      const expanded = links.style.display === 'flex';
      links.style.display = expanded ? 'none' : 'flex';
      links.style.flexDirection = 'column';
      links.style.position = 'absolute';
      links.style.top = '60px';
      links.style.right = '1rem';
      links.style.background = 'var(--bg-secondary)';
      links.style.padding = '1rem';
      links.style.borderRadius = 'var(--radius)';
      links.style.border = '1px solid var(--border)';
    });
  }

  // Smooth scroll for anchor links
  document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', (e) => {
      const target = document.querySelector(anchor.getAttribute('href'));
      if (target) {
        e.preventDefault();
        target.scrollIntoView({ behavior: 'smooth', block: 'start' });
        // Close mobile nav
        if (links && window.innerWidth <= 768) {
          links.style.display = 'none';
        }
      }
    });
  });
});
