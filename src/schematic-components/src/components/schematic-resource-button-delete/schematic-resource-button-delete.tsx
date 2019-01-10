import { Component, Event, EventEmitter, Prop } from '@stencil/core';
import { ResourceButton } from '../../interfaces/ResourceButton';

@Component({
    tag: 'schematic-resource-button-delete',
    styleUrl: 'schematic-resource-button-delete.scss'
})
export class ResourceButtonDelete {
    @Event() deleteCurrentResouce: EventEmitter;
    @Prop() inProgress: string;
    @Prop() confirm: string;
    @Prop() completed: string;

    deleteButton(event: UIEvent) {
        event.preventDefault();

        const deleteAction: ResourceButton = {
            inProgress: this.inProgress, 
            completed: this.completed
        };

        const confirm: boolean = window.confirm(this.confirm);

        if (this.confirm && confirm) {
            this.deleteCurrentResouce.emit(deleteAction);
        }
    }

    render() {
        return (
            <button class="resource-button resource-button resource-button--delete" onClick={(event: UIEvent) => this.deleteButton(event)}>
                <slot/>
            </button>
        );
    }
}