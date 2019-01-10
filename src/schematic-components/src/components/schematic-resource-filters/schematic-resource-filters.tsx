import { Component, Element, State, Prop, Method, Watch, Event, EventEmitter } from '@stencil/core';

@Component({
    tag: 'schematic-resource-filters'
})
export class ResourceFilters {
    @Element() filters: HTMLStencilElement;
    @State() loading: boolean;
    @State() html: string;
    @State() bodyClass: string;
    @Prop() url: string;
    @Prop() filter: string;
    @Prop() resetFilters: string = 'Reset';
    @Prop({ mutable: true, reflectToAttr: true }) active: boolean;
    @Prop({ mutable: true, reflectToAttr: true }) menuOpen: boolean;
    @Event() filterMenuToggle: EventEmitter;
    @Event() filterStatusUpdate: EventEmitter;

    @Watch('menuOpen')
    menuStatusChanged() {
        this.bodyClass = (this.menuOpen) 
            ? 'resource-filters resource-filters--visible' 
            : 'resource-filters';
    }

    componentWillLoad() {
        this.bodyClass = 'resource-filters';
        this.getFilters(this.url);
    }

    @Method()
    getFilters(url: string) {
        fetch(url, {
            method: 'get',
            credentials: 'same-origin'
        }).then(response => {
            this.setLoadingState(false);
            if (response.ok) {
                response.text().then(text => {
                    this.updateFilters(text);
                });
            } else {
                let status: number = response.status;
                let error: string = response.statusText;
                console.error(status + ': ' + error);
                this.updateFiltersError();
            }
        }).catch(error => {
            this.setLoadingState(false);
            console.error(error);
        });
    }

    @Method()
    listResources() {
        const list = this.filters.closest('schematic-resource-explorer')
            .querySelector('schematic-resource-list');
        if (list) {
            list.listResources(list.url);
        }
    }

    closeFiltersButton(event: UIEvent) {
        event.preventDefault();
        this.filterMenuToggle.emit();
    }

    setFiltersButton(event: UIEvent) {
        event.preventDefault();
        this.filterStatusUpdate.emit(true);
    }

    resetFiltersButton(event: UIEvent) {
        event.preventDefault();
        this.getFilters(this.url);
    }

    updateFilters(html: string) {
        this.html = html;
    }

    updateFiltersError() {}

    setLoadingState(state: boolean) {
        this.loading = state;
    }

    render() {
        return (
            <div class={this.bodyClass}>
                <div class="resource-filters__tools">
                    <button class="resource-button resource-button--primary resource-filters__set-button" 
                        onClick={(event) => this.setFiltersButton(event)}>
                        {this.filter}
                    </button>
                    <button class="resource-button resource-filters__reset-button" 
                        onClick={(event) => this.resetFiltersButton(event)}>
                        {this.resetFilters}
                    </button>
                    <button class="resource-button resource-filters__close-button" 
                        onClick={(event) => this.closeFiltersButton(event)}>X</button>
                </div>
                <div class="resource-filters__content">
                    {this.loading
                        ? <schematic-loading></schematic-loading>
                        : <div innerHTML={this.html}></div>
                    }
                </div>
            </div>
        );
    }
}