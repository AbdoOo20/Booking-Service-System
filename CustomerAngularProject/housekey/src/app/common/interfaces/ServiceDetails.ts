import { provider } from "./provider";
import { Service } from "./service";
export interface ServiceDetails {
    id: number;
    name: string;
    details: string;
    location: string;
    startTime: string;
    endTime: string;
    providerName: string;
    priceForTheCurrentDay: number;
    category: string;
    images: string[];
    quantity: number;
    relatedServices: Service[];
    initialPayment: number;
    provider: provider;
}
