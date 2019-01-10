import { Component, Element, Prop } from '@stencil/core';
import '@vaadin/vaadin-tabs/vaadin-tabs.js';

@Component({
    tag: 'schematic-tabs'
})
export class Tabs {
    @Element() tabsElement: HTMLStencilElement;
    @Prop() name: string;

    private tabbedContent: NodeListOf<HTMLStencilElement>;

    processTab(section: HTMLStencilElement, currentIndex: number, selectedIndex: number) {
        if (currentIndex === selectedIndex) {
            section.setAttribute('selected', '');
        } else {
            section.removeAttribute('selected');
        }
    }    

    selectTab(mutation: MutationRecord) {
        const name = mutation.attributeName;

        if (name !== 'selected') {
            return;
        }

        const tab = mutation.target;
        const selectedIndex = parseInt((tab as HTMLElement).getAttribute(name));
        Array.from(this.tabbedContent).map((section, index) => this.processTab(section, index, selectedIndex));
        this.tabbedContent[selectedIndex].setAttribute('selected', '');
    }

    componentWillLoad() {
        let selectorTarget: string = (this.name && this.name.length > 0) 
            ? 'schematic-fieldset[tabs="'+this.name+'"]'
            : 'schematic-fieldset';
        this.tabbedContent = document.querySelectorAll<HTMLStencilElement>(selectorTarget);
    }

    private observer: MutationObserver;

    componentDidLoad() {
        const tabs = this.tabsElement.querySelector('vaadin-tabs');
        const config = { attributes: true };

        this.observer = new MutationObserver(mutations => {
            mutations.map(mutation => this.selectTab(mutation));
        });

        this.observer.observe(tabs, config);
    }

    componentDidUnload() {
        this.observer.disconnect();
    }
    
    render() {
        return(
            <vaadin-tabs>
                <slot/>
            </vaadin-tabs>
        );
    }
}