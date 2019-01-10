//import myObj from './myObj';

declare var Context: any;

const test2 = {
    id: 1,
    updateId() {
        this.id = 2;
    }
}

Context.globalVar = '';
Context.myObj = test2;