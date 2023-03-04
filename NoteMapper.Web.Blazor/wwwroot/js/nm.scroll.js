(function () {    
    let nextScrollDisabled = false;
    
    window.disableNextScroll = function () {
        nextScrollDisabled = true;
    };

    const scrollTo = window.scrollTo;
    window.scrollTo = (x, y) => {
        if (x === 0 && y === 0 && nextScrollDisabled) {
            nextScrollDisabled = false;
            return;
        }

        return scrollTo.apply(this, [x, y]);
    };
})();