import { Component, Event, EventEmitter, Prop } from '@stencil/core';
import { ResourceButton } from '../../interfaces/ResourceButton';

@Component({
    tag: 'schematic-resource-button-create',
    styleUrl: 'schematic-resource-button-create.scss'
})

export class ResourceButtonCreate {
    @Event() createNewResource: EventEmitter;
    @Prop() inProgress: string;
    @Prop() completed: string;

    createButton(event: UIEvent) {
        event.preventDefault();

        const createAction: ResourceButton = {
            inProgress: this.inProgress, 
            completed: this.completed
        };

        this.createNewResource.emit(createAction);
    }

    render() {
        return (
            <button class="resource-button resource-button--primary resource-button--create" 
                onClick={(event: UIEvent) => this.createButton(event)}>
                <slot/>
            </button>
        );
    }
}