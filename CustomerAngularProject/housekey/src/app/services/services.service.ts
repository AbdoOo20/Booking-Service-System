import { HttpClient } from "@angular/common/http";
import { forkJoin, map, Observable, of } from "rxjs";
import { Service } from "../common/interfaces/service";
import { Category } from "../common/interfaces/category";
// Basma Code
import { ServiceDetails } from "../common/interfaces/ServiceDetails";
import { MatBottomSheet } from "@angular/material/bottom-sheet";
import { MatSnackBar } from "@angular/material/snack-bar";
import { MatDialog } from "@angular/material/dialog";
import { TranslateService } from "@ngx-translate/core";
import { DomHandlerService } from "./dom-handler.service";
import {
    ConfirmDialogComponent,
    ConfirmDialogModel,
} from "@shared-components/confirm-dialog/confirm-dialog.component";
import { AlertDialogComponent } from "@shared-components/alert-dialog/alert-dialog.component";
import { Agent } from "../common/interfaces/agent";
import { Injectable } from "@angular/core";
import { wishList } from "../common/interfaces/wishList";

@Injectable({
    providedIn: "root",
})
// Basma Code
// export class wishList {
//     customerId: string;
//     serviceId: number;

//     constructor(CustomerID: string, serviceID: number) {
//         this.customerId = CustomerID;
//         this.serviceId = serviceID;
//     }
// }
export class ServicesService {
    API_URL = "http://localhost:18105/api/Services/all";
    CAT_URL = "http://localhost:18105/api/Categories";
    LOCATION_URL =
        "https://raw.githubusercontent.com/homaily/Saudi-Arabia-Regions-Cities-and-Districts/refs/heads/master/json/regions_lite.json";
    Booking_SRV_URL = "http://localhost:18105/api/Book";
    // Basma Code
    API_GetServicebyID = "http://localhost:18105/api/Services/";
    ApI_Add_to_wishList = "http://localhost:18105/api/wishlist";
    API_getAgents = "";
    API_get_allWishList="http://localhost:18105/api/wishlist/";
    Favourite_service: wishList;

    constructor(
        private http: HttpClient,
        private bottomSheet: MatBottomSheet,
        private snackBar: MatSnackBar,
        public dialog: MatDialog,
        public translateService: TranslateService,
        private domHandlerService: DomHandlerService
    ) {}


