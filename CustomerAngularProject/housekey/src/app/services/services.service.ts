import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { forkJoin, map, Observable, of } from "rxjs";
import { Service } from "../common/interfaces/service";
import { Category } from "../common/interfaces/category";

@Injectable({
    providedIn: "root",
})
export class ServicesService {
    API_URL = "http://localhost:18105/api/Services/all";
    CAT_URL = "http://localhost:18105/api/Categories";
    LOCATION_URL =
        "https://raw.githubusercontent.com/homaily/Saudi-Arabia-Regions-Cities-and-Districts/refs/heads/master/json/regions_lite.json";
    Booking_SRV_URL = "http://localhost:18105/api/Book";

    constructor(private http: HttpClient) {}

    // GetAllLocations(): Observable<Location[]> {
    //     return this.http.get<Location[]>(this.LOCATION_URL);
    // }
    getLocations(): Observable<any> {
        return this.http.get<any>(this.LOCATION_URL);
    }

    GetAllServices(): Observable<Service[]> {
        return this.http
            .get<Service[]>(this.API_URL)
            .pipe(
                map((services) =>
                    services.filter(
                        (item) => item.priceForTheCurrentDay != null
                    )
                )
            );
    }
    GetAllCategoriesItems(): Observable<Category[]> {
        return this.http.get<Category[]>(this.CAT_URL);
    }
    GetAllServicesByCatName(catName: string): Observable<Service[]> {
        if (catName == "") {
            return this.http
                .get<Service[]>(this.API_URL)
                .pipe(
                    map((services) =>
                        services.filter(
                            (item) => item.priceForTheCurrentDay != null
                        )
                    )
                );
        } else if (catName != "") {
            return this.http
                .get<Service[]>(this.API_URL)
                .pipe(
                    map((services) =>
                        services.filter(
                            (item) =>
                                item.category === catName &&
                                item.priceForTheCurrentDay != null
                        )
                    )
                );
        } else {
            return of([]);
        }
    }
    GetAllServicesByLocation(locName: string): Observable<Service[]> {
        if (locName == "") {
            return this.http.get<Service[]>(this.API_URL);
        } else if (locName != "") {
            return this.http
                .get<Service[]>(this.API_URL)
                .pipe(
                    map((services) =>
                        services.filter(
                            (item) =>
                                item.location === locName &&
                                item.priceForTheCurrentDay != null
                        )
                    )
                );
        } else {
            return of([]);
        }
    }
    // GetAllServicesPrice(from: number, to: number): Observable<Service[]> {
    //     if (from >= 0) {
    //         return this.http
    //             .get<Service[]>(this.API_URL)
    //             .pipe(
    //                 map((services) =>
    //                     services.filter(
    //                         (item) => item.priceForTheCurrentDay != null
    //                     )
    //                 )
    //             );
    //     } else {
    //         return this.http.get<Service[]>(this.API_URL).pipe(
    //             map((services) =>
    //                 services.filter((item) => {
    //                     const price = Number(item.priceForTheCurrentDay);
    //                     return price >= from && price <= to && price != null;
    //                 })
    //             )
    //         );
    //     }
    // }
    GetAllServicesPrice(from: number, to: number): Observable<Service[]> {
        return this.http.get<Service[]>(this.API_URL).pipe(
            map((services) =>
                services.filter((item) => {
                    const price = Number(item.priceForTheCurrentDay);
                    return (
                        price >= from &&
                        price <= to &&
                        item.priceForTheCurrentDay != null
                    );
                })
            )
        );
    }
    GetRecomenditionServicesForBooking(): Observable<Service[]> {
        const bookingData$: Observable<any[]> = this.http.get<any[]>(
            this.Booking_SRV_URL
        );
        const serviceData$: Observable<Service[]> = this.http.get<Service[]>(
            this.API_URL
        );

        return forkJoin([bookingData$, serviceData$]).pipe(
            map(([bookingData, serviceData]) => {
                // استخراج serviceId من البيانات المحجوزة
                const serviceIds: number[] = bookingData.map(
                    (item) => item.serviceId
                );
                const frequency: { [key: number]: number } = {};

                // حساب تكرار serviceIds
                serviceIds.forEach((num: number) => {
                    frequency[num] = (frequency[num] || 0) + 1;
                });

                // ترتيب serviceIds بناءً على التكرار
                const sorted: number[] = Object.entries(frequency)
                    .sort(
                        (a: [string, number], b: [string, number]) =>
                            b[1] - a[1]
                    )
                    .map((entry: [string, number]) => Number(entry[0]));

                // الحصول على أعلى 8 serviceIds
                const top8: number[] = sorted.slice(0, 8);

                // تصفية الخدمات بناءً على top8 serviceIds
                const filteredServices: Service[] = serviceData.filter(
                    (service) => top8.includes(service.id)
                );

                return filteredServices; // إرجاع الخدمات المصفاة
            })
        );
    }

