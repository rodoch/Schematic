import { Component, Element, State, Prop, Event, EventEmitter, Watch, Listen } from '@stencil/core';
import { FocusableElements } from '../../shared/accessibility';
import Sortable from '@shopify/draggable/lib/sortable';

@Component({
    tag: 'schematic-resource-order',
    styleUrl: 'schematic-resource-order.scss'
})
export class ResourceOrder {
    @Element() element: HTMLStencilElement;
    @State() hasList: boolean;
    @State() hasSubmissionError: boolean;
    @State() html: string;
    @State() isSuccessful: boolean;
    @State() isVisible: boolean = true;
    @State() loading: boolean;
    @State() statusCode: number;
    @Prop() culture: string;
    @Prop() facets: string = '';
    @Prop() url: string;
    @Event() resourceOrderSubmit: EventEmitter;
    @Event() resourceOrderClose: EventEmitter;

    private firstFocusableElement: HTMLElement;
    private focusableElements: NodeListOf<HTMLElement>;
    private lastFocusableElement: HTMLElement;
    private modal: HTMLElement;
    private strings: any = {
        "en": {
            "close": "Close",
            "cancel": "Cancel",
            "instructions": "Click and drag the items below to reorder them",
            "save": "Save"
        },
        "ga": {
            "close": "Dún",
            "cancel": "Cealaigh",
            "instructions": "Déan cliceáil agus tarraingt ar na míreanna thíos chun iad a chur in ord",
            "save": "Sábháil"
        }
    }

    public form: HTMLFormElement;
    public localizedStrings: any;
    public sortable: any;

    @Watch('isVisible')
    visibilityChanged(newValue: boolean) {
        if (!newValue) {
            return;
        }

        this.fetchData();
    }

    componentWillLoad() {
        this.setLoadingState(true);
        this.preventScrolling();
        this.fetchData();
        this.localizedStrings = (document.documentElement.lang == 'ga' || document.documentElement.lang == 'ga-IE')
            ? this.strings.ga : this.strings.en;
    }

    componentDidLoad() {
        if (!this.isVisible) {
            return;
        }

        this.setInitialFocus();
    }

    componentDidUpdate() {
        if (this.hasList) {
            this.makeSortable();
        }
    }

    @Listen('keydown.escape')
    handleEscape(event: KeyboardEvent) {
      this.handleClose(event);
    }
  
    @Listen('keydown.tab')
    handleTab(event: KeyboardEvent) {
      if (!this.isVisible) {
        return;
      }
      
      if (this.focusableElements.length == 1) {
        event.preventDefault();
      }
  
      if (event.shiftKey) {
        this.handleBackwardTab(event);
      } else {
        this.handleForwardTab(event);
      }
    }
  
    setInitialFocus() {
      this.setFocus(this.modal);
    }

    returnFocusToDocument() {
        this.setFocus(document as any);
    }

    handleBackwardTab(event: KeyboardEvent) {
        if (document.activeElement === this.firstFocusableElement) {
            event.preventDefault();
            this.lastFocusableElement.focus();
        }
    }

    handleForwardTab(event: KeyboardEvent) {
        if (document.activeElement === this.lastFocusableElement) {
            event.preventDefault();
            this.firstFocusableElement.focus();
        }
    }

    setFocus(element: HTMLElement) {
        let focusableElements: NodeListOf<HTMLElement> = element.querySelectorAll(FocusableElements);
        this.focusableElements = Array.prototype.slice.call(focusableElements);
        this.firstFocusableElement = this.focusableElements[0];
        this.lastFocusableElement = this.focusableElements[this.focusableElements.length - 1];
        this.firstFocusableElement.focus();
    }

    preventScrolling() {
        document.body.setAttribute('style', 'width:100%;position:fixed;overflow:hidden');
    }

    allowScrolling() {
        document.body.removeAttribute('style');
    }

    handleClose(event: UIEvent) {
        event.preventDefault();
        this.resourceOrderClose.emit();
        this.returnFocusToDocument();
        this.isVisible = false;
        this.hasList = false;
        this.allowScrolling();
    }

    handleSubmit(event: UIEvent) {
        event.preventDefault();
        this.submitData();
    }

