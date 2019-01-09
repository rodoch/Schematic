import { Component, Element, State, Prop, Method, Listen, Watch, Event, EventEmitter } from '@stencil/core';

@Component({
    tag: 'schematic-resource-editor',
    styleUrl: 'schematic-resource-editor.scss'
})
export class ResourceEditor {
    @Element() editor: HTMLStencilElement;
    @State() editorMode: string;
    @State() editorClass: string;
    @State() loading: boolean;
    @State() html: string;
    @Prop() facets: string = '';
    @Prop() url: string;
    @Prop({ mutable: true, reflectToAttr: true }) resourceId: string;
    @Prop() placeholder: string;
    @Prop() noContent: string = 'This entry was not found';
    @Prop() readInProgress: string = 'Loading…';
    @Prop() readCompleted: string = 'Ready';
    @Event() resourceRefresh: EventEmitter;
    @Event() resourceUpdated: EventEmitter;

    @Watch('resourceId')
    resourceIdChanged() {
        this.setEditor();
    }

    @Watch('editorMode')
    editorModeChanged() {
        this.editorClass = 'resource-editor resource-editor--' + this.editorMode;
    }

    componentWillLoad() {
        this.editorClass = 'resource-editor resource-editor--default';
        this.setEditor();
    }
    
    @Listen('getNewResource')
    onGetNewResource(event: CustomEvent) {
        this.updateEditorStatus(event.detail.inProgress);
        this.newResource(event.detail.completed);
    }
    
    @Listen('createNewResource')
    onCreateNewResource(event: CustomEvent) {
        this.updateEditorStatus(event.detail.inProgress);
        this.createResource(event.detail.completed);
    }
    
    @Listen('updateCurrentResource')
    onUpdateCurrentResource(event: CustomEvent) {
        this.updateEditorStatus(event.detail.inProgress);
        this.updateResource(this.resourceId, event.detail.completed);
    }
    
    @Listen('deleteCurrentResouce')
    onDeleteCurrentResouce(event: CustomEvent) {
        this.updateEditorStatus(event.detail.inProgress);
        this.deleteResource(this.resourceId, event.detail.completed);
    }

    @Method()
    newResource(messageOnComplete: string) {
        this.editorMode = 'create';
        let createUrl = this.urlBuilder('create');
        if (this.facets.length > 0) {
            createUrl += "?facets=" + this.facets;
        }
        console.log(createUrl);
        const config: RequestInit = {
            method: 'get',
            credentials: 'same-origin'
        }
        this.fetchResource(createUrl, config, messageOnComplete);
    }

    @Method()
    createResource(messageOnComplete: string) {
        const createUrl = this.urlBuilder('create');
        const form: HTMLFormElement = this.editor.querySelector('.resource-editor__form');
        const formData: FormData = new FormData(form);
        formData.append('facets', this.facets);
        const config: RequestInit = {
            method: 'post',
            body: formData,
            credentials: 'same-origin'
        }
        this.fetchResource(createUrl, config, messageOnComplete);
    }

    @Method()
    readResource(id: string, messageOnComplete: string) {
        this.editorMode = 'read';
        const readUrl = this.urlBuilder('read');
        let formData: FormData = new FormData();
        formData.append('id', id);
        formData.append('facets', this.facets);
        console.log(formData);
        const config: RequestInit = {
            method: 'post',
            body: formData,
            credentials: 'same-origin'
        }
        this.resourceId = id;
        this.fetchResource(readUrl, config, messageOnComplete);
    }

    @Method()
    updateResource(id: string, messageOnComplete: string) {
        this.editorMode = 'update';
        const saveUrl = this.urlBuilder('update');
        const form: HTMLFormElement = this.editor.querySelector('.resource-editor__form');
        let formData: FormData = new FormData(form);
        formData.append('id', id);
        formData.append('facets', this.facets);
        console.log(formData);
        const config: RequestInit = {
            method: 'post',
            body: formData,
            credentials: 'same-origin'
        }
        this.fetchResource(saveUrl, config, messageOnComplete, true);
    }

    @Method()
    deleteResource(id: string, messageOnComplete: string) {
        this.editorMode = 'delete';
        const deleteUrl = this.urlBuilder('delete');
        const form: HTMLFormElement = this.editor.querySelector('.resource-editor__form');
        let formData: FormData = new FormData(form);
        formData.append('id', id);
        formData.append('facets', this.facets);
        const config: RequestInit = {
            method: 'post',
            body: formData,
            credentials: 'same-origin'
        }
        this.fetchResource(deleteUrl, config, messageOnComplete);
    }

    @Method()
    clearEditor() {
        while (this.editor.hasChildNodes()) {
            this.editor.removeChild(this.editor.lastChild);
        }
    }

    fetchResource(
            url: string, 
            config: RequestInit, 
            messageOnComplete: string, 
            resourceUpdated: boolean = false) {
        this.setLoadingState(true);

        fetch(url, config)
        .then(response => {
            this.setLoadingState(false);
            if (response.ok) {
                switch (response.status)
                {
                    case 201: // Successful resource creation - redirect to new resource editor
                        response.text().then(text => {
                            this.resourceRefresh.emit(text);
                        });
                        break;
                    case 204: // No content
                        this.resourceRefresh.emit();
                        this.updateEditorStatus(messageOnComplete);
                        break;
                    default:
                        response.text().then(text => {
                            this.updateEditor(text);
                            this.updateEditorStatus(messageOnComplete);
                            if (resourceUpdated === true) {
                                this.resourceUpdated.emit();
                            }
                        });
                        break;
                }
            } else {
                switch (response.status)
                {
                    case 404:
                        this.resourceRefresh.emit();
                        this.updateEditorStatus(this.noContent);
                        break;
                    default:
                        let status: number = response.status;
                        let error: string = response.statusText;
                        console.error(status + ': ' + error);
                        this.updateEditorError();
                        break;
                }
            }
        })
        .catch(error => {
            this.setLoadingState(false);
            console.error(error);
        });
    }

    updateEditor(html: string) {
        this.html = html;
    }

    updateEditorError() {
        this.clearEditor();
    }

    updateEditorStatus(status: string) {
        const statusBar = this.editor.querySelector('schematic-resource-editor-status');
        statusBar.status = status;

        setTimeout(function() {
            statusBar.status = '';
        }, 20000)
    }

    setEditor() {
        if (this.editorHasResource()) {
            this.updateEditorStatus(this.readInProgress);
            this.readResource(this.resourceId, this.readCompleted);
        }
    }

    editorHasResource() {
        return (this.resourceId && this.resourceId !== "0") 
            ? true : false;
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
                <div class={this.editorClass}>
                    <slot name="toolbar"></slot>
                    <div class="resource-editor__body">
                        <schematic-loading></schematic-loading>
                    </div>
                </div>
            );
        } else if (this.editorHasResource() || this.editorMode === "create") {
            return (
                <div class={this.editorClass}>
                    <slot name="toolbar"></slot>
                    <div class="resource-editor__body">
                        <div class="resource-editor__content">
                            <div innerHTML={this.html}></div>
                        </div>
                    </div>
                </div>
            );
        } else {
            return (
                <div class={this.editorClass}>
                    <slot name="toolbar"></slot> 
                    <div class="resource-editor__body">
                        <div class="resource-editor__content">
                            <div>{this.placeholder}</div>
                        </div>
                    </div>
                </div>
            );
        }
    }
}