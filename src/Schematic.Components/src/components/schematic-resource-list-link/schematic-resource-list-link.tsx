import { Component, Element, Prop, Event, EventEmitter, Watch } from '@stencil/core';
import scrollIntoView from 'scroll-into-view-if-needed'

@Component({
    tag: 'schematic-resource-list-link',
    styleUrl: 'schematic-resource-list-link.scss'
})

export class ResourceListLink {
    @Element() link: HTMLStencilElement;
    @Prop() resourceId: string;
    @Prop({ mutable: true, reflectToAttr: true }) status: string;
    @Event() activeResourceSet: EventEmitter;

    @Watch('status')
    statusChanged() {
        this.setActiveStatus();
    }

    componentDidLoad() {
        if (this.status === 'active') {
            this.setActiveStatus();
        }
    }

    setActiveStatus() {
        const link = this.link.querySelector('.resource-list__link');
        
        switch (this.status)
        {
            case 'active':
                this.activateLink(link);
                break;
            default:
                this.deactivateLink(link);
                break;
        }
    }

    activateLink(link: Element) {
        if (this.resourceId === "0") {
            return;
        }

        link.classList.add('resource-list__link--active');
        
        scrollIntoView(this.link, {
            behavior: 'smooth',
            scrollMode: 'if-needed'
        });
    }

    deactivateLink(link: Element) {
        link.classList.remove('resource-list__link--active');
    }

    linkClick(event: UIEvent) {
        event.preventDefault();
        this.activeResourceSet.emit(this.resourceId);
    }

    render() {
        return(
            <a class="resource-list__link" href="" onClick={(event: UIEvent) => this.linkClick(event)}>
                <slot/>
            </a>
        );
    }
}