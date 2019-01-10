import { Component, Element, Prop, Watch, Listen, Event, EventEmitter } from '@stencil/core';

@Component({
    tag: 'schematic-resource-repeatable'
})

export class ResourceRepeatable {
    @Element() repeatable: HTMLStencilElement;
    @Prop() model: string;
    @Prop() deleteButton: string;
    @Prop({ mutable: true, reflectToAttr: true }) index: string;
    @Prop({ mutable: true, reflectToAttr: true }) isRequired: boolean = false;
    @Event() resourceRepeated: EventEmitter;

    @Watch('index')
    indexChanged() {
        this.modelRepeater();
    }

    componentDidLoad() {
        this.resourceRepeated.emit();
    }

    @Listen('deleteRepeatable')
    onDeleteRepeatable(event: CustomEvent) {
        event.stopPropagation();
        const repeater = this.repeatable.closest('schematic-resource-repeater');
        setTimeout(function() {
            repeater.sequenceRepeatables();
        }, 1000)
        this.unloadRepeatable();
    }

    modelRepeater() {
        this.elementsUpdate('label');
        this.elementsUpdate('input');
        this.elementsUpdate('textarea');
        this.elementsUpdate('select');
    }

    elementsUpdate(elementTagName: string) {
        const elements = this.repeatable.querySelectorAll(elementTagName);

        for (let i = 0; i < elements.length; ++i) {
            if (elements[i].id) {
                elements[i].id = this.formatId(elements[i].id);
            }

            const nameAttributeValue = elements[i].getAttribute('name');
            if (nameAttributeValue) {
                elements[i].setAttribute('name', this.formatName(nameAttributeValue));
            }

            const forAttributeValue = elements[i].getAttribute('for');
            if (forAttributeValue) {
                elements[i].setAttribute('for', this.formatId(forAttributeValue));
            }
        }
    }

    formatName(entity: string) {
        entity = entity.replace(this.model, '');
        entity = entity.replace(/\[(.*?)\]\./g, '');
        return this.model + '[' + this.index + '].' + entity;
    }

    formatId(entity: string) {
        const entityTransformed = this.model.replace(/\s*\./g, '_');
        entity = entity.replace(entityTransformed, '');
        entity = entity.replace(/\_(.*?)\_\_/g, '');
        return entityTransformed + '_'  + this.index + '__' + entity;
    }

    unloadRepeatable() {
        this.repeatable.parentNode.removeChild(this.repeatable);
    }

    render() {
        return (
            <div class="resource-repeatable">
                <slot/>
                <div class="resource-repeatable__tools">
                    {this.deleteButton && !this.isRequired
                        ?   <schematic-resource-repeater-delete>{this.deleteButton}</schematic-resource-repeater-delete>
                        :   ''
                    }
                </div>
            </div>
        );
    }
}