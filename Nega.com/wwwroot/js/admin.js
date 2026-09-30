// NEGA admin panel – vanilla JS helpers
(function () {
  "use strict";

  // Dropdowns (notifications, user menu)
  document.addEventListener("click", e => {
    const trigger = e.target.closest("[data-dropdown]");
    document.querySelectorAll(".dropdown.open").forEach(d => { if (!trigger || d !== trigger.parentElement) d.classList.remove("open"); });
    if (trigger) trigger.parentElement.classList.toggle("open");
    if (!e.target.closest(".sidebar") && !e.target.closest(".menu-toggle")) document.body.classList.remove("nav-open");
  });

  // Confirm before delete
  document.querySelectorAll("form[data-confirm]").forEach(f => f.addEventListener("submit", e => {
    if (!confirm(f.dataset.confirm || "Wirklich löschen?")) e.preventDefault();
  }));

  // Image preview before upload
  document.querySelectorAll("[data-image-input]").forEach(input => input.addEventListener("change", () => {
    const box = input.closest(".image-field").querySelector("[data-preview]");
    const file = input.files && input.files[0];
    if (!file || !box) return;
    const img = document.createElement("img");
    img.src = URL.createObjectURL(file);
    box.innerHTML = "";
    box.appendChild(img);
  }));

  // Character counters
  document.querySelectorAll("[data-count]").forEach(el => {
    const out = document.getElementById(el.dataset.count);
    const max = el.getAttribute("maxlength");
    const upd = () => out.textContent = el.value.length + (max ? " / " + max : "") + " Zeichen";
    el.addEventListener("input", upd); upd();
  });

  // Label every table cell with its column header (used by the mobile card layout)
  document.querySelectorAll("table.table").forEach(t => {
    const heads = [...t.querySelectorAll("thead th")].map(th => th.textContent.trim());
    t.querySelectorAll("tbody tr").forEach(tr => [...tr.children].forEach((td, i) => td.setAttribute("data-label", heads[i] || "")));
  });

  // Auto-hide success alerts
  setTimeout(() => document.querySelectorAll(".content > .alert-ok").forEach(a => { a.style.transition = "opacity .5s"; a.style.opacity = "0"; setTimeout(() => a.remove(), 500); }), 4500);
})();
