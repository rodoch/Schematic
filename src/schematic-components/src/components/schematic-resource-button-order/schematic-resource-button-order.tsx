import { Component, Event, EventEmitter, Prop } from '@stencil/core';
import { ResourceButton } from '../../interfaces/ResourceButton';

@Component({
    tag: 'schematic-resource-button-order',
    styleUrl: 'schematic-resource-button-order.scss'
})
export class ResourceButtonOrder {
    @Event() orderResources: EventEmitter;
    @Prop() inProgress: string;
    @Prop() completed: string;

    orderButton(event: UIEvent) {
        event.preventDefault();

        const newAction: ResourceButton = {
            inProgress: this.inProgress, 
            completed: this.completed
        };

        this.orderResources.emit(newAction);
    }

    render() {
        return (
            <button class="resource-button resource-button--order" onClick={(event: UIEvent) => this.orderButton(event)}>
                <slot/>
            </button>
        );
    }
}