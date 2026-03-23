// ─── Counter animation via IntersectionObserver — zero forced reflow ────────
const counterObserver = new IntersectionObserver(entries => {
    entries.forEach(entry => {
        if (!entry.isIntersecting) return;

        const el = entry.target;
        // Read target value ONCE before any writes — avoids read-after-write reflow
        const target = parseFloat(el.dataset.countTarget) || 0;
        const hasPlus = el.classList.contains("plus");
        const duration = 2000;
        const start = performance.now();

        function tick(now) {
            const progress = Math.min((now - start) / duration, 1);
            // easeOutQuad — feels more natural than linear
            const eased = 1 - (1 - progress) * (1 - progress);
            el.textContent = Math.ceil(eased * target) + (progress >= 1 && hasPlus ? "+" : "");
            if (progress < 1) requestAnimationFrame(tick);
        }

        requestAnimationFrame(tick);
        counterObserver.unobserve(el);
    });
});

// Batch all DOM reads before observer starts — no read-after-write
document.querySelectorAll(".count").forEach(el => {
    // Store target in data attribute — avoids reading textContent after any write
    el.dataset.countTarget = el.textContent.trim().replace("+", "");
    counterObserver.observe(el);
});

// ─── Pricing responsive fix — use ResizeObserver, zero forced reflow ─────────
// Old: $(window).width() — reads offsetWidth = forced reflow
// New: ResizeObserver — browser pushes size to us, no JS geometry read needed
const pricingList = document.querySelector("#pricing .container ul");

if (pricingList) {
    const resizeObserver = new ResizeObserver(entries => {
        // entries[0].contentRect.width is provided by browser — no reflow
        const width = entries[0].contentRect.width;
        if (width < 514) {
            pricingList.classList.remove("three");
        } else {
            pricingList.classList.add("three");
        }
    });
    resizeObserver.observe(document.body);
}

// ─── Search helpers ───────────────────────────────────────────────────────────
function SimpleSearch() {
    const q = document.getElementById("q");
    if (q) window.location.href = "/simplesearch?q=" + encodeURIComponent(q.value);
}

function KeyUp(e) {
    if (e.keyCode === 13) SimpleSearch();
}