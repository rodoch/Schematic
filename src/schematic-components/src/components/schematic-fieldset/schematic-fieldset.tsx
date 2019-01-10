import { Component, Prop, State, Watch } from '@stencil/core';

@Component({
    tag: 'schematic-fieldset',
    styleUrl: 'schematic-fieldset.scss'
})
export class Fieldset {
    @Prop() tabs: string;
    @Prop() selected: boolean;
    @State() fieldsetClass: string;

    componentWillLoad() {
        this.fieldsetClass = "resource-editor__fieldset";

        if (this.tabs && this.tabs.length > 0 && !this.selected) {
            this.fieldsetClass += " tabs-hidden";
        }
    }

    @Watch('selected')
    updateSelected(newValue: boolean): void  {
        this.fieldsetClass =  (newValue) 
            ? "resource-editor__fieldset"
            : "resource-editor__fieldset tabs-hidden";
    }

    render() {
        return(
            <fieldset class={this.fieldsetClass}>
                <slot/>
            </fieldset>
        );
    }
}