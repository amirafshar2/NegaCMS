// NEGA public website – small vanilla JS helpers (no external libraries)
(function () {
  "use strict";

  // Mobile navigation
  const toggle = document.querySelector(".nav-toggle");
  const links = document.getElementById("navLinks");
  if (toggle && links) {
    toggle.addEventListener("click", () => {
      const open = links.classList.toggle("open");
      toggle.setAttribute("aria-expanded", open);
    });
    links.querySelectorAll("a").forEach(a => a.addEventListener("click", () => links.classList.remove("open")));
  }

  // Reveal on scroll + counters
  const counters = el => {
    el.querySelectorAll(".count").forEach(c => {
      const to = parseInt(c.dataset.to, 10) || 0, start = performance.now(), dur = 1400;
      const fmt = n => n.toLocaleString("de-DE");
      const step = t => {
        const p = Math.min(1, (t - start) / dur);
        c.textContent = fmt(Math.round(to * (1 - Math.pow(1 - p, 3))));
        if (p < 1) requestAnimationFrame(step);
      };
      requestAnimationFrame(step);
    });
  };
  if ("IntersectionObserver" in window) {
    const io = new IntersectionObserver(entries => entries.forEach(e => {
      if (e.isIntersecting) { e.target.classList.add("visible"); counters(e.target); io.unobserve(e.target); }
    }), { threshold: 0.12 });
    document.querySelectorAll(".reveal").forEach(el => io.observe(el));
  } else {
    document.querySelectorAll(".reveal").forEach(el => { el.classList.add("visible"); counters(el); });
  }

  // Portfolio filter
  document.querySelectorAll(".filters button").forEach(btn => btn.addEventListener("click", () => {
    document.querySelectorAll(".filters button").forEach(b => b.classList.remove("active"));
    btn.classList.add("active");
    const f = btn.dataset.filter;
    document.querySelectorAll("#workGrid .work").forEach(w => w.classList.toggle("hide", f !== "all" && w.dataset.cat !== f));
  }));

  // YouTube: load only after click (2-click solution for DSGVO)
  document.querySelectorAll(".video-box[data-yt]").forEach(box => {
    const btn = box.querySelector(".play");
    if (!btn || !box.dataset.yt) return;
    btn.addEventListener("click", () => {
      const f = document.createElement("iframe");
      f.src = "https://www.youtube-nocookie.com/embed/" + box.dataset.yt + "?autoplay=1&rel=0";
      f.allow = "autoplay; encrypted-media; picture-in-picture";
      f.allowFullscreen = true;
      f.title = "Video";
      box.innerHTML = "";
      box.appendChild(f);
    });
  });

  // AJAX forms (contact + newsletter) with graceful fallback
  document.querySelectorAll("form.js-ajax").forEach(form => form.addEventListener("submit", async ev => {
    if (!form.checkValidity()) { form.reportValidity(); ev.preventDefault(); return; }
    ev.preventDefault();
    const out = document.getElementById(form.dataset.result);
    const btn = form.querySelector("[type=submit]");
    btn.disabled = true;
    try {
      const res = await fetch(form.action, { method: "POST", body: new FormData(form), headers: { "X-Requested-With": "XMLHttpRequest" } });
      const data = await res.json();
      out.innerHTML = "";
      const div = document.createElement("div");
      div.className = "alert " + (data.success ? "alert-ok" : "alert-err");
      div.setAttribute("role", "status");
      div.innerHTML = '<i class="fas ' + (data.success ? "fa-check-circle" : "fa-exclamation-circle") + '"></i><span></span>';
      div.querySelector("span").textContent = data.message;
      out.appendChild(div);
      if (data.success) form.reset();
    } catch (e) {
      form.submit();
    } finally { btn.disabled = false; }
  }));

  // Back to top
  const top = document.querySelector(".to-top");
  if (top) {
    window.addEventListener("scroll", () => top.classList.toggle("show", window.scrollY > 600), { passive: true });
    top.addEventListener("click", () => window.scrollTo({ top: 0, behavior: "smooth" }));
  }
})();
