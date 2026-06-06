// TermBlade docs — mobile nav toggle + sidebar toggle
document.addEventListener('DOMContentLoaded', () => {
  const toggle = document.querySelector('.nav-toggle');
  const links = document.querySelector('.nav-links');

  if (toggle && links) {
    toggle.setAttribute('aria-expanded', 'false');

    toggle.addEventListener('click', (e) => {
      e.stopPropagation();
      const isOpen = links.classList.toggle('open');
      toggle.classList.toggle('open', isOpen);
      toggle.setAttribute('aria-expanded', String(isOpen));
    });

    // Close when clicking outside the nav
    document.addEventListener('click', (e) => {
      if (!toggle.contains(e.target) && !links.contains(e.target)) {
        links.classList.remove('open');
        toggle.classList.remove('open');
        toggle.setAttribute('aria-expanded', 'false');
      }
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
          links.classList.remove('open');
          toggle.classList.remove('open');
          toggle.setAttribute('aria-expanded', 'false');
        }
      }
    });
  });

  // Collapsible sidebar on components page
  const sidebarToggle = document.querySelector('.sidebar-toggle');
  const sidebarBody = document.querySelector('.sidebar-body');

  if (sidebarToggle && sidebarBody) {
    sidebarToggle.setAttribute('aria-expanded', 'false');

    sidebarToggle.addEventListener('click', () => {
      const isOpen = sidebarBody.classList.toggle('open');
      sidebarToggle.classList.toggle('open', isOpen);
      sidebarToggle.setAttribute('aria-expanded', String(isOpen));
    });

    // Close sidebar after navigating to a section on mobile
    sidebarBody.querySelectorAll('a[href^="#"]').forEach(link => {
      link.addEventListener('click', () => {
        if (window.innerWidth <= 900) {
          sidebarBody.classList.remove('open');
          sidebarToggle.classList.remove('open');
          sidebarToggle.setAttribute('aria-expanded', 'false');
        }
      });
    });
  }

  // Highlight active sidebar link on scroll (components page)
  const sections = document.querySelectorAll('.component-section[id]');
  if (sections.length) {
    const observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          document.querySelectorAll('.sidebar-group a').forEach(a => a.classList.remove('active'));
          const active = document.querySelector(`.sidebar-group a[href="#${entry.target.id}"]`);
          if (active) active.classList.add('active');
        }
      });
    }, { rootMargin: '-70px 0px -60% 0px' });

    sections.forEach(s => observer.observe(s));
  }
});
