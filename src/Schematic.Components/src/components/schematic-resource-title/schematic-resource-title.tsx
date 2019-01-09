import { Component, Element, Event, EventEmitter } from '@stencil/core';

@Component({
    tag: 'schematic-resource-title'
})

export class ResourceExplorer {
    @Element() title: HTMLStencilElement;
    @Event() newResourceTitle: EventEmitter;

    componentDidLoad() {
        const title = this.title.textContent.trim();
        this.newResourceTitle.emit(title);
    }
}