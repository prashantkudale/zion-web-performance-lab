function SimpleSearch() {
    const q = document.getElementById("q");
    if (q) window.location.href = "/simplesearch?q=" + encodeURIComponent(q.value);
}

function KeyUp(e) {
    if (e.keyCode === 13) SimpleSearch();
}