    // GetAllServices(): Observable<Service[]> {
    //     return this.httpClient.get<Service[]>(this.API_URL);
    // }
    // public filterData(
    //     data: any,
    //     params: any,
    //     sort?: any,
    //     page?: any,
    //     perPage?: any
    // ) {
    //     if (params) {
    //         if (params.propertyType) {
    //             data = data.filter(
    //                 (service: Service) =>
    //                     service.category == params.propertyType.name
    //             );
    //         }

    //         //     if (params.propertyStatus && params.propertyStatus.length) {
    //         //         let statuses: any[] = [];
    //         //         params.propertyStatus.forEach((status: any) => {
    //         //             statuses.push(status.name);
    //         //         });
    //         //         let properties: any[] = [];
    //         //         data.filter((property: any) =>
    //         //             property.propertyStatus.forEach((status: any) => {
    //         //                 if (statuses.indexOf(status) > -1) {
    //         //                     if (!properties.includes(property)) {
    //         //                         properties.push(property);
    //         //                     }
    //         //                 }
    //         //             })
    //         //         );
    //         //         data = properties;
    //         //     }

    //         //     if (params.price) {
    //         //         if (this.settingsService.settings.currency == "USD") {
    //         //             if (params.price.from) {
    //         //                 data = data.filter((property: Property) => {
    //         //                     if (
    //         //                         property.priceDollar.sale &&
    //         //                         property.priceDollar.sale >= params.price.from
    //         //                     ) {
    //         //                         return true;
    //         //                     }
    //         //                     if (
    //         //                         property.priceDollar.rent &&
    //         //                         property.priceDollar.rent >= params.price.from
    //         //                     ) {
    //         //                         return true;
    //         //                     }
    //         //                     return false;
    //         //                 });
    //         //             }
    //         //             if (params.price.to) {
    //         //                 data = data.filter((property: Property) => {
    //         //                     if (
    //         //                         property.priceDollar.sale &&
    //         //                         property.priceDollar.sale <= params.price.to
    //         //                     ) {
    //         //                         return true;
    //         //                     }
    //         //                     if (
    //         //                         property.priceDollar.rent &&
    //         //                         property.priceDollar.rent <= params.price.to
    //         //                     ) {
    //         //                         return true;
    //         //                     }
    //         //                     return false;
    //         //                 });
    //         //             }
    //         //         }
    //         //         if (this.settingsService.settings.currency == "EUR") {
    //         //             if (params.price.from) {
    //         //                 data = data.filter((property: Property) => {
    //         //                     if (
    //         //                         property.priceEuro.sale &&
    //         //                         property.priceEuro.sale >= params.price.from
    //         //                     ) {
    //         //                         return true;
    //         //                     }
    //         //                     if (
    //         //                         property.priceEuro.rent &&
    //         //                         property.priceEuro.rent >= params.price.from
    //         //                     ) {
    //         //                         return true;
    //         //                     }
    //         //                     return false;
    //         //                 });
    //         //             }
    //         //             if (params.price.to) {
    //         //                 data = data.filter((property: Property) => {
    //         //                     if (
    //         //                         property.priceEuro.sale &&
    //         //                         property.priceEuro.sale <= params.price.to
    //         //                     ) {
    //         //                         return true;
    //         //                     }
    //         //                     if (
    //         //                         property.priceEuro.rent &&
    //         //                         property.priceEuro.rent <= params.price.to
    //         //                     ) {
    //         //                         return true;
    //         //                     }
    //         //                     return false;
    //         //                 });
    //         //             }
    //         //         }
    //         //     }

    //         //     if (params.city) {
    //         //         data = data.filter(
    //         //             (property: Property) => property.city == params.city.name
    //         //         );
    //         //     }

    //         //     if (params.zipCode) {
    //         //         data = data.filter(
    //         //             (property: Property) => property.zipCode == params.zipCode
    //         //         );
    //         //     }

    //         //     if (params.neighborhood && params.neighborhood.length) {
    //         //         let neighborhoods: any[] = [];
    //         //         params.neighborhood.forEach((item: any) => {
    //         //             neighborhoods.push(item.name);
    //         //         });
    //         //         let properties: any[] = [];
    //         //         data.filter((property: any) =>
    //         //             property.neighborhood.forEach((item: any) => {
    //         //                 if (neighborhoods.indexOf(item) > -1) {
    //         //                     if (!properties.includes(property)) {
    //         //                         properties.push(property);
    //         //                     }
    //         //                 }
    //         //             })
    //         //         );
    //         //         data = properties;
    //         //     }

