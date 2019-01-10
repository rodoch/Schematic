import { Element, Component, State, Prop, Listen } from '@stencil/core';
import '../schematic-uploader/schematic-uploader';

@Component({
    tag: 'schematic-image-uploader',
    styleUrl: 'schematic-image-uploader.scss',
})
export class ImageUploader {
    @Element() imageUploader: HTMLStencilElement;
    @State() hasImage: boolean = false;
    @State() imgAlt: string;
    @State() imgSrc: string;
    @State() uploadPath: string;
    @Prop() accept: string;
    @Prop() alt: string;
    @Prop() base: string;
    @Prop() container: string;
    @Prop() culture: string;
    @Prop() delete: string;
    @Prop() deleteInput: string;
    @Prop() fileName: string;
    @Prop() formDataName: string = 'files';
    @Prop() headers: string;
    @Prop() input: string;
    @Prop() maxFiles: number;
    @Prop() method: string = 'POST';
    @Prop() nodrop: boolean = false;
    @Prop() path: string;
    @Prop() target: string;

    private deleteInputElement: HTMLInputElement;
    private targetInputElement: HTMLInputElement;

    @Listen('uploadResult')
    onEvent(event: CustomEvent) {
        console.log(event.detail.detail.xhr.response);
        const xhrResponse: any = JSON.parse(event.detail.detail.xhr.response);
        const newFileId: string = xhrResponse[0].id;
        const newFileName: string = event.detail.detail.file.name;
        this.targetInputElement.setAttribute('value', newFileId);
        this.displayImage(newFileName);
    }

    enforceTrailingSlash(path: string) {
        return path.endsWith("/") ? path : path + "/";
    }

    getImagePath(path: string, fileName: string) {
        let imagePath: string = (this.base && this.base.length > 0) ? this.base : '';

        if (imagePath.endsWith('/')) {
            imagePath = imagePath.slice(0, -1);
        }

        imagePath += this.enforceTrailingSlash(path) + fileName;

        if (this.container && this.container.length > 0) {
            imagePath += "?container=" + this.container;
        }

        return imagePath;
    }

    getUploadPath() {
        let path: string = (this.base && this.base.length > 0) ? this.base : '';

        if (path.endsWith('/')) {
            path = path.slice(0, -1);
        }

        path += this.target;

        if (this.container && this.container.length > 0) {
            path += "?container=" + this.container;
        }

        return path;
    }

    deleteImage(event: UIEvent) {
        event.preventDefault();
        this.deleteInputElement.value = 'true';
        this.hasImage = false;
    }

    displayImage(fileName: string) {
        this.imgSrc = this.getImagePath(this.path, fileName);
        this.hasImage = true;
    }

    componentWillLoad() {
        this.hasImage = (this.fileName && this.fileName.length > 1) ? true : false;
        this.imgAlt = this.alt;
        this.imgSrc = this.getImagePath(this.path, this.fileName);
        this.uploadPath = this.getUploadPath();
        this.targetInputElement = document.getElementById(this.input) as HTMLInputElement;
        this.deleteInputElement = document.getElementById(this.deleteInput) as HTMLInputElement;
    }

    render() {
        return([
            <div>
                {this.hasImage &&
                    <div class="resource-uploader__image-panel">
                        <img class="resource-uploader__image" src={this.imgSrc} alt={this.imgAlt} />
                        {this.delete &&
                            <div class="resource-uploader__image-controls">
                                <button class="resource-uploader__image-delete-button" 
                                    onClick={(event: UIEvent) => this.deleteImage(event)}>{this.delete}</button>
                            </div>
                        }
                    </div>
                }
            </div>,
            <div class={this.hasImage ? 'resource-uploader__slot-contents' : 'resource-uploader__slot-contents--hidden'}>
                <slot/>
            </div>,
            <schematic-uploader
                accept={this.accept}
                culture={this.culture}
                form-data-name={this.formDataName}
                headers={this.headers}
                target={this.uploadPath} 
                max-files={this.maxFiles} 
                method={this.method}
                nodrop={this.nodrop}>
            </schematic-uploader>
        ]);
    }
}