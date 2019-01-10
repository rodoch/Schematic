import { Component, Prop } from '@stencil/core';
import flatpickr from 'flatpickr';

@Component({
    tag: 'schematic-datepicker',
    styleUrl: 'schematic-datepicker.scss'
})
export class Datepicker {
    @Prop() altInput: boolean = true;
    @Prop() altFormat: string = "j F Y";
    @Prop() culture: string = "en";
    @Prop() dateFormat: string = "Y-m-d H:i";
    @Prop() defaultDate: string;
    @Prop() enableTime: boolean = false;
    @Prop() input: string;

    flatpickr: any;
    datepickerElement: HTMLDivElement;
    targetInputElement: HTMLInputElement;

    componentDidLoad() {
        if (this.input && this.input.length > 0) {
            this.targetInputElement = document.getElementById(this.input) as HTMLInputElement;
        }

        this.flatpickr = flatpickr(this.datepickerElement, {
            altInput: true,
            altFormat: this.altFormat,
            dateFormat: this.dateFormat,
            defaultDate: this.defaultDate,
            enableTime: this.enableTime,
            locale: (this.culture !== 'ga' && this.culture !== 'ga-IE') 
                ? undefined
                : {
                    firstDayOfWeek: 1,
                    weekdays: {
                        shorthand: ["Domh", "Luan", "Máirt", "Céad", "Déar", "Aoin", "Sath"],
                        longhand: ["Domhnach", "Luan", "Máirt", "Céadaoin", "Déardaoin", "Aoine", "Satharn"]
                    },
                    months: {
                        shorthand: ["Ean", "Fea", "Már", "Aib", "Beal", "Meith", "Iúil", "Lún", "M. Fómh.", "D. Fómh.", "Samh", "Nol"],
                        longhand: ["Eanáir", "Feabhra", "Márta", "Aibreán", "Bealtaine", "Meitheamh", "Iúil", "Lúnasa", "Meán Fómhair", "Deireadh Fómhair", "Samhain", "Nollaig"]
                    },
                    ordinal: function ordinal(nth) {
                        if (nth > 1) return "";
                        return "ú";
                    },
                    rangeSeparator: " go ",
                    weekAbbreviation: "Sn",
                    scrollTitle: "Scrolláil chun cur leis",
                    toggleTitle: "Cliceáil chun malartú",
                    yearAriaLabel: "Bliain"
                },
            onChange: (selectedDates: any, dateStr: string, instance: any) => {
                console.log(selectedDates);
                console.log(instance);
                this.targetInputElement.value = dateStr;
            }
        });
    }

    render() {
        return(
            <input ref={(el: HTMLDivElement) => this.datepickerElement = el} />
        );
    }
}