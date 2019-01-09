import { Component, Element, Prop, State } from '@stencil/core';

enum Modes {
    SignIn,
    ForgotPassword,
    SetPassword
}

@Component({
    tag: 'schematic-sign-in'
})
export class SignIn {
    @Element() authentication: HTMLStencilElement;
    @State() html: string;
    @State() loading: boolean;
    @State() modes: any;
    @State() authMode: Modes;
    @Prop() url: string;
    @Prop() mode: string;
    @Prop() token: string;
    @Prop() forgot: string;
    @Prop() return: string;

    componentWillLoad() {
        this.modes = {
            signIn:  "sign-in",
            forgotPassword: "forgot-password",
            setPassword: "set-password"
        }

        switch (this.mode) {
            case this.modes.signIn:
                this.authMode = Modes.SignIn;
                break;
            case this.modes.forgotPassword:
                this.authMode = Modes.ForgotPassword;
                break;
            case this.modes.setPassword:
                this.authMode = Modes.SetPassword;
                break;
        }

        const url = this.composeAuthUrl();
        const config: RequestInit = {
            method: 'get',
            credentials: 'same-origin'
        }
        this.fetchData(url, config);
    }

    componentDidUpdate() {
        if (!this.loading) {
            const form: HTMLFormElement = this.authentication.querySelector('.sign-in-form');

            if (form) {
                form.addEventListener('submit', (event: UIEvent) => this.submitForm(event), false);
            }
        }
    }

    composeAuthUrl() {
        let modeUrl: string;
        
        switch (this.authMode) {
            case Modes.SignIn:
                modeUrl = this.modes.signIn;
                break;
            case Modes.ForgotPassword:
                modeUrl = this.modes.forgotPassword;
                break;
            case Modes.SetPassword:
                modeUrl = this.modes.setPassword;
                break;
        }

        let returnUrl = this.enforceTrailingSlash(this.url) + modeUrl;

        if (this.token && this.token.length > 0) {
            returnUrl += "?token=" + this.token;
        }

        return returnUrl;
    }

    getForm() {
        const url = this.composeAuthUrl();
        const config: RequestInit = {
            method: 'get',
            credentials: 'same-origin'
        }
        this.fetchData(url, config);
    }

    submitForm(event: UIEvent) {
        event.preventDefault();
        const url = this.composeAuthUrl();
        const form: HTMLFormElement = this.authentication.querySelector('.sign-in-form');
        const formData: FormData = new FormData(form);
        const config: RequestInit = {
            method: 'post',
            body: formData,
            credentials: 'same-origin'
        }
        this.fetchData(url, config);
    }

    fetchData(url: string, config: RequestInit) {
        this.loading = true;
        fetch(url, config)
        .then(response => {
            if (response.ok) {
                const contentType = response.headers.get('content-type');
                if (contentType && contentType.indexOf('application/json') !== -1) {
                    response.json().then(data => {
                        const route = data.route;
                        if (route.length > 0) {
                            window.location = route;
                        }
                    });
                } else {
                    response.text().then(text => {
                        this.html = text;
                        this.loading = false;
                    });
                }
            } else {
                let status: number = response.status;
                let error: string = response.statusText;
                console.error(status + ': ' + error);
                this.loading = false;
            }
        }).catch(error => {
            console.error(error);
            this.loading = false;
        });
    }

    openSignIn(event: UIEvent) {
        event.preventDefault();
        this.authMode = Modes.SignIn;
        this.getForm();
    }

    openForgotPassword(event: UIEvent) {
        event.preventDefault();
        this.authMode = Modes.ForgotPassword;
        this.getForm();
    }

    enforceTrailingSlash(url) {
        return url.endsWith('/') ? url : url + '/';
    }

    render() {
        return(
            <div class="sign-in__container">
                {this.html
                    ?   <div class="sign-in__form-container" innerHTML={this.html}></div>
                    :   <div class="sign-in__form-container">
                            <schematic-loading></schematic-loading>
                        </div>
                }
                {this.authMode === Modes.SignIn
                    ?   <div class="sign-in__forgot-link">
                            <a href="" onClick={(event) => this.openForgotPassword(event)}>{this.forgot}</a>
                        </div>
                    :   ''
                }
                {this.authMode !== Modes.SignIn
                    ?   <div class="sign-in__forgot-link">
                            <a href="" onClick={(event) => this.openSignIn(event)}>{this.return}</a>
                        </div>
                    :   ''
                }
            </div>
        );
    }
}