  /*  getServiceById(id: number) {
        return this.http.get("http://localhost:18105/api/Services/" + id)
    }*/

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
    public getServiceById(id: number): Observable<ServiceDetails> {
        return this.http.get<ServiceDetails>(this.API_GetServicebyID + id);
    }
    // public getRelatedServices(id: number): Observable<Service[]> {
    //     return this.http
    //         .get<any[]>(this.API_GetServicebyID + id)
    //         .pipe(map((services) => services.filter((item) => item.id == id)));
    // }
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
            return this.http
                .get<Service[]>(this.API_URL)
                .pipe(
                    map((services) =>
                        services.filter(
                            (item) => item.priceForTheCurrentDay != null
                        )
                    )
                );
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
    GetAllServicesPrice(from: number, to: number): Observable<Service[]> {
        if (from == 0 && to == 0) {
            return this.http
                .get<Service[]>(this.API_URL)
                .pipe(
                    map((services) =>
                        services.filter(
                            (item) => item.priceForTheCurrentDay != null
                        )
                    )
                );
        } else {
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
    }

  
    // Test Function
    GetAllFilterion(
        cat: string | null | undefined,
        loc: string | null | undefined,
        frm: number | null | undefined,
        to: number | null | undefined
    ): Observable<Service[]> {
        return this.http.get<Service[]>(this.API_URL).pipe(
            map((services) =>
                services
                    .filter((item) => {
                        const price = Number(item.priceForTheCurrentDay);
                        return (
                            (cat == null ||
                                cat == undefined ||
                                cat === item.category) &&
                            (loc == null ||
                                loc == undefined ||
                                loc === item.location) &&
                            (frm == null || frm == undefined || price >= frm) &&
                            (to == null || to == undefined || price <= to) &&
                            item.priceForTheCurrentDay != null
                        );
                    })
                    .reverse()
            )
        );
    }

    GetRecomenditionServicesForBooking(): Observable<Service[]> {
        const bookingData$: Observable<any[]> = this.http.get<any[]>(
            this.Booking_SRV_URL
        );
        const serviceData$: Observable<Service[]> = this.http
            .get<Service[]>(this.API_URL)
            .pipe(
                map((services) =>
                    services.filter(
                        (item) => item.priceForTheCurrentDay != null
                    )
                )
            );

        return forkJoin([bookingData$, serviceData$]).pipe(
            map(([bookingData, serviceData]) => {
                const serviceIds: number[] = bookingData.map(
                    (item) => item.serviceId
                );
                const frequency: { [key: number]: number } = {};

                serviceIds.forEach((num: number) => {
                    frequency[num] = (frequency[num] || 0) + 1;
                });

                const sorted: number[] = Object.entries(frequency)
                    .sort(
                        (a: [string, number], b: [string, number]) =>
                            b[1] - a[1]
                    )
                    .map((entry: [string, number]) => Number(entry[0]));

                const top8: number[] = sorted.slice(0, 8);

                const filteredServices: Service[] = serviceData.filter(
                    (service) => top8.includes(service.id)
                );

                return filteredServices;
            })
        );
    }

    // Basma Code
    public addToFavorites(service:Service, direction: any ,CustomerID : string) {
        this.Favourite_service = new wishList (service.id,CustomerID);
           
        this.http.post(this.ApI_Add_to_wishList, this.Favourite_service)
        .subscribe(
          (response) => {
            // Success: Show a success message using snackbar
            this.snackBar.open('The Service ' + service.name + '" has been added to favorites.', '×', {
              verticalPosition: 'top',
              duration: 3000,
              direction: direction
            });
          },
          (error) => {
            // Error: Handle the error case here, you can also show an error message
            this.snackBar.open('Failed to add the property to favorites.', '×', {
              verticalPosition: 'top',
              duration: 3000,
              direction: direction
            });
          }
        );
          
         }
 
         public addToFavoritesInServiceDetails(service: ServiceDetails, direction: any ,CustomerID : string) {
             this.Favourite_service = new wishList (service.id,CustomerID);
                
             this.http.post(this.ApI_Add_to_wishList, this.Favourite_service)
             .subscribe(
               (response) => {
                 // Success: Show a success message using snackbar
                 this.snackBar.open('The property "' + service.name + '" has been added to favorites.', '×', {
                   verticalPosition: 'top',
                   duration: 3000,
                   direction: direction
                 });
               },
               (error) => {
                 // Error: Handle the error case here, you can also show an error message
                 this.snackBar.open('Failed to add the property to favorites.', '×', {
                   verticalPosition: 'top',
                   duration: 3000,
                   direction: direction
                 });
               }
             );
               
              }
            //to get services in the wish List
           getAllServicesInWishList(customerid:string){
               return this.http.get(this.API_get_allWishList+"/"+customerid);
              }
              
    getPropertyById(id: number): Observable<ServiceDetails> {
        return this.http.get<ServiceDetails>(
            this.API_GetServicebyID + "/" + id
        );
    }

    getRelatedProperties(id: number): Observable<Service[]> {
        return this.http.get<Service[]>(this.API_GetServicebyID + "/" + id);
    }

    public openConfirmDialog(title: string, message: string) {
        const dialogData = new ConfirmDialogModel(title, message);
        const dialogRef = this.dialog.open(ConfirmDialogComponent, {
            maxWidth: "400px",
            data: dialogData,
        });
        return dialogRef;
    }

    public openAlertDialog(message: string) {
        const dialogRef = this.dialog.open(AlertDialogComponent, {
            maxWidth: "400px",
            data: message,
        });
        return dialogRef;
    }
    public paginator(items: any, page?: number, perPage?: number) {
        var page = page || 1,
            perPage = perPage || 4,
            offset = (page - 1) * perPage,
            paginatedItems = items.slice(offset).slice(0, perPage),
            totalPages = Math.ceil(items.length / perPage);
        return {
            data: paginatedItems,
            pagination: {
                page: page,
                perPage: perPage,
                prePage: page - 1 ? page - 1 : null,
                nextPage: totalPages > page ? page + 1 : null,
                total: items.length,
                totalPages: totalPages,
            },
        };
    }
    public getAgents(): Agent[] {
        return [
            {
                id: 1,
                fullName: "Lusia Manuel",
                desc: "Phasellus sed metus leo. Donec laoreet, lacus ut suscipit convallis, erat enim eleifend nulla, at sagittis enim urna et lacus.",
                organization: "HouseKey",
                email: "lusia.m@housekey.com",
                phone: "(224) 267-1346",
                social: {
                    facebook: "lusia",
                    twitter: "lusia",
                    linkedin: "lusia",
                    instagram: "lusia",
                    website: "https://lusia.manuel.com",
                },
                ratingsCount: 6,
                ratingsValue: 480,
                image: "images/agents/a-1.jpg",
            },
        ];
    }
}
