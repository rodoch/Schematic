import { Component, Prop } from '@stencil/core';

@Component({
    tag: 'schematic-resource-search',
    styleUrl: 'schematic-resource-search.scss'
})

export class ResourceSearch {
    @Prop({ mutable: true, reflectToAttr: true }) value: string;
    @Prop() url: string;
    @Prop() placeholder: string;
    
    updateValue(event) {
        this.value = event.target.value;
    }

    search(event) {
        event.preventDefault();
        const list = document.querySelector('schematic-resource-list');
        const searchUrl = (this.url) ? this.url : list.url;
        list.listResources(searchUrl);
    }

    render() {
        return (
            <div class="resource-search resource-navigator__content">
                <form class="resource-search__form" onSubmit={(event) => this.search(event)}>
                    <input class="resource-search__search-input" type="text" placeholder={this.placeholder} 
                        value={this.value} onInput={(event) => this.updateValue(event)} />
                </form>
            </div>
        );
    }
}