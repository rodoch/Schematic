import { Component, State, Listen } from '@stencil/core';

@Component({
    tag: 'schematic-language-switcher'
})

export class LanguageSwitcher {
    @State() open: boolean;
    @State() menuClass: string;

    componentWillLoad() {
        this.menuClass = 'lang-sub-menu';
    }

    @Listen('toggleLanguageMenu')
    onToggleLanguageMenu(event: UIEvent) {
        event.stopPropagation();
        this.open = (!this.open) ? true : false;
        this.menuClass = (!this.open) ? 'lang-sub-menu' : 'lang-sub-menu lang-sub-menu--open';
    }

    render() {
        return(
            <div class="lang-menu">
                <slot name="current" />
                <div class={this.menuClass}>
                    <ul class="lang-sub-menu__list">
                        <slot name="list" />
                    </ul>
                </div>
            </div>
        );
    }
}