(function () {
    // apply hover state to linked elements
    document.addEventListener('mouseover', e => {
        const target = e.target;
        const hoverId = target.getAttribute('data-hover');
        if (!hoverId) {
            return;
        }

        const elements = document.querySelectorAll('[data-hover="' + hoverId + '"]');
        elements.forEach(el => el.classList.add('hover'));
    });

    document.addEventListener('mouseout', e => {
        const target = e.target;
        const hoverId = target.getAttribute('data-hover');
        if (!hoverId) {
            return;
        }

        const elements = document.querySelectorAll('[data-hover="' + hoverId + '"]');
        elements.forEach(el => el.classList.remove('hover'));
    });
})();