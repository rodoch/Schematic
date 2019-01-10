import { Component, Element, Prop, State, Event, EventEmitter } from '@stencil/core';
import { SelectSearchItem } from '../../interfaces/SelectSearchItem';

@Component({
    tag: 'schematic-select-item'
})

export class SelectItem {
    @Element() item: HTMLStencilElement;
    @State() content: string;
    @Prop() value: string;
    @Prop({ mutable: true, reflectToAttr: true }) tabIndex: number;
    @Event() itemSelected: EventEmitter;

    componentDidLoad() {
        this.content = this.item.querySelector('div[slot="content"]').innerHTML;
    }

    itemClicked() {
        const item: SelectSearchItem = {
            value: this.value,
            content: this.content
        };

        this.itemSelected.emit(item);
    }

    handleKeyDown(event: KeyboardEvent) {
        event.preventDefault();
        
        switch (event.keyCode)
        {
            case 39:
            case 40:
                this.goToNextSelectItem();
                break;
            case 37:
            case 38:
                this.goToPreviousSelectItem();
                break;
            case 13:
                this.itemClicked();
                break;
            case 27:
                this.escapeFromSelectList();
                break;
        }
    }

    goToNextSelectItem() {
        let nextElement: HTMLSchematicSelectItemElement = this.item.nextSibling as HTMLSchematicSelectItemElement;

        // Ensure next sibling is not a text node
        while (nextElement && nextElement.nodeType != 1) {
            nextElement = nextElement.nextSibling as HTMLSchematicSelectItemElement;
        }

        if (nextElement) {
            const nextItem: HTMLElement = nextElement.querySelector('.select-item');
            this.tabIndex = -1;
            nextItem.tabIndex = 0;
            nextItem.focus();
        }
    }

    goToPreviousSelectItem() {
        let previousElement: HTMLSchematicSelectItemElement = this.item.previousSibling as HTMLSchematicSelectItemElement;

        while (previousElement && previousElement.nodeType != 1) {
            previousElement = previousElement.previousSibling as HTMLSchematicSelectItemElement;
        }

        if (previousElement) {
            const previousItem: HTMLElement = previousElement.querySelector('.select-item');
            this.tabIndex = -1;
            previousItem.tabIndex = 0;
            previousItem.focus();
        } else {
            this.returnToSearchInput();
        }
    }

    escapeFromSelectList() {
        this.returnToSearchInput();
    }

    returnToSearchInput() {
        this.tabIndex = -1;
        const root: HTMLElement = this.item.parentElement.parentElement;
        const searchInput: HTMLElement = root.querySelector('.select-search__input');
        searchInput.focus();
    }

    render() {
        return(
            <div class="select-item" tabIndex={this.tabIndex} 
                    onKeyDown={(event) => this.handleKeyDown(event)} 
                    onClick={() => this.itemClicked()}>
                <slot name="content" />
            </div>
        );
    }
}