import { Element, Component, Prop, Event, EventEmitter } from '@stencil/core';
import '@vaadin/vaadin-upload/vaadin-upload.js';
//import * as i18nEN from './i18n/en-GB.js';
import * as i18nGA from './i18n/ga-IE.js';

@Component({
    tag: 'schematic-uploader'
})
export class Uploader {
    @Element() uploader: HTMLStencilElement;
    @Prop() accept: string;
    @Prop() culture: string;
    @Prop() formDataName: string;
    @Prop() headers: string;
    @Prop() maxFiles: number;
    @Prop() method: string = 'POST';
    @Prop() nodrop: boolean = false;
    @Prop() target: string;
    @Event() uploadResult: EventEmitter;

    componentDidLoad() {
        const upload: any = this.uploader.querySelector("vaadin-upload");

        if (this.culture && (this.culture == "ga" || this.culture == "ga-IE")) {
            upload.i18n = i18nGA.default;
        }

        upload.addEventListener('upload-response', (event: CustomEvent) => {
            this.uploadResult.emit(event);
        });
    }

    render() {
        return(
            <vaadin-upload
                accept={this.accept}
                form-data-name={this.formDataName}
                headers={this.headers}
                target={this.target} 
                max-files={this.maxFiles} 
                method={this.method}
                nodrop={this.nodrop}>
            </vaadin-upload>
        );
    }
}