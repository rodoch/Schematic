import { Component, Element, Listen, Prop, Method } from '@stencil/core';

@Component({
    tag: 'schematic-resource-repeater'
})

export class ResourceRepeater {
    @Element() repeater: HTMLStencilElement;
    @Prop() minInstances: number;

    @Listen('resourceRepeated')
    onResourceRepeated(event: CustomEvent) {
        event.stopPropagation();
        this.sequenceRepeatables();
    }

    @Method()
    sequenceRepeatables() {
        let repeatables = this.repeater.querySelectorAll('schematic-resource-repeatable');

        for (let i: number = 0; i < repeatables.length; ++i) {
            repeatables[i].index = String(i);

            if (this.minInstances && i < this.minInstances) {
                repeatables[i].isRequired = true;
            }
        }
    }

    render() {
        return(
            <div class="resource-repeater">
                <slot/>
            </div>
        );
    }
}