(function (bootstrap) {
    document.querySelectorAll('[data-js-remove]').forEach(el => {
        el.remove();
    });    

    // tooltips
    document.addEventListener('mouseover', e => {
        const target = e.target;        

        const text = target.getAttribute('data-tooltip');
        if (!text) {
            return;
        }

        const hasTooltip = target.getAttribute('data-tooltip-ready');
        if (hasTooltip) {
            return;
        }
        
        const tooltip = new bootstrap.Tooltip(target, {
            title: text,
            trigger: 'hover'
        });

        tooltip.show();

        target.setAttribute('data-tooltip-ready', 'true');
    });    
})(bootstrap);