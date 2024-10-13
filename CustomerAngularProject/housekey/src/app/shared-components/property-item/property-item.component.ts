import {
    Component,
    OnInit,
    Input,
    ViewChild,
    SimpleChange,
    ChangeDetectorRef,
} from "@angular/core";
import { Property } from "@models/app.models";
import { SwiperDirective } from "../../theme/components/swiper/swiper.directive";
import {
    SwiperConfigInterface,
    SwiperModule,
    SwiperPaginationInterface,
} from "../../theme/components/swiper/swiper.module";
import { Settings, SettingsService } from "@services/settings.service";
import { AppService } from "@services/app.service";
import { CompareOverviewComponent } from "@shared-components/compare-overview/compare-overview.component";
import { MatCardModule } from "@angular/material/card";
import { FlexLayoutModule } from "@ngbracket/ngx-layout";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { RouterModule } from "@angular/router";
import { CurrencyPipe, DatePipe } from "@angular/common";
import { RatingComponent } from "@shared-components/rating/rating.component";
import { ServicesService } from "@services/services.service";
import { Service } from "../../common/interfaces/service";
import { DecodingTokenService } from "@services/decoding-token.service";

@Component({
    selector: "app-property-item",
    standalone: true,
    imports: [
        RouterModule,
        FlexLayoutModule,
        MatCardModule,
        MatButtonModule,
        MatIconModule,
        SwiperModule,
        CurrencyPipe,
        RatingComponent,
        DatePipe,
    ],
    providers: [ServicesService, DecodingTokenService],
    templateUrl: "./property-item.component.html",
    styleUrls: ["./property-item.component.scss"],
})
export class PropertyItemComponent implements OnInit {
    @Input() property: Property;
    @Input() service: Service;
    @Input() viewType: string = "grid";
    @Input() viewColChanged: number = 0;
    @Input() fullWidthPage: boolean = true;
    public column: number = 4;
    public customerid: string;
    @ViewChild(SwiperDirective) directiveRef: SwiperDirective;
    public config: SwiperConfigInterface = {};
    private pagination: SwiperPaginationInterface = {
        el: ".swiper-pagination",
        clickable: true,
    };
    public settings: Settings;
    public Services_WishList: any;
    constructor(
        public settingsService: SettingsService,
        public appService: AppService,
        public cdr: ChangeDetectorRef,
        public myServ: ServicesService,
        public decodingCustomerID: DecodingTokenService
    ) {
        this.settings = this.settingsService.settings;
        this.customerid = this.decodingCustomerID.getUserIdFromToken();
    }
    // My Function
    public getImageUrl(imagePath: string): string {
        const baseUrl = "http://localhost:30480";
        return `${baseUrl}${imagePath}`;
    }
    ngOnInit() {
        // this.myServ.GetAllServices().subscribe({
        //     next: (data) => {
        //         // this.ServicesItems = data as Service[];
        //         console.log(data);
        //     },
        //     error: (err) => {
        //         console.log(err);
        //     },
        // });
    }

    ngAfterViewInit() {
        this.initCarousel();
        this.cdr.detectChanges();
        // this.appService.getAddress(this.property.location.lat, this.property.location.lng).subscribe(data=>{
        //   console.log(data['results'][0]['formatted_address']);
        //   this.address = data['results'][0]['formatted_address'];
        // })
    }

    ngOnChanges(changes: { [propKey: string]: SimpleChange }) {
        if (changes.viewColChanged) {
            this.getColumnCount(changes.viewColChanged.currentValue);
            if (!changes.viewColChanged.isFirstChange()) {
                if (this.property.gallery.length > 1) {
                    this.directiveRef.update();
                }
            }
        }

        for (let propName in changes) {
            // let changedProp = changes[propName];
            // if (!changedProp.isFirstChange()) {
            //   if(this.property.gallery.length > 1){
            //     this.initCarousel();
            //     this.config.autoHeight = true;
            //     this.directiveRef.update();
            //   }
            // }
        }
    }

    public getColumnCount(value) {
        if (value == 25) {
            this.column = 4;
        } else if (value == 33.3) {
            this.column = 3;
        } else if (value == 50) {
            this.column = 2;
        } else {
            this.column = 1;
        }
    }

    public initCarousel() {
        this.config = {
            slidesPerView: 1,
            spaceBetween: 0,
            keyboard: false,
            navigation: true,
            pagination: this.pagination,
            grabCursor: true,
            loop: true,
            preloadImages: false,
            lazy: true,
            nested: true,
            // autoplay: {
            //   delay: 5000,
            //   disableOnInteraction: false
            // },
            speed: 500,
            effect: "slide",
        };
    }

    public addToCompare() {
        this.appService.addToCompare(
            this.property,
            CompareOverviewComponent,
            this.settings.rtl ? "rtl" : "ltr"
        );
    }

    public onCompare() {
        return this.appService.Data.compareList.filter(
            (item) => item.id == this.property.id
        )[0];
    }

    public addToFavorites() {
        this.myServ.addToFavorites(
            this.service,
            this.settings.rtl ? "rtl" : "ltr",
            this.customerid //customerId
        );
    }

    // public onFavorites() {
    //     this.Services_WishList=this.myServ.getAllServicesInWishList(this.customerid) ;

    //     // Assuming getAllServicesInWishList is asynchronous and returns a Promise or Observable
    //     this.myServ.getAllServicesInWishList(this.customerid).subscribe((services) => {
    //       this.Services_WishList = services;

    //       // Check if this.service.id is already in the wishlist
    //       const isInWishlist = this.Services_WishList.some(servWish => servWish.Id === this.service.id);

    //       if (isInWishlist) {
    //         // Service is already in the wishlist, handle accordingly (e.g., update UI)
    //         console.log('Service is already in wishlist:', this.service.id);

    //         // Example: Update UI or set a flag
    //       } else {
    //         // Service is not in the wishlist
    //         console.log('Service is not in wishlist:', this.service.id);
    //         // Example: Handle adding to wishlist logic
    //       }
    //     }, (error) => {
    //       console.error('Failed to fetch wishlist services:', error);
    //       // Handle error scenario if needed
    //     });
    // }
}