    //         //     if (params.street && params.street.length) {
    //         //         let streets: any[] = [];
    //         //         params.street.forEach((item: any) => {
    //         //             streets.push(item.name);
    //         //         });
    //         //         let properties: any[] = [];
    //         //         data.filter((property: Property) =>
    //         //             property.street.forEach((item) => {
    //         //                 if (streets.indexOf(item) > -1) {
    //         //                     if (!properties.includes(property)) {
    //         //                         properties.push(property);
    //         //                     }
    //         //                 }
    //         //             })
    //         //         );
    //         //         data = properties;
    //         //     }

    //         //     if (params.bedrooms) {
    //         //         if (params.bedrooms.from) {
    //         //             data = data.filter(
    //         //                 (property: Property) =>
    //         //                     property.bedrooms >= params.bedrooms.from
    //         //             );
    //         //         }
    //         //         if (params.bedrooms.to) {
    //         //             data = data.filter(
    //         //                 (property: Property) =>
    //         //                     property.bedrooms <= params.bedrooms.to
    //         //             );
    //         //         }
    //         //     }

    //         //     if (params.bathrooms) {
    //         //         if (params.bathrooms.from) {
    //         //             data = data.filter(
    //         //                 (property: Property) =>
    //         //                     property.bathrooms >= params.bathrooms.from
    //         //             );
    //         //         }
    //         //         if (params.bathrooms.to) {
    //         //             data = data.filter(
    //         //                 (property: Property) =>
    //         //                     property.bathrooms <= params.bathrooms.to
    //         //             );
    //         //         }
    //         //     }

    //         //     if (params.garages) {
    //         //         if (params.garages.from) {
    //         //             data = data.filter(
    //         //                 (property: Property) =>
    //         //                     property.garages >= params.garages.from
    //         //             );
    //         //         }
    //         //         if (params.garages.to) {
    //         //             data = data.filter(
    //         //                 (property: Property) =>
    //         //                     property.garages <= params.garages.to
    //         //             );
    //         //         }
    //         //     }

    //         //     if (params.area) {
    //         //         if (params.area.from) {
    //         //             data = data.filter(
    //         //                 (property: Property) =>
    //         //                     property.area.value >= params.area.from
    //         //             );
    //         //         }
    //         //         if (params.area.to) {
    //         //             data = data.filter(
    //         //                 (property: Property) =>
    //         //                     property.area.value <= params.area.to
    //         //             );
    //         //         }
    //         //     }

    //         //     if (params.yearBuilt) {
    //         //         if (params.yearBuilt.from) {
    //         //             data = data.filter(
    //         //                 (property: Property) =>
    //         //                     property.yearBuilt >= params.yearBuilt.from
    //         //             );
    //         //         }
    //         //         if (params.yearBuilt.to) {
    //         //             data = data.filter(
    //         //                 (property: Property) =>
    //         //                     property.yearBuilt <= params.yearBuilt.to
    //         //             );
    //         //         }
    //         //     }

    //         //     if (params.features) {
    //         //         let arr: any[] = [];
    //         //         params.features.forEach((feature: any) => {
    //         //             if (feature.selected) arr.push(feature.name);
    //         //         });
    //         //         if (arr.length > 0) {
    //         //             let properties: any[] = [];
    //         //             data.filter((property: Property) =>
    //         //                 property.features.forEach((feature) => {
    //         //                     if (arr.indexOf(feature) > -1) {
    //         //                         if (!properties.includes(property)) {
    //         //                             properties.push(property);
    //         //                         }
    //         //                     }
    //         //                 })
    //         //             );
    //         //             data = properties;
    //         //         }
    //         //     }
    //         // }

    //         // // console.log(data)

    //         // //for show more properties mock data
    //         // for (var index = 0; index < 2; index++) {
    //         //     data = data.concat(data);
    //     }
    //     for (var index = 0; index < 2; index++) {
    //         data = data.concat(data);
    //     }

    //     // this.sortData(sort, data);
    //     return this.paginator(data, page, perPage);
    // }
    // public paginator(items: any, page?: number, perPage?: number) {
    //     var page = page || 1,
    //         perPage = perPage || 4,
    //         offset = (page - 1) * perPage,
    //         paginatedItems = items.slice(offset).slice(0, perPage),
    //         totalPages = Math.ceil(items.length / perPage);
    //     return {
    //         data: paginatedItems,
    //         pagination: {
    //             page: page,
    //             perPage: perPage,
    //             prePage: page - 1 ? page - 1 : null,
    //             nextPage: totalPages > page ? page + 1 : null,
    //             total: items.length,
    //             totalPages: totalPages,
    //         },
    //     };
    // }
}
