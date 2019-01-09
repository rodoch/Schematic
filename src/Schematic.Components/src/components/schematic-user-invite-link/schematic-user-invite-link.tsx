import { Component, State, Prop } from '@stencil/core';

@Component({
    tag: 'schematic-user-invite-link'
})

export class UserInviteLink {
    @State() successStatus: boolean;
    @State() loading: boolean;
    @State() class: string;
    @Prop() url: string;
    @Prop() userId: string;
    @Prop() text: string;
    @Prop() link: string;
    @Prop() success: string;

    componentWillUpdate() {
        this.class = (!this.loading) ? '' : 'loading';
    }

    linkClicked(event: UIEvent) {
        event.preventDefault();
        let formData: FormData = new FormData();
        formData.append('userID', this.userId);
        const config: RequestInit = {
            method: 'post',
            body: formData,
            credentials: 'same-origin'
        }
        this.fetchData(this.url, config);
    }

    fetchData(url: string, config: RequestInit) {
        this.loading = true;
        fetch(url, config)
        .then(response => {
            if (response.ok) {
                this.successStatus = true;
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

    render() {
        return(
            <div class="resource-editor__user-verification">
                {this.successStatus
                    ?   <div innerHTML={this.success}></div>
                    :   <div class={this.class}>
                            {this.text} <a href="" onClick={(event) => this.linkClicked(event)}>
                                {this.link}
                            </a>
                        </div>
                }
            </div>
        );
    }
}