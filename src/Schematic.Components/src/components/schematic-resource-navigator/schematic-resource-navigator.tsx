import { Component, Element, State, Prop } from '@stencil/core';

@Component({
    tag: 'schematic-resource-navigator'
})

export class ResourceNavigator {
    @Element() navigator: HTMLStencilElement;
    @State() navigatorClass: string;
    @Prop() hasSearch: boolean = true;
    @Prop() hasFilters: boolean = true;

    componentWillLoad() {
        let navigatorClass: string = 'resource-navigator';
        
        if (this.hasSearch && this.hasFilters) {
            navigatorClass += ' resource-navigator--has-search-filters';
        } else if (this.hasSearch) { 
            navigatorClass += ' resource-navigator--has-search';
        } else if (this.hasFilters) { 
            navigatorClass += ' resource-navigator--has-filters';
        }

        this.navigatorClass = navigatorClass;
    }

    render() {
        return(
            <div class={this.navigatorClass}>
                <slot/>
            </div>
        );
    }
}