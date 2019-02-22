import { Component, Element, Listen, Prop, Method } from '@stencil/core';

@Component({
    tag: 'schematic-resource-repeater'
})

export class ResourceRepeater {
    @Element() repeater: HTMLStencilElement;
    @Prop() minInstances: number;
    @Prop() initialInstanceIndex: number;

    @Listen('resourceRepeated')
    onResourceRepeated(event: CustomEvent) {
        event.stopPropagation();
        this.sequenceRepeatables();
    }
    @Method()
    sequenceRepeatables() {
        let repeatables = this.repeater.querySelectorAll('schematic-resource-repeatable');
        let index: number = (this.initialInstanceIndex && this.initialInstanceIndex > 0)
            ? this.initialInstanceIndex : 0

        if (index === 1) {
            repeatables[index - 1].index = String(index);
        }

        for (index; index < repeatables.length; ++index) {
            console.log("go");
            repeatables[index].index = String(index);

            if (this.minInstances && index < this.minInstances) {
                repeatables[index].isRequired = true;
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