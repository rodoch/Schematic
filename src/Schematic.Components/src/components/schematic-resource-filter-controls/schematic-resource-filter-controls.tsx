import { Component, Element, State, Prop, Event, EventEmitter, Watch } from '@stencil/core';

@Component({
    tag: 'schematic-resource-filter-controls'
})

export class ResourceFilterControls {
    @Element() controls: HTMLStencilElement;
    @State() menuOpen: boolean;
    @State() filtersClass: string;
    @State() filtersStatus: string;
    @Prop({ mutable: true, reflectToAttr: true }) active: boolean;
    @Prop() filtersOn: string;
    @Prop() filtersOff: string;
    @Prop() removeFilters: string = 'Remove';
    @Event() filterMenuToggle: EventEmitter;
    @Event() filterStatusUpdate: EventEmitter;

    @Watch('active')
    filterStatusChanged() {
        this.filtersStatus = (this.active) ? this.filtersOn : this.filtersOff;
        this.filtersClass = (this.active) 
            ? 'resource-filter__controls resource-filter__controls--active'
            : 'resource-filter__controls';
    }

    componentWillLoad() {
        this.menuOpen = false;
        this.filtersClass = 'resource-filter__controls';
        this.filtersStatus = this.filtersOff;
    }

    openFiltersButton(event: UIEvent) {
        event.preventDefault();
        this.filterMenuToggle.emit();
    }

    removeFiltersButton(event: UIEvent) {
        event.preventDefault();
        this.filterStatusUpdate.emit(false);
        this.listResources();
    }

    listResources() {
        const list = this.controls.closest('schematic-resource-navigator')
            .querySelector('schematic-resource-list');
        if (list) {
            list.listResources(list.url);
        }
    }

    render() {
        return (
            <div class={this.filtersClass}>
                <button class="resource-filter__filter-button" onClick={(event) => this.openFiltersButton(event)}>{this.filtersStatus}</button>
                {this.active
                    ?   <button class="resource-button resource-filter__remove-button" onClick={(event) => this.removeFiltersButton(event)}>
                            {this.removeFilters}
                        </button>
                    :   ''
                }
            </div>
        );
    }
}