    async fetchData() {
        let url = this.urlBuilder('order');
        if (this.facets.length > 0) {
            url += "?facets=" + this.facets;
        }

        const config: RequestInit = {
            method: 'get',
            credentials: 'same-origin'
        }

        try {
            const response: any = await fetch(url, config);
            let status: number = await response.status;

            switch (status) {
                case 200:
                    this.setLoadingState(false);
                    response.text().then(text => {
                        this.updateEditor(text);
                    });
                    break;
                case 400:
                    this.hasSubmissionError = true;
                    this.statusCode = status;
                    break;
                case 500:
                    this.hasSubmissionError = true;
                    this.statusCode = status;
                    break;
            }
        } catch (error) {
            console.log('Fetch failed', error);
            this.hasSubmissionError = true;
        }
    }
    
    async submitData() {
        let url = this.urlBuilder('order');

        if (this.facets.length > 0) {
            url += "?facets=" + this.facets;
        }

        let formData = new FormData(this.form);
        formData.append('resourceOrderModel.Facets', this.facets);

        let listItems: NodeListOf<HTMLElement> = this.form.querySelectorAll('li');

        for (let i = 0; i < listItems.length; i++) {
            formData.append('resourceOrderModel.ResourceOrder[' + i + '].Key', listItems[i].dataset.resourceId);
            formData.append('resourceOrderModel.ResourceOrder[' + i + '].Value', i.toString());
        }

        const config: RequestInit = {
            method: 'post',
            body: formData,
            credentials: 'same-origin'
        };

        try {
            const response: any = await fetch(url, config);
            let status: number = await response.status;

            switch (status) {
                case 200:
                    this.resourceOrderSubmit.emit();
                    this.returnFocusToDocument();
                    this.resourceOrderClose.emit();
                    this.hasList = false;
                    this.allowScrolling();
                    break;
                case 400:
                    this.hasSubmissionError = true;
                    this.statusCode = status;
                    break;
                case 500:
                    this.hasSubmissionError = true;
                    this.statusCode = status;
                    break;
            }
        } catch (error) {
            console.log('Fetch failed', error);
            this.hasSubmissionError = true;
        }
    }

    updateEditor(html: string) {
        this.html = html;
        this.hasList = true;
    }

    makeSortable() {
        this.sortable = new Sortable(this.element.querySelector('ul'), {
            draggable: 'li'
        });
    }

    setLoadingState(state: boolean) {
        this.loading = state;
    }

    urlBuilder(endpoint: string) {
        return this.enforceTrailingSlash(this.url) + endpoint;
    }

    enforceTrailingSlash(url: string) {
        return url.endsWith('/') ? url : url + '/';
    }

    render() {
        let modalBody;

        if (this.loading) {
            modalBody = <schematic-loading></schematic-loading>
        } else {
            modalBody = 
            <form class="gaois-modal__form" onSubmit={(event: UIEvent) => this.handleSubmit(event)}
                ref={(el: HTMLFormElement) => this.form = el}>
                <div innerHTML={this.html}></div>
            </form>
        }
        
        if (this.isVisible) {
            return (
                <div class="gaois-modal-background">
                    <div class="gaois-modal" role="dialog" aria-labelledby="dialog-title"
                        ref={(el: HTMLElement) => this.modal = el}>
                        <header class="gaois-modal__header">
                            <p class="gaois-modal__header-instructions">{this.localizedStrings.instructions}</p>
                            <div class="gaois-modal__header-bar">
                                <button class="gaois-modal__close-button" onClick={(event: UIEvent) => this.handleClose(event)}>
                                    {this.localizedStrings.close}
                                </button>
                            </div>
                        </header>
                        <div class="gaois-modal__content">
                            {modalBody}
                        </div>
                        <footer class="gaois-modal__footer">
                            <button class="button" onClick={(event: UIEvent) => this.handleClose(event)}>
                                {this.localizedStrings.cancel}
                            </button>
                            <button class="button button--primary" onClick={(event: UIEvent) => this.handleSubmit(event)}>
                                {this.localizedStrings.save}
                            </button>
                        </footer>
                    </div>
                </div>
            );
        }
    }
}