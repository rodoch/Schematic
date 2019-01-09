import { Component, Element } from '@stencil/core';
const popmotion = (window as any).popmotion;

@Component({
    tag: 'schematic-loading',
    styleUrl: 'schematic-loading.scss'
})

export class Loading {
    @Element() loader: HTMLStencilElement;

    componentDidLoad() {
        if (popmotion) {
            const container = this.loader.querySelector('.balls');

            const ballStylers = Array
            .from(container.childNodes)
            .map(popmotion.styler as any);
            
            const distance = 100;
            
            popmotion.everyFrame()
            .start((timestamp) => ballStylers.map((thisStyler: any, i) => {
                thisStyler.set('y', distance * Math.sin(0.004 * timestamp + (i * 0.5)));
            }));
        }
    }

    render() {
        if (popmotion) {
            return (
                <div class="loading">
                    <div class="loading-content--hidden">
                        <slot/>
                    </div>
                    <div class="loading-body">
                        <div class="balls">
                            <div class="ball"></div>
                            <div class="ball"></div>
                            <div class="ball"></div>
                            <div class="ball"></div>
                        </div>
                    </div>
                </div>
            );
        }
    }
}