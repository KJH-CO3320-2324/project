(function () {
    const attrPrefix = 'data-nm-tour';
    const attrs = {
        trigger: `${attrPrefix}-for`,
        container: `${attrPrefix}-container`,
        title: `${attrPrefix}-title`,
        step: `${attrPrefix}-step`,
        stepArrow: `${attrPrefix}-step-arrow`,
        stepSelector: `${attrPrefix}-step-selector`,
        stepTarget: `${attrPrefix}-step-target`,
        stepTitle: `${attrPrefix}-step-title`,
        stepMessage: `${attrPrefix}-step-message`
    };

    // Returns a step object for a given DOM element.
    // A step is not returned if it does not have a step attribute, or if its step number < 0
    function getStep(el, container) {
        const filterSelector = el.getAttribute(attrs.stepSelector);

        const targetSelector = `[${attrs.step}="${el.getAttribute(attrs.stepTarget)}"]`;
        const targets = Array.from(document.querySelectorAll(targetSelector));
        
        return {
            arrow: el.getAttribute(attrs.stepArrow) !== 'false',
            el: () => targets.find(target => !filterSelector || target.matches(filterSelector)),
            selector: el.getAttribute(attrs.stepSelector),
            title: el.getAttribute(attrs.stepTitle) || container.getAttribute(attrs.title),
            message: el.getAttribute(attrs.stepMessage) || el.innerHTML
        };
    }

    // Returns the valid step objects for the given DOM container.
    // The container itself can be a step.
    function getSteps(container) {
        const stepElements = container.querySelectorAll(`[${attrs.stepTarget}]`);
        
        const steps = [];
        
        stepElements.forEach(el => {
            const step = getStep(el, container);
            steps.push(step);
        });

        return steps;
    }

    // Listen to click events. 
    // Start the tour if the click event was a trigger for a valid tour.
    document.addEventListener('click', e => {
        if (Shepherd.activeTour) {
            return;
        }

        const trigger = e.target.closest(`[${attrs.trigger}]`);
        if (!trigger) {
            return;
        }

        const containerId = trigger.getAttribute(attrs.trigger);
        const container = document.querySelector(`[${attrs.container}="${containerId}"]`);
        if (!container) {
            return;
        }

        const steps = getSteps(container);
        if (steps.length === 0) {
            return;
        }

        const tour = new Shepherd.Tour({
            defaultStepOptions: {
                cancelIcon: {
                    enabled: true
                },
                classes: 'card',
                scrollTo: { behavior: 'smooth', block: 'center' }
            },
            useModalOverlay: true
        });

        steps.forEach((step, i) => {
            const buttons = [];
            if (steps.length > 1) {
                if (i > 0) {
                    buttons.push({
                        action: function () {
                            return this.back();
                        },
                        classes: 'btn btn btn-outline-secondary btn-sm',
                        text: 'Back'
                    });
                }

                buttons.push({
                    action: function () {
                        return this.next();
                    },
                    classes: 'btn btn btn-outline-primary btn-sm',
                    text: i === steps.length - 1 ? 'Close' : 'Next'
                });
            }
            
            tour.addStep({
                arrow: step.arrow !== false,
                attachTo: {
                    element: step.el,
                    on: 'bottom'
                },
                buttons: buttons,
                title: step.title,
                text: step.message
            });
        });

        tour.start();
    });
})();