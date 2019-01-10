import { Component, Element, State, Event, EventEmitter, Listen } from '@stencil/core';

@Component({
    tag: 'schematic-resource-explorer'
})

export class ResourceExplorer {
    @Element() explorer: HTMLStencilElement;
    @State() initialPageTitle: string;
    @State() filterIsActive: boolean;
    @State() filterMenuOpen: boolean;
    @Event() urlUpdated: EventEmitter;

    componentWillLoad() {
        this.initialPageTitle = document.title;
        this.filterIsActive = false;
        this.filterMenuOpen = false;

        (window as Window).onpopstate = (event) => {
            if (event.state) {
                this.populate(event.state.resourceId);
            }
        };
    }

    @Listen('activeResourceSet')
    onActiveResourceSet(event: CustomEvent) {
        const resourceId = event.detail;
        this.populate(resourceId);
        this.updateHistory(resourceId);
    }

    @Listen('getNewResource')
    onGetNewResource() {
        const list = this.explorer.querySelector('schematic-resource-list');
        if (list) {
            list.newResource();
            this.updateHistoryNewResource();
        }
    }

    @Listen('resourceRefresh')
    onResourceRefresh(event: CustomEvent) {
        const resourceId: string = (event.detail) ? event.detail : "0";
        this.populate(resourceId);

        const list = this.explorer.querySelector('schematic-resource-list');
        if (list) {
            list.listResources(list.url);
        }
        
        this.updateHistory(resourceId);
    }

    @Listen('resourceUpdated')
    onResourceUpdated() {
        const list = this.explorer.querySelector('schematic-resource-list');
        if (list) {
            list.listResources(list.url);
        }
    }
    
    @Listen('resourceOrderSubmit')
    onResourceOrderSubmit() {
        const list = this.explorer.querySelector('schematic-resource-list');
        if (list) {
            list.listResources(list.url);
        }
    }

    @Listen('filterMenuToggle')
    onFilterMenuToggle() {
        this.filterMenuOpen = (!this.filterMenuOpen) ? true : false;
        this.toggleFilterMenu();
    }

    @Listen('filterStatusUpdate')
    onFilterStatusUpdate(event: CustomEvent) {
        this.filterIsActive = event.detail;
        this.toggleFilterStatus();
    }

    @Listen('newResourceTitle')
    onNewResourceTitle(event: CustomEvent) {
        const resourceTitle: string = event.detail;
        const url: string = location.href;
        let newTitle: string = resourceTitle;

        const splitByPipe: string[] = this.initialPageTitle.split(' | ');
        if (splitByPipe.length > 0) {
            newTitle += ' | ';
            newTitle += splitByPipe.slice(1);
        }

        document.title = newTitle;
        history.replaceState(history.state, newTitle, url);
    }

    populate(resourceId: string) {
        const editor = this.explorer.querySelector('schematic-resource-editor');
        if (editor) {
            editor.resourceId = resourceId;
        }
        
        const list = this.explorer.querySelector('schematic-resource-list');
        if (list) {
            list.activeResourceId = resourceId;
        }
    }

    getUrlParameter(name: string) {
        name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
        var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
        var results = regex.exec(location.search);
        return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
    };

    updateHistory(resourceId: string) {
        const title: string = document.title;
        let url: string = location.protocol + '//' + location.host + location.pathname;
        url += "?id=" + resourceId;

        const facets: string = this.getUrlParameter('facets');

        if (facets && facets.length > 0) {
            url += "&facets=" + facets;
        }

        if (resourceId === "0") {
            history.replaceState({resourceId: resourceId}, title, url);
        } else {
            history.pushState({resourceId: resourceId}, title, url);
        }

        this.urlUpdated.emit();
    }

    updateHistoryNewResource() {
        const title = this.initialPageTitle;
        let url = location.protocol + '//' + location.host + location.pathname;
        url += "?id=0";

        const facets: string = this.getUrlParameter('facets');

        if (facets && facets.length > 0) {
            url += "&facets=" + facets;
        }
        
        history.pushState({resourceId: "0"}, title, url);
        this.urlUpdated.emit();
    }

    toggleFilterMenu() {
        const filters: HTMLSchematicResourceFiltersElement = this.explorer.querySelector('schematic-resource-filters');
        if (filters) {
            filters.menuOpen = this.filterMenuOpen;
        }
    }

    toggleFilterStatus() {
        const controls: HTMLSchematicResourceFilterControlsElement = this.explorer.querySelector('schematic-resource-filter-controls');
        const filters: HTMLSchematicResourceFiltersElement = this.explorer.querySelector('schematic-resource-filters');

        if (controls) {
            controls.active = this.filterIsActive;
        }

        if (filters) {
            filters.active = this.filterIsActive;
            filters.listResources();
        }
    }

    render() {
        return(
            <div class="resource-explorer">
                <slot/>
            </div>
        );
    }
}