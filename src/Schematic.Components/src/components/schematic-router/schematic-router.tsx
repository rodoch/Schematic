import { Component, State, Listen } from '@stencil/core';

@Component({
    tag: 'schematic-router'
})
export class Router {
    @State() open: boolean;
    @State() menuClass: string;

    @Listen('urlUpdated')
    onUrlUpdated() {
        const languageButtons = document.querySelectorAll('schematic-language-button');
        for (let i: number = 0; i < languageButtons.length; ++i) {
            languageButtons[i].setHref();
        }
    }
}