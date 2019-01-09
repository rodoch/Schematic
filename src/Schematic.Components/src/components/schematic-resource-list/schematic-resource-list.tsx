import { Component, State, Element, Prop, Event, EventEmitter, Method, Watch } from '@stencil/core';

@Component({
    tag: 'schematic-resource-list'
})
export class ResourceList {
    @Element() list: HTMLStencilElement;
    @State() loading: boolean;
    @State() hasContent: boolean = true;
    @State() html: string;
    @Prop() url: string;
    @Prop() facets: string;
    @Prop() noContent: string = 'No result found';
    @Prop({ mutable: true, reflectToAttr: true }) activeResourceId: string;
    @Event() activeResourceSet: EventEmitter;

    @Watch('activeResourceId')
    activeResourceChanged() {
        this.setActiveResource();
    }

    componentDidLoad() {
        this.listResources(this.url);

        if (this.activeResourceId) {
            this.activeResourceSet.emit(this.activeResourceId);
        }
    }

    @Method()
    listResources(url: string) {
        this.setLoadingState(true);

        const explorer: HTMLSchematicResourceExplorerElement = this.list.closest('schematic-resource-explorer');
        if (!explorer) {
            console.error('Explorer element required');
            return;
        }

        const filters: HTMLSchematicResourceFiltersElement = explorer.querySelector('schematic-resource-filters');
        const filterForm: HTMLFormElement = filters.querySelector('.resource-filter__form');
        let formData: FormData = (filters && filters.active && filterForm) 
            ? new FormData(filterForm) 
            : new FormData();

        const searchForm: HTMLFormElement = explorer.querySelector('.resource-search__form');
        if (searchForm) {
            const searchInput: HTMLInputElement = searchForm.querySelector('.resource-search__search-input');
            const query: string = searchInput.value;
            formData.append('Query', query);
        }

        formData.append('ActiveResourceID', this.activeResourceId);
        formData.append('Facets', this.facets);

        const config: RequestInit = {
            method: 'post',
            body: formData,
            credentials: 'same-origin'
        };
        
        fetch(url, config)
        .then(response => {
            this.setLoadingState(false);
            if (response.ok) {
                if (response.status === 204) {
                    // No content
                    this.hasContent = false;
                } else {
                    response.text().then(text => {
                        this.hasContent = true;
                        this.updateResourceList(text);
                    });
                }
            } else {
                let status: number = response.status;
                let error: string = response.statusText;
                console.error(status + ': ' + error);
                this.updateResourceListError();
            }
        }).catch(error => {
            this.setLoadingState(false);
            console.error(error);
        });
    }

    @Method()
    newResource() {
        this.unsetActiveResource();
    }

    @Method()
    clearResourceList() {
        while (this.list.hasChildNodes()) {
            this.list.removeChild(this.list.lastChild);
        }
    }

    setActiveResource() {
        const links = this.list.querySelectorAll('schematic-resource-list-link');
        for (let i = 0; i < links.length; ++i) {
            if (links[i].getAttribute('resource-id') === this.activeResourceId) {
                links[i].status = 'active';
            } else {
                links[i].status = '';
            }
        }
    }

    unsetActiveResource() {
        const links = this.list.querySelectorAll('schematic-resource-list-link');
        for (let i = 0; i < links.length; ++i) {
            links[i].status = '';
        }
    }

    updateResourceList(html: string){
        this.html = html;
    }

    updateResourceListError() {
        this.clearResourceList();
    }

    setLoadingState(state: boolean) {
        this.loading = state;
    }
    
    render() {
        if (this.loading) {
            return (
                <div class="resource-list__loading">
                    <schematic-loading></schematic-loading>
                </div>
            );
        } else if (this.hasContent) {
            return (
                <div innerHTML={this.html}></div>
            );
        } else {
            return (
                <div class="resource-navigator__no-result">{this.noContent}</div>
            );
        }
    }
}