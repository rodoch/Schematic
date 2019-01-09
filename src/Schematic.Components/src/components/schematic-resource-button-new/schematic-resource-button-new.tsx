import { Component, Event, EventEmitter, Prop } from '@stencil/core';
import { ResourceButton } from '../../interfaces/ResourceButton';

@Component({
    tag: 'schematic-resource-button-new',
    styleUrl: 'schematic-resource-button-new.scss'
})

export class ResourceButtonNew {
    @Event() getNewResource: EventEmitter;
    @Prop() inProgress: string;
    @Prop() completed: string;

    newButton(event: UIEvent) {
        event.preventDefault();

        const newAction: ResourceButton = {
            inProgress: this.inProgress, 
            completed: this.completed
        };

        this.getNewResource.emit(newAction);
    }

    render() {
        return (
            <button class="resource-button resource-button--new" onClick={(event: UIEvent) => this.newButton(event)}>
                <slot/>
            </button>
        );
    }
}