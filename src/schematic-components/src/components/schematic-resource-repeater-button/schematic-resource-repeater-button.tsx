import { Component, Element, State, Prop } from '@stencil/core';

@Component({
    tag: 'schematic-resource-repeater-button'
})

export class ResourceRepeaterButton {
    @Element() button: HTMLStencilElement;
    @State() loading: boolean;
    @State() hasResource: boolean = false;
    @State() html: string;
    @Prop() url: string;
    @Prop() text: string;

    buttonPress(event: UIEvent) {
        event.preventDefault();
        this.hasResource = true;
        const createUrl = this.urlBuilder('create');
        const config: RequestInit = {
            method: 'get',
            credentials: 'same-origin'
        }
        this.fetchResource(createUrl, config);
    }

    fetchResource(url: string, config: RequestInit) {
        this.setLoadingState(true);

        fetch(url, config)
        .then(response => {
            this.setLoadingState(false);
            if (response.ok) {
                response.text().then(text => {
                    this.html = text;
                });
            } else {
                let status: number = response.status;
                let error: string = response.statusText;
                console.error(status + ': ' + error);
            }
        })
        .catch(error => {
            this.setLoadingState(false);
            console.error(error);
        });
    }

    setLoadingState(state: boolean) {
        this.loading = state;
    }

    urlBuilder(endpoint: string) {
        return this.enforceTrailingSlash(this.url) + endpoint;
    }

    enforceTrailingSlash(url: string) {
        return url.endsWith("/") ? url : url + "/";
    }

    render() {
        if (this.loading) {
            return (
                <div class="resource-repeater__loading">
                    <schematic-loading>
                        <slot/>
                    </schematic-loading>
                </div>
            );
        } else if (this.hasResource) {
            return (
                <div>
                    <div innerHTML={this.html}></div>
                    <div>
                        <schematic-resource-repeater-button url={this.url} text={this.text}>
                        </schematic-resource-repeater-button>
                    </div>
                </div>
            );
        } else {
            return (
                <button class="resource-repeater__button" onClick={(event: UIEvent) => this.buttonPress(event)}>{this.text}</button>
            );
        }
    }
}