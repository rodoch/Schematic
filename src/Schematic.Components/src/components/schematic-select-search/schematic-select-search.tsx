import { Component, Element, State, Prop, Listen } from '@stencil/core';
import { SelectSearchItem } from '../../interfaces/SelectSearchItem';
import 'elix/src/ListBox';

@Component({
    tag: 'schematic-select-search'
})

export class SelectSearch {
    @Element() select: HTMLStencilElement;
    @State() searchActive: boolean;
    @State() selected: SelectSearchItem;
    @State() input: HTMLInputElement;
    @State() searchInput: HTMLInputElement;
    @State() list: HTMLElement;
    @State() content: string;
    @State() value: string;
    @State() loading: boolean;
    @State() html: string;
    @State() listVisible: boolean = false;
    @Prop() url: string;
    @Prop() placeholder: string;

    componentWillLoad() {
        this.content = this.placeholder;
        this.input = this.select.querySelector('input');
        this.setContent();
    }

    componentDidUpdate() {
        if (this.searchActive) {
            const input: HTMLInputElement = this.select
                .querySelector('.select-search__input');
            input.focus();
        }

        this.listVisible = (this.searchActive && this.value && this.value.length > 0) 
            ? true 
            : false;

        if (this.listVisible) {
            document.addEventListener('click', this.offClickHandler.bind(this));
        }
    }

    @Listen('itemSelected')
    onItemSelected(event: CustomEvent) {
        this.selectValue(event.detail.value);
        this.content = event.detail.content;
        this.toggleSearch(event);
    }

    toggleSearch(event) {
        event.preventDefault();
        this.searchActive = (!this.searchActive) ? true : false;
    }

    search(event) {
        event.preventDefault();
        this.getResults();
    }
    
    updateValue(event) {
        this.value = event.target.value;
        this.getResults();
    }

    getResults() {
        this.loading = true;
        let url: string = this.enforceTrailingSlash(this.url);
        const query: string = this.value;
        const searchUrl: string =  url + "select/search/?query=" + query;

        fetch(searchUrl, {
            method: 'get',
            credentials: 'same-origin'
        }).then(response => {
            if (response.ok) {
                response.text().then(text => {
                    if (query === this.value) {
                        this.html = text;
                        this.loading = false;
                    }
                });
            } else {
                let status: number = response.status;
                let error: string = response.statusText;
                console.error(status + ': ' + error);
                this.loading = false;
            }
        }).catch(error => {
            console.error(error);
            this.loading = false;
        });
    }

    setContent() {
        if (!this.input.value || this.input.value.length === 0) {
            return;
        }

        this.loading = true;
        let url: string = this.enforceTrailingSlash(this.url);
        const getUrl =  url + "select/display/?id=" + this.input.value;

        fetch(getUrl, {
            method: 'get',
            credentials: 'same-origin'
        }).then(response => {
            if (response.ok) {
                response.text().then(text => {
                    this.content = text;
                    this.loading = false;
                });
            } else {
                let status: number = response.status;
                let error: string = response.statusText;
                console.error(status + ': ' + error);
                this.loading = false;
            }
        }).catch(error => {
            console.error(error);
            this.loading = false;
        });
    }

    selectValue(value: string) {
        this.input.value = value;
    }

    deactivateSearch() {
        this.searchActive = false;
        this.listVisible = false;
    }
    
    offClickHandler(event) {
        if (!this.select.contains(event.target)) {
            this.deactivateSearch();
        }
    }

    enforceTrailingSlash(url) {
        return url.endsWith("/") ? url : url + "/";
    }

    handleKeyDown(event: KeyboardEvent) {
        if (event.keyCode === 40) {
            event.preventDefault();
            const items: NodeListOf<HTMLElement> = this.list.querySelectorAll('.select-item');

            Array.call(items).forEach(element => {
                element.tabIndex = -1;
            });

            items[0].tabIndex = 0;
            items[0].focus();
        }
    }

    render() {
        return (
            <div class="select-search">
                {!this.searchActive
                    ?   <button class="select-search__button--toggle" 
                            innerHTML={this.content} 
                            onClick={(event) => this.toggleSearch(event)}>
                        </button>
                    :   <form class="select-search__form" onSubmit={(event) => this.search(event)}>
                            <input class="select-search__input" type="text"
                                placeholder={this.placeholder} 
                                value={this.value}
                                onInput={(event) => this.updateValue(event)}
                                onKeyDown={(event) => this.handleKeyDown(event)} />
                        </form>
                }
                {this.listVisible && this.loading
                    ?   <div class="select-search__list select-search__list--loading">
                            <schematic-loading></schematic-loading>
                        </div>
                    :   ''
                }
                {this.listVisible && !this.loading
                    ?   <div class="select-search__list" 
                            ref={(el: HTMLElement) => this.list = el} 
                            innerHTML={this.html}>
                        </div>
                    :   ''
                }
            </div>
        );
    }
}