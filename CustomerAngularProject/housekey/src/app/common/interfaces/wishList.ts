export class wishList {
    customerId : string ;
    serviceId: number;

    constructor(serviceID : number , customerId : string ){
        this.customerId = customerId ;
        this.serviceId = serviceID;
    } }