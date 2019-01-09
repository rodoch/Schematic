import { Component, Prop } from '@stencil/core';

@Component({
    tag: 'schematic-resource-editor-status'
})

export class ResourceEditorStatus {
    @Prop() status: string;
    @Prop() message: string;

    render() {
        return (
            <div class="resource-editor-status">
                {this.status}
            </div>
        );
    }
}