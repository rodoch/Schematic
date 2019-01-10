import { Component, Element, State, Prop, Event, EventEmitter, Method } from '@stencil/core';

@Component({
    tag: 'schematic-language-button'
})

export class LanguageButton {
    @Element() button: HTMLStencilElement;
    @State() open: boolean;
    @State() current: boolean;
    @State() href: string;
    @State() handleOffClick: any;
    @Prop() base: string;
    @Prop() code: string;
    @Prop() text: string;
    @Event() toggleLanguageMenu: EventEmitter;

    componentWillLoad() {
        this.current = (this.button.getAttribute('slot') === 'current') ? true : false;
        this.setHref();
        this.handleOffClick = this.offClickHandler.bind(this);
    }

    @Method()
    setHref() {
        if (this.current) {
            return;
        }

        const query: string = window.location.search;
        const path: string = window.location.pathname;
        let pathArray: string[] = path.split('/');
        if (this.base && this.base.length > 0 && this.base !== "/") {
            pathArray[2] = this.code;
        } else {
            pathArray[1] = this.code;
        }
        const newPath: string = pathArray.join('/');
        let result = newPath;
        result += (query) ? query : '';
        this.href = result;
    }

    buttonPress(event: UIEvent) {
        event.preventDefault();
        this.open = (!this.open) ? true : false;
        this.toggleLanguageMenu.emit();

        if (this.current) {
            this.toggleOffClickHander(this.open);
        }
    }
    
    offClickHandler(event) {
        if (!this.button.contains(event.target)) {
            this.open = (!this.open) ? true : false;
            this.toggleLanguageMenu.emit();
            this.toggleOffClickHander(false);
        }
    }

    toggleOffClickHander(state: boolean) {
        if (state) {
            document.addEventListener('click', this.handleOffClick);
        } else {
            document.removeEventListener('click', this.handleOffClick);
        }
    }

    render() {
        if (this.current) {
            return(
                <button class="portal-header__button portal-header__button--language" 
                    onClick={(event) => this.buttonPress(event)}>{this.text}</button>
            );
        } else {
            return(
                <li class="lang-sub-menu__item">
                    <a class="lang-sub-menu__link" href={this.href}>{this.text}</a>
                </li>
            );
        }
    }
}