import { Component, Element } from '@stencil/core';

@Component({
    tag: 'schematic-resource-editor-toolbar'
})

export class ResourceEditorToolbar {
    @Element() toolbar: HTMLStencilElement;

    render() {
        return (
            <div class="resource-toolbar">
                <slot name="status"></slot>
                <div class="resource-toolbar__buttons">
                    <slot/>
                </div>
            </div>
        );
    }
}