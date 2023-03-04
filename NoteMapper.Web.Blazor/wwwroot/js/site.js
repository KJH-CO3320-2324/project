const cos = Math.cos;
const sin = Math.sin;
const π = Math.PI;

const f_matrix_times = (([[a, b], [c, d]], [x, y]) => [a * x + b * y, c * x + d * y]);
const f_rotate_matrix = (x => [[cos(x), -sin(x)], [sin(x), cos(x)]]);
const f_vec_add = (([a1, a2], [b1, b2]) => [a1 + b1, a2 + b2]);

const f_svg_ellipse_arc = (([cx, cy], [rx, ry], [t1, Δ], φ) => {
    /* [
    returns a SVG path element that represent a ellipse.
    cx,cy → center of ellipse
    rx,ry → major minor radius
    t1 → start angle, in radian.
    Δ → angle to sweep, in radian. positive.
    φ → rotation on the whole, in radian
    URL: SVG Circle Arc http://xahlee.info/js/svg_circle_arc.html
    Version 2019-06-19
     ] */
    Δ = Δ % (2 * π);
    const rotMatrix = f_rotate_matrix(φ);
    const [sX, sY] = (f_vec_add(f_matrix_times(rotMatrix, [rx * cos(t1), ry * sin(t1)]), [cx, cy]));
    const [eX, eY] = (f_vec_add(f_matrix_times(rotMatrix, [rx * cos(t1 + Δ), ry * sin(t1 + Δ)]), [cx, cy]));
    const fA = ((Δ > π) ? 1 : 0);
    const fS = ((Δ > 0) ? 1 : 0);
    const path_2wk2r = document.createElementNS("http://www.w3.org/2000/svg", "path");
    path_2wk2r.setAttribute("d", "M " + sX + " " + sY + " A " + [rx, ry, φ / (2 * π) * 360, fA, fS, eX, eY].join(" "));
    return path_2wk2r;
});

(function (bootstrap) {
    document.querySelectorAll('[data-js-remove]').forEach(el => {
        el.remove();
    });
        
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

    /* GLOBAL FUNCTIONS */
    window.pageLoad = function () {

    };
})(bootstrap);