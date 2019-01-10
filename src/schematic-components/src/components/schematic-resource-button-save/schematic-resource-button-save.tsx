import { Component, Event, EventEmitter, Prop } from '@stencil/core';
import { ResourceButton } from '../../interfaces/ResourceButton';

@Component({
    tag: 'schematic-resource-button-save',
    styleUrl: 'schematic-resource-button-save.scss'
})
export class ResourceButtonSave {
    @Event() updateCurrentResource: EventEmitter;
    @Prop() inProgress: string;
    @Prop() completed: string;

    saveButton(event: UIEvent) {
        event.preventDefault();

        const saveAction: ResourceButton = {
            inProgress: this.inProgress, 
            completed: this.completed
        };

        this.updateCurrentResource.emit(saveAction);
    }

    render() {
        return (
            <button class="resource-button resource-button--primary resource-button--save" 
                onClick={(event: UIEvent) => this.saveButton(event)}>
                <slot/>
            </button>
        );
    }
}