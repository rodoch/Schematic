export default class Resource {
    private id = 0;

    public readResourceId() {
        return this.id;
    }

    public updateResourceId(newId: number) {
        this.id = newId;
    }
}