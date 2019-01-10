import { Component, Element, Event, EventEmitter } from '@stencil/core';

@Component({
    tag: 'schematic-resource-repeater-delete'
})

export class ResourceRepeaterDeleteButton {
    @Element() button: HTMLStencilElement;
    @Event() deleteRepeatable: EventEmitter;

    buttonPress(event: UIEvent) {
        event.preventDefault();
        this.deleteRepeatable.emit();
    }

    render() {
        return (
            <button class="resource-repeater__delete-button" onClick={(event: UIEvent) => this.buttonPress(event)}>
                <slot/>
            </button>
        );
    }
}